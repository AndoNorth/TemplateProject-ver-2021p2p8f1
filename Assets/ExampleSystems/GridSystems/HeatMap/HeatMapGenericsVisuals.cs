using UnityEngine;

namespace TemplateProject
{
    // this script uses grid and mesh systems to show heatmap visuals
    public class HeatMapGenericsVisuals : MonoBehaviour
    {
        GridSystem<HeatMapGridObject> grid;
        public Sprite sprite;
        Material material;
        MeshStruct meshStruct;
        Mesh mesh;
        bool updateMesh = false;
        // user changable
        public bool strengthReduction = true;
        [Range(1, 256)]
        public int centerValue = 128;
        public int maxRange = 8, fullValRange = 3;
        private void Awake()
        {
            material = MeshSystem.ConvertSprite2Material(sprite);
            mesh = new Mesh();
        }
        public void SetupGrid(GridSystem<HeatMapGridObject> grid)
        {
            this.grid = grid;

            grid.OnGridObjectChanged += Grid_OnGridValueChanged;

            MeshSystem.CreateEmptyMeshStruct(grid.GetWidth() * grid.GetHeight(), out meshStruct);

            MeshSystem.ApplyUVtoUVArray(MeshSystem.GetUVRectangleFromPixels(1, 1, 1, 16, sprite.texture.width, sprite.texture.height), meshStruct.uv);

            // initialise mesh properties into variables
            mesh.vertices = meshStruct.vertices;
            mesh.uv = meshStruct.uv;
            mesh.triangles = meshStruct.triangles;

            UpdateHeatMapVisual();

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
                if (strengthReduction)
                {
                    AddValueDiamond(position, centerValue, fullValRange, maxRange);
                }
                else
                {
                    AddValueDiamond(position, centerValue, maxRange);
                }
            }
        }
        private void LateUpdate()
        {
            if (updateMesh)
            {
                updateMesh = false;
                UpdateHeatMapVisual();
            }
        }
        // heatmap
        private void UpdateHeatMapVisual()
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

                    HeatMapGridObject gridObject = grid.GetGridObject(x, y);
                    float gridValueNormalized = gridObject.GetValueNormalized();
                    Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                    MeshSystem.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
        private void Grid_OnGridValueChanged(object sender, GridSystem<HeatMapGridObject>.OnGridObjectChangedEventArgs e)
        {
            updateMesh = true;
        }
        private void getXYfromWP(Vector3 worldPosition, out int x, out int y)
        {
            grid.getXYfromWP(worldPosition, out x, out y);
        }
        public void AddValue(int x, int y, int value)
        {
            HeatMapGridObject gridObj = grid.GetGridObject(x, y);
            if (gridObj?.GetValue() >= HeatMapGridObject.HEAT_MAP_MAX_VALUE)
            {
                return;
            }
            gridObj?.AddValue(value);
        }
        public void AddValue(Vector3 worldPos, int value)
        {
            HeatMapGridObject gridObj = grid.GetGridObject(worldPos);
            if (gridObj?.GetValue() >= HeatMapGridObject.HEAT_MAP_MAX_VALUE)
            {
                return;
            }
            gridObj?.AddValue(value);
        }
        public void AddValueSquare(Vector3 worldPos, int value, int range)
        {
            getXYfromWP(worldPos, out int originX, out int originY);
            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y < range; y++)
                {
                    AddValue(originX + x, originY + y, value);
                    if (x != 0 && y != 0)
                    {
                        AddValue(originX - x, originY - y, value);
                    }
                }

            }
        }
        public void AddValueDiamond(Vector3 worldPos, int value, int range)
        {
            getXYfromWP(worldPos, out int originX, out int originY);
            for (int x = 0; x < range; x++)
            {
                for (int y = 0; y + x < range; y++)
                {
                    AddValue(originX + x, originY + y, value);
                    if (x != 0)
                    {
                        AddValue(originX - x, originY + y, value);
                    }
                    if (y != 0)
                    {
                        AddValue(originX + x, originY - y, value);
                        if (x != 0)
                        {
                            AddValue(originX - x, originY - y, value);
                        }
                    }
                }
            }
        }
        public void AddValueDiamond(Vector3 worldPos, int value, int fullValueRange, int totalRange)
        {
            int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));

            getXYfromWP(worldPos, out int originX, out int originY);
            for (int x = 0; x < totalRange; x++)
            {
                for (int y = 0; y + x < totalRange; y++)
                {
                    int radius = x + y;
                    int addValueAmount = value;
                    if (radius > fullValueRange)
                    {
                        addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                    }

                    AddValue(originX + x, originY + y, addValueAmount); // top-right
                    if (x != 0)
                    {
                        AddValue(originX - x, originY + y, addValueAmount); // top-left
                    }
                    if (y != 0)
                    {
                        AddValue(originX + x, originY - y, addValueAmount); // bottom-right
                        if (x != 0)
                        {
                            AddValue(originX - x, originY - y, addValueAmount); // bottom-left
                        }
                    }
                }
            }
        }
    }
}
