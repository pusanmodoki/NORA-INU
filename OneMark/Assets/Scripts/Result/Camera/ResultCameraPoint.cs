using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultCameraPoint: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.LookAt(this.transform.parent);   // 親の方に向いている
    }

}
