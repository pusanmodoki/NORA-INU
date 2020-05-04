using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    [SerializeField]
    private Material postEffect;

    private void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, postEffect);
    }
}
