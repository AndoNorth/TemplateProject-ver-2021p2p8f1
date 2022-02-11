using UnityEngine;

namespace TemplateProject
{
    public class ColourSelector : MonoBehaviour
    {
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
        }
    }
}
