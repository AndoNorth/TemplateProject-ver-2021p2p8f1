using UnityEngine;

namespace TemplateProject
{
    public class LoadPixelArtDrawing : MonoBehaviour
    {
        public static readonly string IMAGE_FILE_PATH = "/ExampleSystems/GridSystems/PixelArtDrawing/Save/";
        public static readonly string IMAGE_FILE_NAME = "/pixelArt.png";

        MeshStruct meshStruct;
        Mesh mesh;
        MeshRenderer meshRenderer;

        private void Awake()
        {
            mesh = new Mesh();
            // initialise gameObject with appropriate properties
            GameObject gameObject = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
            gameObject.transform.SetParent(transform);
            gameObject.transform.localScale = new Vector3(1, 1, 1); // (x , y, z) - size of mesh in local world
            gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        // Start is called before the first frame update
        void Start()
        {
            MeshSystem.Create1x1Mesh(out meshStruct, out mesh);

            // initialise mesh properties into variables
            mesh.vertices = meshStruct.vertices;
            mesh.uv = meshStruct.uv;
            mesh.triangles = meshStruct.triangles;

            // set gameObject components to proposed mesh
            GetComponentInChildren<MeshFilter>().mesh = mesh;
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            LoadImageFromFile();
        }
        public void LoadImageFromFile()
        {
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.filterMode = FilterMode.Point;

            if (!System.IO.File.Exists(Application.dataPath + IMAGE_FILE_PATH + IMAGE_FILE_NAME))
            {
                return;
            }

            byte[] byteArray = System.IO.File.ReadAllBytes(Application.dataPath + IMAGE_FILE_PATH + IMAGE_FILE_NAME);

            texture2D.LoadImage(byteArray);

            meshRenderer.material.mainTexture = texture2D;
        }

        public void SetTexture(Texture2D texture2D)
        {
            texture2D.filterMode = FilterMode.Point;
            meshRenderer.material.mainTexture = texture2D;
        }
    }
}
