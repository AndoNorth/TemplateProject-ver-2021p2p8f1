using System.Collections.Generic;
using UnityEngine;

namespace TemplateProject
{
    // this script uses grid and mesh systems to show heatmap visuals
    public class TilemapVisuals : MonoBehaviour
    {
        [System.Serializable]
        public struct TilemapSpriteUV
        {
            public TilemapSystem.TilemapObject.TilemapSprite tilemapSprite;
            public Vector2Int uv00Pixels;
            public Vector2Int uv11Pixels;
        }

        private struct UVCoords
        {
            public Vector2 uv00;
            public Vector2 uv11;
        }

        [SerializeField]
        private TilemapSpriteUV[] tilemapSpriteUVArray;
        GridSystem<TilemapSystem.TilemapObject> grid;
        public Sprite sprite;
        Material material;
        MeshStruct meshStruct;
        Mesh mesh;
        bool updateMesh = false;
        private Dictionary<TilemapSystem.TilemapObject.TilemapSprite, UVCoords> uvCoordsDictionary;

        private void Awake()
        {
            material = MeshSystem.ConvertSprite2Material(sprite);
            mesh = new Mesh();

            Texture texture = material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            uvCoordsDictionary = new Dictionary<TilemapSystem.TilemapObject.TilemapSprite, UVCoords>();

            foreach (TilemapSpriteUV tilemapSpriteUV in tilemapSpriteUVArray)
            {
                uvCoordsDictionary[tilemapSpriteUV.tilemapSprite] = new UVCoords
                {
                    uv00 = new Vector2(tilemapSpriteUV.uv00Pixels.x / textureWidth, tilemapSpriteUV.uv00Pixels.y / textureHeight),
                    uv11 = new Vector2(tilemapSpriteUV.uv11Pixels.x / textureWidth, tilemapSpriteUV.uv11Pixels.y / textureHeight)
                };
            }
        }
        public void SetupGrid(TilemapSystem tilemap, GridSystem<TilemapSystem.TilemapObject> grid)
        {
            this.grid = grid;

            grid.OnGridObjectChanged += Grid_OnGridValueChanged;

            MeshSystem.CreateEmptyMeshStruct(grid.GetWidth() * grid.GetHeight(), out meshStruct);

            MeshSystem.ApplyUVtoUVArray(MeshSystem.GetUVRectangleFromPixels(1, 1, 1, 16, sprite.texture.width, sprite.texture.height), meshStruct.uv);

            // initialise mesh properties into variables
            mesh.vertices = meshStruct.vertices;
            mesh.uv = meshStruct.uv;
            mesh.triangles = meshStruct.triangles;

            UpdateTilemapVisual();

            // initialise gameObject with appropriate properties
            GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.transform.SetParent(transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1); // (x , y, z) - size of mesh in local world
            gameObject.transform.localPosition = new Vector3(0, 0, 0);

            // set gameObject components to proposed mesh
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
            gameObject.GetComponent<MeshRenderer>().material = material;
            tilemap.OnLoaded += TIlemap_OnLoaded;
        }
        private void TIlemap_OnLoaded(object sender, System.EventArgs e)
        {
            updateMesh = true;
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
                UpdateTilemapVisual();
            }
        }
        private void UpdateTilemapVisual()
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

                    TilemapSystem.TilemapObject gridObject = grid.GetGridObject(x, y);
                    TilemapSystem.TilemapObject.TilemapSprite tilemapSprite = gridObject.GetTilemapSprite();
                    Vector2 gridUV00, gridUV11;
                    if (tilemapSprite == TilemapSystem.TilemapObject.TilemapSprite.None)
                    {
                        gridUV00 = Vector2.zero;
                        gridUV11 = Vector2.zero;
                        quadSize = Vector3.zero; // doesnt draw the mesh, saves GPU
                    }
                    else
                    {
                        UVCoords uvCoords = uvCoordsDictionary[tilemapSprite];
                        gridUV00 = uvCoords.uv00;
                        gridUV11 = uvCoords.uv11;
                    }
                    MeshSystem.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
        private void Grid_OnGridValueChanged(object sender, GridSystem<TilemapSystem.TilemapObject>.OnGridObjectChangedEventArgs e)
        {
            updateMesh = true;
        }
        private void getXYfromWP(Vector3 worldPosition, out int x, out int y)
        {
            grid.getXYfromWP(worldPosition, out x, out y);
        }
    }
}
