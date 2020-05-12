using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUVSet : MonoBehaviour
{
    public static float s_stageSize { get; set; }

    Mesh m_mesh;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetFloat("_StageWidth", MainGameManager.instance.stageSize.x);
        GetComponent<MeshRenderer>().material.SetFloat("_StageHeight", MainGameManager.instance.stageSize.y);
    }


}
