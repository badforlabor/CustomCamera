using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSphereAndSkybox : MonoBehaviour {


    public MeshFilter[] Spheres;
    public Material SphereMaterial;
    public SkyboxRender Skybox;

    RenderTexture RT = null;
    int ScreenWidth;
    int ScreenHeight;

    // Use this for initialization
    void Start () {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;
        RT = new RenderTexture(ScreenWidth, ScreenHeight, 24);
    }

    void OnResize()
    {
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        RT.Release();
        RT.width = ScreenWidth;
        RT.height = ScreenHeight;
        RT.Create();
    }

	// Update is called once per frame
	void Update () {
		
	}
    private void OnPostRender()
    {
        if (Screen.width != ScreenWidth || Screen.height != ScreenHeight)
        {
            OnResize();
        }

        var cam = Camera.current;

        Graphics.SetRenderTarget(RT);
        GL.Clear(true, true, Color.gray);

        if (SphereMaterial)
        {
            SphereMaterial.SetPass(0);
            foreach (var sphere in Spheres)
            {
                Graphics.DrawMeshNow(sphere.sharedMesh, sphere.transform.localToWorldMatrix);
            }
        }

        Skybox.Draw(cam, RT);

        Graphics.Blit(RT, cam.targetTexture);
    }
}
