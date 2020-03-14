//--------------------------------------------------------------
// CameraOperation.cs
// カメラ操作
// 作成者：綾野紗世
//--------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOperation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("追従するオブジェクト")]
    private GameObject followingObject;

    [SerializeField]
    [Tooltip("カメラを動かすスピード")]
    private float leaveSpeed;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ズームイン・ズームアウト
        this.transform.position =
        Vector3.Lerp(this.transform.position, followingObject.transform.position, leaveSpeed);
    }
} 
