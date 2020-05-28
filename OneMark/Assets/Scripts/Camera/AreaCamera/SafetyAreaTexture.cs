using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyAreaTexture : MonoBehaviour
{
    [SerializeField]
    Texture m_texture = null;

    public static Texture texture { get; private set; }

    private void Awake()
    {
        texture = m_texture;
    }
}
