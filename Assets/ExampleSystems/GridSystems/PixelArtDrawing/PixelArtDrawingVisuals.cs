using UnityEngine;

namespace TemplateProject
{
    // this script uses grid and mesh systems to show heatmap visuals
    public class PixelArtDrawingVisuals : MonoBehaviour
    {
        GridSystem<PixelArtDrawingTest.GridObject> grid;
        public Sprite sprite;
        Material material;
        MeshStruct meshStruct;
        Mesh mesh;
        bool updateMesh = false;

        private void Awake()
        {
            material = MeshSystem.ConvertSprite2Material(sprite);
            mesh = new Mesh();

            Texture texture = material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;
        }
        public void SetupGrid(GridSystem<PixelArtDrawingTest.GridObject> grid)
        {
            this.grid = grid;

            grid.OnGridObjectChanged += Grid_OnGridValueChanged;

            MeshSystem.CreateEmptyMeshStruct(grid.GetWidth() * grid.GetHeight(), out meshStruct);

            MeshSystem.ApplyUVtoUVArray(MeshSystem.GetUVRectangleFromPixels(1, 1, 1, 16, sprite.texture.width, sprite.texture.height), meshStruct.uv);

            // initialise mesh properties into variables
            mesh.vertices = meshStruct.vertices;
            mesh.uv = meshStruct.uv;
            mesh.triangles = meshStruct.triangles;

            UpdateVisual();

            // initialise gameObject with appropriate properties
            GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.transform.SetParent(transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1); // (x , y, z) - size of mesh in local world
            gameObject.transform.localPosition = new Vector3(0, 0, 0);

            // set gameObject components to proposed mesh
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().material = material;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && grid != null)
            {
                Vector3 position = GeneralUtility.GetMouseWorldPosition();
            }
        }
        private void LateUpdate()
        {
            if (updateMesh)
            {
                updateMesh = false;
                UpdateVisual();
            }
        }
        private void UpdateVisual()
        {
            int gridWidth = grid.GetWidth();
            int gridHeight = grid.GetHeight();
            MeshSystem.CreateEmptyMeshArrays(gridWidth * gridHeight, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int index = x * gridHeight + y;
                    Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                    PixelArtDrawingTest.GridObject gridObject = grid.GetGridObject(x, y);
                    Vector2 gridUV00, gridUV11;

                    gridUV00 = gridObject.GetColorUV();
                    gridUV11 = gridObject.GetColorUV();

                    MeshSystem.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
        private void Grid_OnGridValueChanged(object sender, GridSystem<PixelArtDrawingTest.GridObject>.OnGridObjectChangedEventArgs e)
        {
            updateMesh = true;
        }
        private void getXYfromWP(Vector3 worldPosition, out int x, out int y)
        {
            grid.getXYfromWP(worldPosition, out x, out y);
        }
        public Texture2D GetTexture2D()
        {
            return sprite.texture;
        }
    }
}
