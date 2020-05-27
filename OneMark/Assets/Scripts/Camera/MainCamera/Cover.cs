using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [SerializeField]
    Transform m_startPoint = null;

    [SerializeField]
    Transform m_endPoint = null;

    [SerializeField]
    BaseMovePoint m_movePoint = null;


    public BaseMovePoint movePoint { get { return m_movePoint; } set { m_movePoint = value; } }
    public Transform startPoint { get { return m_startPoint; } set { m_startPoint = value; } }
    public Transform endPoint { get { return m_endPoint; } set { m_endPoint = value; } }

    public float t { get; set; } = 0.0f;



    private void LateUpdate()
    {
        if (movePoint.isMoving)
        {
            Interpolation();
        }
    }

    private void Interpolation()
    {
        Vector3 vec = Vector3.zero;

        vec = endPoint.position - startPoint.position;
        vec *= t;
        m_movePoint.MovePointUpdate(vec);
    }
}
