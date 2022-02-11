using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// brief: this class has multiple custom mesh functions
public static class MeshSystem
{
    private static Quaternion[] cachedQuaternionEulerArr;
    // brief: initialize 1 x 1 mesh - one sided mesh ( almost equivalent to sprite renderer )
    public static void Create1x1Mesh(out MeshStruct _meshStruct, out Mesh _mesh)
    {
        // this MESH code is equivalent to the sprite renderer
        // https://www.youtube.com/watch?v=11c9rWRotJ8
        // 4, 4, 6
        // the number of vertices should match the no.UnitVectors (uv)
        // vertices is equal to the number of sides of the polygon - 4 sides per square
        // uv is equal to the number of points required to make the mesh - the perimeter
        // this is the number of sides equivalent to create the mesh - 3 sides per triangle
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        // initialise the vertices
        // these define the coordinates of each "polygon"
        //
        // 0 x * * * x 1
        //   *       *
        //   *       *
        //   *       *
        // 2 x * * * x 3
        //
        vertices[0] = new Vector3((float)-0.5, (float)0.5); // top left
        vertices[1] = new Vector3((float)0.5, (float)0.5); // top right
        vertices[2] = new Vector3((float)-0.5, (float)-0.5); // bottom left
        vertices[3] = new Vector3((float)0.5, (float)-0.5); // bottom right
        // previous method (this is not centered, above method centers mesh)
        /* vertices[0] = new Vector3(0, 1); // top left
        vertices[1] = new Vector3(1, 1); // top right
        vertices[2] = new Vector3(0, 0); // bottom left
        vertices[3] = new Vector3(1, 0); // bottom right */

        // initialise the triangle's
        // these need to be ordered clockwise 
        // if done counter clockwise then will render on opposite face
        // these follow the order of the vertices
        //  - triangle 1 -
        //  0 x -- x 1
        //    |   /
        //    |  /
        //    | /
        //  2 x
        //  - triangle 1 -
        triangles[0] = 0; // top left, 0
        triangles[1] = 1; // -> top right, 1
        triangles[2] = 2; // -> bottom right, 2
        // - triangle 2 -
        //         x 1
        //       / |
        //      /  |
        //     /   |
        //  2 x -- x 0
        // - triangle 2 -
        triangles[3] = 2; // bottom right, 2
        triangles[4] = 1; // -> top right, 1
        triangles[5] = 3; // -> bottom left, 3

        // initialise the uv's
        // this will initialize the entire texture onto the mesh
        uv[0] = new Vector2(0, 1); // top left
        uv[1] = new Vector2(1, 1); // top right
        uv[2] = new Vector2(0, 0); // bottom left
        uv[3] = new Vector2(1, 0); // bottom right

        _mesh = new Mesh();
        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;

        _meshStruct = new MeshStruct(vertices, uv, triangles);
    }
    // brief: initialise n x n meshStruct
    public static void CreateEmptyMeshStruct(int quadCount, out MeshStruct meshStruct)
    {
        Vector3[] vertices = new Vector3[4 * quadCount];
        Vector2[] uv = new Vector2[4 * quadCount];
        int[] triangles = new int[6 * quadCount];

        meshStruct = new MeshStruct(vertices, uv, triangles);
    }
    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles)
    {
        vertices = new Vector3[4 * quadCount];
        uvs = new Vector2[4 * quadCount];
        triangles = new int[6 * quadCount];
    }
    // brief: 
    private static void CacheQuaternionEuler()
    {
        if (cachedQuaternionEulerArr != null) return;
        cachedQuaternionEulerArr = new Quaternion[360];
        for (int i = 0; i < 360; i++)
        {
            cachedQuaternionEulerArr[i] = Quaternion.Euler(0, 0, i);
        }
    }
    private static Quaternion GetQuaternionEuler(float rotFloat)
    {
        int rot = Mathf.RoundToInt(rotFloat);
        rot = rot % 360;
        if (rot < 0) rot += 360;
        //if (rot >= 360) rot -= 360;
        if (cachedQuaternionEulerArr == null) CacheQuaternionEuler();
        return cachedQuaternionEulerArr[rot];
    }
    // brief: populate meshStruct
    public static void AddToMeshArrays(MeshStruct meshStruct, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        //Relocate vertices
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.y;
        if (skewed)
        {
            meshStruct.vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            meshStruct.vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            meshStruct.vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            meshStruct.vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            meshStruct.vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            meshStruct.vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            meshStruct.vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            meshStruct.vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
        }

        //Relocate UVs
        meshStruct.uv[vIndex0] = new Vector2(uv00.x, uv11.y);
        meshStruct.uv[vIndex1] = new Vector2(uv00.x, uv00.y);
        meshStruct.uv[vIndex2] = new Vector2(uv11.x, uv00.y);
        meshStruct.uv[vIndex3] = new Vector2(uv11.x, uv11.y);

        //Create triangles
        int tIndex = index * 6;

        meshStruct.triangles[tIndex + 0] = vIndex0;
        meshStruct.triangles[tIndex + 1] = vIndex3;
        meshStruct.triangles[tIndex + 2] = vIndex1;

        meshStruct.triangles[tIndex + 3] = vIndex1;
        meshStruct.triangles[tIndex + 4] = vIndex3;
        meshStruct.triangles[tIndex + 5] = vIndex2;
    }
    public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uvs, int[] triangles, int index, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        //Relocate vertices
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.y;
        if (skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
        }

        //Relocate UVs
        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        //Create triangles
        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex3;
        triangles[tIndex + 2] = vIndex1;

        triangles[tIndex + 3] = vIndex1;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;
    }
    // brief: 
    public static Mesh AddToMesh(Mesh mesh, Vector3 pos, float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
    {
        if (mesh == null)
        {
            MeshStruct meshStruct;
            Create1x1Mesh(out meshStruct, out mesh);
        }
        Vector3[] vertices = new Vector3[4 + mesh.vertices.Length];
        Vector2[] uvs = new Vector2[4 + mesh.uv.Length];
        int[] triangles = new int[6 + mesh.triangles.Length];

        mesh.vertices.CopyTo(vertices, 0);
        mesh.uv.CopyTo(uvs, 0);
        mesh.triangles.CopyTo(triangles, 0);


        int index = vertices.Length / 4 - 1;
        //Relocate vertices
        int vIndex = index * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        baseSize *= .5f;

        bool skewed = baseSize.x != baseSize.y;
        if (skewed)
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y);
            vertices[vIndex1] = pos + GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y);
            vertices[vIndex2] = pos + GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y);
            vertices[vIndex3] = pos + GetQuaternionEuler(rot) * baseSize;
        }
        else
        {
            vertices[vIndex0] = pos + GetQuaternionEuler(rot - 270) * baseSize;
            vertices[vIndex1] = pos + GetQuaternionEuler(rot - 180) * baseSize;
            vertices[vIndex2] = pos + GetQuaternionEuler(rot - 90) * baseSize;
            vertices[vIndex3] = pos + GetQuaternionEuler(rot - 0) * baseSize;
        }

        //Relocate UVs
        uvs[vIndex0] = new Vector2(uv00.x, uv11.y);
        uvs[vIndex1] = new Vector2(uv00.x, uv00.y);
        uvs[vIndex2] = new Vector2(uv11.x, uv00.y);
        uvs[vIndex3] = new Vector2(uv11.x, uv11.y);

        //Create triangles
        int tIndex = index * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex3;
        triangles[tIndex + 2] = vIndex1;

        triangles[tIndex + 3] = vIndex1;
        triangles[tIndex + 4] = vIndex3;
        triangles[tIndex + 5] = vIndex2;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        //mesh.bounds = bounds;

        return mesh;
    }
    // brief: converts pixel coordinates to uv
    public static Vector2 ConvertPixelToUVCoordinates(int x, int y, int textureWidth, int textureHeight)
    {
        return new Vector2((float)x / textureWidth, (float)y / textureHeight);
    }
    // brief: maps uv coordinates from "pixels" - starting from the bottom left of the input texture to the top right
    public static Vector2[] GetUVRectangleFromPixels(int x, int y, int width, int height, int textureWidth, int textureHeight)
    {
        return new Vector2[] {
            ConvertPixelToUVCoordinates(x         , y + height, textureWidth, textureHeight),
            ConvertPixelToUVCoordinates(x + width , y + height, textureWidth, textureHeight),
            ConvertPixelToUVCoordinates(x         , y         , textureWidth, textureHeight),
            ConvertPixelToUVCoordinates(x + width , y         , textureWidth, textureHeight)
        };

    }
    public static Vector2[] GetUVRectangleFromPixels(int[] pixelArray)
    {
        return new Vector2[] {
            ConvertPixelToUVCoordinates(pixelArray[0] , pixelArray[1] + pixelArray[3] , pixelArray[4] , pixelArray[5]),
            ConvertPixelToUVCoordinates(pixelArray[0] + pixelArray[2] , pixelArray[1] + pixelArray[3] , pixelArray[4] , pixelArray[5]),
            ConvertPixelToUVCoordinates(pixelArray[0] , pixelArray[1] , pixelArray[4] , pixelArray[5]),
            ConvertPixelToUVCoordinates(pixelArray[0] + pixelArray[2] , pixelArray[1] , pixelArray[4] , pixelArray[5])
        };

    }

    // brief: apply the uv mapping to reference uv
    public static void ApplyUVtoUVArray(Vector2[] uv, Vector2[] mainUV) // previously ref Vector2[] mainUV
    {
        // this would need to be expanded if more UV's are used
        // for example a huge texture with multiple polygons
        mainUV[0] = uv[0];
        mainUV[1] = uv[1];
        mainUV[2] = uv[2];
        mainUV[3] = uv[3];
    }

    // brief: convert sprite to texture then applies to material and returns that for use by mesh renderer
    public static Material ConvertSprite2Material(Sprite _sprite)
    {
        Material _material = new Material(Shader.Find("Unlit/Transparent"));
        Texture _texture = _sprite.texture;
        _material.mainTexture = _texture;

        return _material;
    }
}
