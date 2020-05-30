using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField]
    Transform m_camera = null;

    private void Update()
    {
        transform.LookAt(m_camera.position);
    }

}
