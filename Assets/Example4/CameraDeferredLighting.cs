/**,
 * Auth :   liubo
 * Date :   2019/05/07 22:11
 * Comment: 效仿learn-OpenGl中的样例(https://learnopengl.com/Advanced-Lighting/Deferred-Shading)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDeferredLighting : MonoBehaviour {
    
    public MeshFilter[] _MeshList;
    public Light[] _LightList;

    public float LightFactor = 0.01f;

    RenderTexture RT;
    RenderTexture DepthTexture;
    RenderTexture[] GBufferTextures;
    RenderBuffer[] GBuffers;
    int[] GBufferIds;
    
    public Material DeferMaterial;

	// Use this for initialization
	void Start () {
        CreateRT();
    }
    static int Width()
    {
        return Screen.width;
    }
    static int Height()
    {
        return Screen.height;
    }
    void CreateRT()
    {
        RT = new RenderTexture(Width(), Height(), 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        DepthTexture = new RenderTexture(Width(), Height(), 24, RenderTextureFormat.Depth, RenderTextureReadWrite.Linear);
        GBufferTextures = new RenderTexture[]
            {
                new RenderTexture(Width(), Height(), 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Width(), Height(), 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Width(), Height(), 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
                new RenderTexture(Width(), Height(), 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear),
            };
        GBuffers = new RenderBuffer[GBufferTextures.Length];
        GBufferIds = new int[GBufferTextures.Length];
        for (int i = 0; i < GBuffers.Length; i++)
        {
            GBuffers[i] = GBufferTextures[i].colorBuffer;
            GBufferIds[i] = Shader.PropertyToID("_GBuffer" + i);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnPostRender()
    {
        GL.Clear(true, true, Color.gray);

        if (GBuffers == null)
        {
            CreateRT();
        }

        for (int i = 0; i < GBufferIds.Length; i++)
        {
            Shader.SetGlobalTexture(GBufferIds[i], GBufferTextures[i]);
        }
        if (GBuffers == null || GBuffers.Length == 0 || GBuffers[0].GetNativeRenderBufferPtr() == System.IntPtr.Zero)
        {
            UnityEngine.Debug.LogError("error!");
            return;
        }
        Graphics.SetRenderTarget(GBuffers, DepthTexture.depthBuffer);
        GL.Clear(true, true, Color.black);
        foreach (var mesh in _MeshList)
        {
            var r = mesh.GetComponent<MeshRenderer>();
            var m = mesh.sharedMesh;
            if (!r || r.sharedMaterials.Length == 0 || !m)
            {
                continue;
            }
            var mats = r.sharedMaterials;
            for (int i = 0; i < m.subMeshCount; i++)
            {
                mats[i].SetPass(0);
                //mats[i].SetMatrix(Shader.PropertyToID("model"), mesh.transform.localToWorldMatrix);
                Graphics.DrawMeshNow(m, mesh.transform.localToWorldMatrix, i);
            }
        }

        Graphics.SetRenderTarget(RT);
        GL.Clear(true, true, Color.black);
        if (DeferMaterial)
        {
            //DeferMaterial.SetPass(0);
            //Graphics.DrawMeshNow(MyQuad.QuadMesh, Matrix4x4.identity);
            if (this._LightList != null)
            {
                DeferMaterial.SetInt(Shader.PropertyToID("_LightCount"), this._LightList.Length);
                var lightsPosition = new List<Vector4>();
                var lightsColor = new List<Vector4>();
                var lightsLinear = new List<float>();
                var lightsQuadratic = new List<float>();

                for (int i = 0; i < this._LightList.Length; i++)
                {
                    var light = this._LightList[i];
                    lightsPosition.Add(light.transform.position);
                    lightsColor.Add(light.color);
                    lightsLinear.Add(0.7f * LightFactor);
                    lightsQuadratic.Add(1.8f * LightFactor);
                }
                DeferMaterial.SetVectorArray(Shader.PropertyToID("lightsPosition"), lightsPosition.ToArray());
                DeferMaterial.SetVectorArray(Shader.PropertyToID("lightsColor"), lightsColor.ToArray());
                DeferMaterial.SetFloatArray(Shader.PropertyToID("lightsLinear"), lightsLinear.ToArray());
                DeferMaterial.SetFloatArray(Shader.PropertyToID("lightsQuadratic"), lightsQuadratic.ToArray());
            }


            Graphics.Blit(null, RT, DeferMaterial, 0);
        }

        Graphics.Blit(RT, Camera.current.targetTexture);
    }
}
