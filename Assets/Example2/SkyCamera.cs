/**,
 * Auth :   liubo
 * Date :   2019/04/29 16:58
 * Comment: 天空和  
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkyCamera : MonoBehaviour {

    static int _CornersId = Shader.PropertyToID("_Corners");
    public Material SkyMaterial;
    Mesh _SkyMesh;
    Mesh SkyMesh
    {
        get
        {
            if (_SkyMesh != null)
            {
                return _SkyMesh;
            }

            _SkyMesh = new Mesh();
            _SkyMesh.vertices = new Vector3[]
            {
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, -1, 0),
            };

            // dx平台，左上角是00，右下角是11
            _SkyMesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };
            //_SkyMesh.SetIndices(new int[] { 0, 1, 2, 3 }, MeshTopology.Quads, 0);
            _SkyMesh.SetIndices(new int[] { 0, 1, 2, 0, 3, 2 }, MeshTopology.Triangles, 0);
            return _SkyMesh;
        }
    }
    Vector4[] _CornersValue = new Vector4[4];
    void DrawSkybox(Camera cam, RenderTexture rt)
    {
        if (!SkyMaterial)
        {
            return;
        }

        // 每一帧，天空盒绘制的都是最远处的一个面片
        // camera朝向不同时，cam.ViewportToWorldPoint的结果不一样
        _CornersValue[0] = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.farClipPlane));
        _CornersValue[1] = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.farClipPlane));
        _CornersValue[2] = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.farClipPlane));
        _CornersValue[3] = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.farClipPlane));

        SkyMaterial.SetVectorArray(_CornersId, _CornersValue);
        SkyMaterial.SetPass(0);

        Graphics.SetRenderTarget(rt);
        Graphics.DrawMeshNow(SkyMesh, Matrix4x4.identity);
    }


	// Use this for initialization
	void Start () {
        RT = new RenderTexture(Screen.width, Screen.height, 24);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    RenderTexture RT;
    private void OnPostRender()
    {
        var cam = Camera.current;
        Graphics.SetRenderTarget(RT);
        GL.Clear(true, true, Color.gray);
        DrawSkybox(cam, RT);
        Graphics.Blit(RT, cam.targetTexture);
    }
}
