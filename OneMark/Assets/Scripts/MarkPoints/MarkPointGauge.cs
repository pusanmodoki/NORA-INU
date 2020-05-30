using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkPointGauge : MonoBehaviour
{
    [SerializeField]
    MeshFilter m_mesh = null;
    [SerializeField]
    MeshRenderer m_renderer = null;
    [SerializeField]
    BaseMarkPoint m_markPoint = null;


    Material m_material = null;

    public float m_maxHeight { get; private set; } = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        List<Vector3> list = new List<Vector3>();
        m_mesh.sharedMesh.GetVertices(list);
        m_material = m_renderer.material;

        for (int i = 0; i < m_mesh.mesh.vertexCount; ++i)
        {
            if(list[i].y > m_maxHeight)
            {
                m_maxHeight = list[i].y;
            }
        }

        m_material.SetFloat("_Height", m_maxHeight);
    }

    private void Update()
    {
        m_material.SetFloat("_Gauge", m_markPoint.effectiveCounter01);

    }
}
