﻿//--------------------------------------------------------------
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
    [Tooltip("プレイヤー")]
    private GameObject playerObject;

    [SerializeField]
    [Tooltip("カメラが動くスピード")]
    private float leaveSpeed;

    [Tooltip("リザルト判定")]
    public bool resultFlg = false;

    [SerializeField]
    [Tooltip("リザルト判定")]
    private float lookPointMoveTime = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (resultFlg == true) 
        {
            // プレイヤーの向き
            playerObject.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

            // ズームポイントについていく
            this.transform.position =
            Vector3.Slerp(this.transform.position, followingObject.transform.position, leaveSpeed);

            // プレイヤーを見る
            this.transform.LookAt(lookObject.transform);

            Invoke("MovePoint", lookPointMoveTime);
        }
    }

    public void ResultFlg()
    {
        resultFlg = true;
    }

    private void MovePoint()
    {
        lookObject.transform.position =
            Vector3.Lerp(lookObject.transform.position, new Vector3(2.0f, 1.5f, 0.0f), leaveSpeed);
    }

}