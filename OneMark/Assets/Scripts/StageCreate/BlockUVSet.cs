using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUVSet : MonoBehaviour
{
    public static float s_stageSize { get; set; }

    private Mesh m_mesh = null;
    // Start is called before the first frame update
    void Start()
    {
        m_mesh = GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
