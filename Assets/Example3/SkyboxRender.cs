using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkyboxRender
{
    public Material SkyMaterial;

    static Mesh _Mesh;
    static Mesh SkyMesh
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

    static int CornersId = Shader.PropertyToID("_Corners");
    
    public void Draw(Camera cam, RenderTexture rt)
    {
        if (!SkyMaterial)
        {
            return;
        }

        var Corners = new Vector4[]
            {
                cam.ViewportToWorldPoint(new Vector3(0, 0, cam.farClipPlane)),
                cam.ViewportToWorldPoint(new Vector3(1, 0, cam.farClipPlane)),
                cam.ViewportToWorldPoint(new Vector3(0, 1, cam.farClipPlane)),
                cam.ViewportToWorldPoint(new Vector3(1, 1, cam.farClipPlane)),
            };
        SkyMaterial.SetVectorArray(CornersId, Corners);

        SkyMaterial.SetPass(0);
        Graphics.DrawMeshNow(SkyMesh, Matrix4x4.identity);
    }
}
