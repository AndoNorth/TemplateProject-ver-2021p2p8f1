using UnityEngine;

public struct MeshStruct
{
    // declare variables
    public Vector3[] vertices;
    public Vector2[] uv;
    public int[] triangles;
    // constructor
    public MeshStruct(Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        this.vertices = vertices;
        this.uv = uv;
        this.triangles = triangles;
    }
}

