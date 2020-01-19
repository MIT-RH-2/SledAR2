using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowMaker : MonoBehaviour
{
    // I move around, make particles, write to a render texture and use the rendertexture to update the
    // snow material shader on the mesh
    

    int falloffRadius = 100;
    [SerializeField] Texture brushTexture;
    [SerializeField] GameObject environment;
    [SerializeField] Material snowMaterial;
    [SerializeField] GameObject playArea;
    [SerializeField] int resolution = 1024;
    [SerializeField] float environmentHeight;
    Renderer envRenderer;
    Collider envCollider;
    Collider playAreaCollider;
    Mesh envMesh;
    Vector2 envSize;
    public RenderTexture rt;

    // Start is called before the first frame update
    void Start()
    {
        envMesh = environment.GetComponent<MeshFilter>().mesh;
        envRenderer = environment.GetComponent<Renderer>();
        envCollider = environment.GetComponent<Collider>();
        envRenderer.material = snowMaterial;

        rt = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        envRenderer.material.SetTexture("_SnowHeight", rt);
        playAreaCollider = playArea.GetComponent<Collider>();
        envSize = new Vector2(playAreaCollider.bounds.size.x, playAreaCollider.bounds.size.z);
    }

    // Update is called once per frame
    void Update()
    {
        //envSize = new Vector2(envCollider.bounds.size.x, envCollider.bounds.size.z);
        // ^ if i can inversely scale the previous texture onto the new texture...later

        environment.GetComponent<Renderer>().material.SetVector("_PlayArea", new Vector4(playAreaCollider.transform.position.x, playAreaCollider.transform.position.z, envSize.x, envSize.y));

        // i am in world space, i need to know my position with the environment's min point as the origin
        // and max point as resolution

        //float relativeX = Util.Map(transform.position.x, envCollider.bounds.min.x, envCollider.bounds.max.x, 0, 1024);
        //float relativeZ = Util.Map(transform.position.z, envCollider.bounds.min.z, envCollider.bounds.max.z, 0, 1024);
        float relativeX = Util.Map(transform.position.x, playAreaCollider.bounds.min.x, playAreaCollider.bounds.max.x, 0, 1024);
        float relativeZ = Util.Map(transform.position.z, playAreaCollider.bounds.min.z, playAreaCollider.bounds.max.z, 0, 1024);
        Debug.Log("env size: " + envCollider.bounds.size);
        Debug.Log("relative position: " + relativeX + ", " + relativeZ);
        RenderTexture.active = rt;
        GL.PushMatrix();                      
        GL.LoadPixelMatrix(0, resolution, resolution, 0);     
        Graphics.DrawTexture(new Rect(relativeX, resolution - relativeZ, falloffRadius, falloffRadius), brushTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }
}

public class Util
{
    public static float Map(float x, float min1, float max1, float min2, float max2)
    {
        return (x - min1) / (max1 - min1) * (max2 - min2) + min2;
    }
}
