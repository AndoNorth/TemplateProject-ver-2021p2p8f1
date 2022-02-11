using UnityEngine;

namespace TemplateProject
{
    public class PixelArtDrawingTest : MonoBehaviour
    {
        public bool showDebug;
        public Vector2[] colorUVs;

        private GridSystem<GridObject> grid;
        private Vector2 colorUV = Vector2.zero;
        private Texture2D colorTexture2D;
        [SerializeField]
        private PixelArtDrawingVisuals pixelArtDrawingSystemVisual;
        [SerializeField]
        private LoadPixelArtDrawing loadPixelArtDrawing;
        [SerializeField]
        private ColourSelector colourSelector;

        private void Awake()
        {
            grid = new GridSystem<GridObject>(16, 16, 1f, Vector3.zero, (GridSystem<GridObject> g, int x, int y) => new GridObject(g, x, y), showDebug);
        }
        private void Start()
        {
            pixelArtDrawingSystemVisual.SetupGrid(grid);
            colorTexture2D = pixelArtDrawingSystemVisual.GetTexture2D();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 position = GeneralUtility.GetMouseWorldPosition();
                grid.GetGridObject(position).SetColorUV(colorUV);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                colorUV = colorUVs[0];
                Debug.Log("Color 0 set");
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                colorUV = colorUVs[1];
                Debug.Log("Color 1 set");
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                colorUV = colorUVs[2];
                Debug.Log("Color 2 set");
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                colorUV = colorUVs[3];
                Debug.Log("Color 3 set");
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                loadPixelArtDrawing.SetTexture(GetTilemapTexture());
                Debug.Log("loaded on display mesh");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                SaveImage();
                Debug.Log("saved");
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                loadPixelArtDrawing.LoadImageFromFile();
                Debug.Log("loaded");
            }
        }

        private void SaveImage()
        {
            Texture2D texture2D = GetTilemapTexture();
            byte[] byteArray = texture2D.EncodeToPNG();
            if (!System.IO.File.Exists(Application.dataPath + LoadPixelArtDrawing.IMAGE_FILE_PATH))
            {
                System.IO.Directory.CreateDirectory(Application.dataPath + LoadPixelArtDrawing.IMAGE_FILE_PATH);
            }
            System.IO.File.WriteAllBytes(Application.dataPath + LoadPixelArtDrawing.IMAGE_FILE_PATH + LoadPixelArtDrawing.IMAGE_FILE_NAME, byteArray);
        }
        private Texture2D GetTilemapTexture()
        {
            int gridWidth, gridHeight;
            gridWidth = grid.GetWidth();
            gridHeight = grid.GetHeight();
            Texture2D texture2D = new Texture2D(gridWidth, gridHeight, TextureFormat.ARGB32, false);
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    GridObject gridObject = grid.GetGridObject(x, y);

                    int pixelX = (int)(gridObject.GetColorUV().x * colorTexture2D.width);
                    int pixelY = (int)(gridObject.GetColorUV().y * colorTexture2D.height);

                    Color pixelColor = colorTexture2D.GetPixel(pixelX, pixelY);

                    texture2D.SetPixel(x, y, pixelColor);
                }
            }
            texture2D.Apply();
            return texture2D;
        }
        public class GridObject
        {
            private GridSystem<GridObject> grid;
            private int x;
            private int y;
            private Vector2 colorUV;

            public GridObject(GridSystem<GridObject> grid, int x, int y)
            {
                this.grid = grid;
                this.x = x;
                this.y = y;
            }
            public void SetColorUV(Vector2 colorUV)
            {
                this.colorUV = colorUV;
                grid.TriggerGridObjectChanged(x, y);
            }
            public Vector2 GetColorUV()
            {
                return colorUV;
            }

            public override string ToString()
            {
                return (colorUV.x).ToString();
            }
        }
    }
}
