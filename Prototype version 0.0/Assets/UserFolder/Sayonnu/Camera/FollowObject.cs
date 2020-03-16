//--------------------------------------------------------------
// FollowObject.cs
// カメラ操作
// 作成者：綾野紗世
//--------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("追従するオブジェクト")]
    private GameObject followingObject;

    [SerializeField]
    [Tooltip("注視するオブジェクト")]
    private GameObject lookObject;

    [SerializeField]
    [Tooltip("カメラが動くスピード")]
    private float leaveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーを見る
        this.transform.LookAt(lookObject.transform);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ズームポイントについていく
        this.transform.position =
        Vector3.Lerp(this.transform.position, followingObject.transform.position, leaveSpeed);

    }
} 
