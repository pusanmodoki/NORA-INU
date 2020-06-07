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
    [Tooltip("追従するオブジェクト")]
    private GameObject followingObject = null;

    [Tooltip("注視するオブジェクト")]
    private GameObject lookObject = null;

    [Tooltip("プレイヤー")]
    private GameObject playerObject = null;

    [Tooltip("スタート判定")]
    public bool startFlg = false;

    [SerializeField, Tooltip("スタートの時にカメラが動くスピード")]
    private float startCameraSpeed = 0.03f;

    [SerializeField, Tooltip("スタートの時にカメラが動き出す時間")]
    private float followObjectMoveTime = 5.0f;

    [Tooltip("リザルト判定")]
    public bool resultFlg = false;

    [SerializeField, Tooltip("リザルトの時にカメラが動くスピード")]
    private float resultCameraSpeed = 0.03f;

    [SerializeField, Tooltip("リザルトの時にカメラが動き出す時間")]
    private float lookPointMoveTime = 5.0f;

    [SerializeField]
    PlayerCovering m_covering = null;

    [SerializeField]
    Animator m_uiAnimator = null;

    // Start is called before the first frame update
    void Start()
    {
		playerObject = PlayerAndTerritoryManager.instance.mainPlayer.gameObject;
		lookObject = PlayerAndTerritoryManager.instance.mainPlayer.resultCameraLookPoint;
		followingObject = PlayerAndTerritoryManager.instance.mainPlayer.resultCameraMovePoint;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (startFlg == true) 
        {
            followingObject.transform.localPosition = new Vector3(0.0f, 1.0f, 4.0f);

            this.transform.position =
                Vector3.Slerp(this.transform.position, followingObject.transform.position, startCameraSpeed);

            this.transform.LookAt(lookObject.transform.position - new Vector3(0.0f, 0.0f, 0.0f));

            Invoke("MoveCameraPoint", followObjectMoveTime);
        }

        // リザルト
        if (resultFlg == true) 
        {
            // プレイヤーの向き
            playerObject.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

            followingObject.transform.localPosition = new Vector3(0.0f, 1.5f, 4.0f);

            // カメラポイントオブジェクトについていく
            this.transform.position =
            Vector3.Slerp(this.transform.position, followingObject.transform.position, resultCameraSpeed);

            // プレイヤーを見る
            this.transform.LookAt(lookObject.transform);

            Invoke("MoveLookPoint", lookPointMoveTime);
        }
    }

    public void StartFlg()
    {
        startFlg = true;
        resultFlg = false;
    }

    public void ResultFlg()
    {

        m_covering.isMoving = false;
        resultFlg = true;
    }

    private void MoveCameraPoint()
    {
        //followingObject.transform.localPosition =
        //    Vector3.Lerp(followingObject.transform.localPosition, new Vector3(0.0f, 33.0f, -13.0f), startCameraSpeed);

        //this.transform.position =
        //        Vector3.Slerp(this.transform.position, followingObject.transform.position, startCameraSpeed);

        //this.transform.LookAt(lookObject.transform);
        m_covering.isMoving = true;
        startFlg = false;
        //playerObject.GetComponent<PlayerInput>().isEnableInput = true;
        m_uiAnimator.SetTrigger("SetPosition");
    }

    private void MoveLookPoint()
    {
        lookObject.transform.localPosition =
            Vector3.Lerp(lookObject.transform.localPosition, new Vector3(-2.0f, 1.5f, 0.0f), resultCameraSpeed);
    }

}
