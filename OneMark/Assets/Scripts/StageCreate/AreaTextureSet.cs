using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTextureSet : MonoBehaviour
{
    [SerializeField]
    MeshRenderer m_renderer = null;

    // Start is called before the first frame update
    void Start()
    {
        if(NormalAreaTexture.texture)
            m_renderer.material.SetTexture("_AreaTex", NormalAreaTexture.texture);
        if (SafetyAreaTexture.texture)
            m_renderer.material.SetTexture("_SafetyAreaTex", SafetyAreaTexture.texture);
    }
}
