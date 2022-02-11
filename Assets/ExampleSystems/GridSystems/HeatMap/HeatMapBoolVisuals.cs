using UnityEngine;

namespace TemplateProject
{
    // this script uses grid and mesh systems to show heatmap visuals
    public class HeatMapBoolVisuals : MonoBehaviour
    {
        GridSystem<bool> grid;
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
        public void SetupGrid(GridSystem<bool> grid)
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
                grid.SetGridObject(position, true);
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

                    bool gridValue = grid.GetGridObject(x, y);
                    float gridValueNormalized = gridValue ? 1f : 0f;
                    Vector2 gridValueUV = new Vector2(gridValueNormalized, 0f);
                    MeshSystem.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridValueUV, gridValueUV);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
        private void Grid_OnGridValueChanged(object sender, GridSystem<bool>.OnGridObjectChangedEventArgs e)
        {
            updateMesh = true;
        }
    }
}
