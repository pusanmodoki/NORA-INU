using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMovePoint : MonoBehaviour
{
    [SerializeField]
    protected Cover m_cover = null;


    public virtual void MovePointUpdate(Vector3 _vec)
    {
        transform.position = m_cover.startPoint.position + _vec;
    }
}
