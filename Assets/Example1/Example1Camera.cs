/**,
 * Auth :   liubo
 * Date :   2019/04/29 15:36
 * Comment: 2018.2.6f1版本是好用的  
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Example1Camera : MonoBehaviour
{
    RenderTexture RT;

    public Material SolidMaterial;
    public MeshFilter Cube;

    // Start is called before the first frame update
    void Start()
    {
        RT = new RenderTexture(Screen.width, Screen.height, 24);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPostRender()
    {
        var cam = Camera.current;

        if (!Cube || !SolidMaterial)
        {
            Graphics.SetRenderTarget(RT);
            GL.Clear(true, true, Color.gray);
            Graphics.Blit(RT, cam.targetTexture);
            return;
        }

        Graphics.SetRenderTarget(RT);
        GL.Clear(true, true, Color.black);
        SolidMaterial.SetPass(0);
        //Graphics.DrawMeshNow(Cube.mesh, Cube.transform.localToWorldMatrix);
        Graphics.DrawMeshNow(Cube.sharedMesh, Cube.transform.position, Cube.transform.rotation);
        Graphics.Blit(RT, cam.targetTexture);
    }
}
