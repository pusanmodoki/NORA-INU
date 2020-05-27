using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovePoint : MonoBehaviour
{
    [SerializeField]
    protected Cover m_cover = null;

    [SerializeField]
    private bool m_isMoving = true;

    public bool isMoving { get { return m_isMoving; } set { m_isMoving = value; } }

    public virtual void MovePointUpdate(Vector3 _vec)
    {
        transform.position = m_cover.startPoint.position + _vec;
    }
}
