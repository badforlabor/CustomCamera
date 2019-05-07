using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuad
{
    static Mesh _Mesh;
    public static Mesh QuadMesh
    {
        get
        {
            if (!_Mesh)
            {
                _Mesh = new Mesh();
                _Mesh.vertices = new Vector3[]
                    {
                        new Vector3(-1, 1, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, -1, 0),
                        new Vector3(-1, -1, 0),
                    };
                _Mesh.uv = new Vector2[]
                    {
                        new Vector2(0, 0),
                        new Vector2(1, 0),
                        new Vector2(1, 1),
                        new Vector2(0, 1),
                    };
                _Mesh.SetIndices(new int[] { 0, 1, 2, 0, 2, 3 }, MeshTopology.Triangles, 0);
            }
            return _Mesh;
        }
    }
}
