using UnityEngine;
using System.Collections;

public class IVRPostEffect : MonoBehaviour
{
    [HideInInspector]
    public Vector2 Scale;

    [HideInInspector]
    public Vector2 ScaleIn;

    [HideInInspector]
    public Vector2 Center = new Vector2(0.5f, 0.5f);

    [HideInInspector]
    public float K0, K1, K2;


    public Material material;


    // OnRenderImage
    void OnRenderImage(RenderTexture SourceTexture, RenderTexture DestTexture)
    {
        if (material != null)
        {
            // Render with distortion
            InitPortraitProperties(ref material);
            Graphics.Blit(SourceTexture, DestTexture, material);
        }
        else
        {
            // Pass through
            Graphics.Blit(SourceTexture, DestTexture);
        }
    }

    private void InitPortraitProperties(ref Material material)
    {
        material.SetVector("_Center", Center);
        material.SetVector("_Scale", Scale);
        material.SetVector("_ScaleIn", ScaleIn);
        material.SetVector("_HmdWarpParam", new Vector4(K0, K1, K2, 0));
    }
}
