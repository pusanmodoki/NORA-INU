using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshLinkInit : MonoBehaviour
{
    [SerializeField]
    private NavMeshLink m_top = null;

    [SerializeField]
    private NavMeshLink m_bottom = null;

    [SerializeField]
    private NavMeshLink m_right = null;

    [SerializeField]
    private NavMeshLink m_left = null;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 setPoint = Vector3.zero;
        setPoint.x = 0.0f;
        setPoint.y = 2.5f;
        setPoint.z = transform.localScale.z / 2.0f - 1.0f;
        m_top.startPoint = setPoint;
        setPoint.z *= -1.0f;
        m_bottom.startPoint = setPoint;

        setPoint.z = 0.0f;
        setPoint.x = transform.localScale.x / 2.0f - 1.0f;
        m_right.startPoint = setPoint;
        setPoint.x *= -1.0f;
        m_left.startPoint = setPoint;


        setPoint.x = 0.0f;
        setPoint.y = -2.0f;
        setPoint.z = transform.localScale.z / 2.0f + 3.0f;
        m_top.endPoint = setPoint;
        setPoint.z *= -1.0f;
        m_bottom.endPoint = setPoint;

        setPoint.z = 0.0f;
        setPoint.x = transform.localScale.x / 2.0f + 3.0f;
        m_right.endPoint = setPoint;
        setPoint.x *= -1.0f;
        m_left.endPoint = setPoint;


        m_bottom.width = transform.localScale.x;
        m_top.width = transform.localScale.x;
        m_right.width = transform.localScale.z;
        m_left.width = transform.localScale.z;

        m_bottom.UpdateLink();
        m_top.UpdateLink();
        m_right.UpdateLink();
        m_left.UpdateLink();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
