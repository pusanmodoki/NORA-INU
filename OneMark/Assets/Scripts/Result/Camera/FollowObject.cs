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
	public bool isCompleteStartMoveEnter { get; private set; } = false;
	public bool isCompleteResultMoveEnter { get; private set; } = false;

	[Tooltip("追従するオブジェクト")]
    private GameObject followingObject = null;

    [Tooltip("注視するオブジェクト")]
    private GameObject lookObject = null;

    [Tooltip("プレイヤー")]
    private GameObject playerObject = null;

    [Tooltip("スタート判定")]
    public bool startFlg = false;
	
	[SerializeField]
	float m_startCameraMoveSeconds = 0.5f;
	[SerializeField]
	float m_startCameraWaitSeconds = 2.5f;

	[Tooltip("リザルト判定")]
    public bool resultFlg = false;

	[SerializeField]
	float m_resultCameraMoveSeconds = 0.5f;
	[SerializeField]
	float m_resultCameraSecondMoveSeconds = 0.5f;

	[SerializeField]
    MainCameraStartSet m_cameraSrart = null;

	Timer m_moveTimer = new Timer();
	Vector3 m_moveStartPosition = Vector3.zero;
	Animator m_uiAnimator = null;
	int m_state = 0;

    // Start is called before the first frame update
    void Start()
    {
		playerObject = PlayerAndTerritoryManager.instance.mainPlayer.gameObject;
		lookObject = PlayerAndTerritoryManager.instance.mainPlayer.resultCameraLookPoint;
		followingObject = PlayerAndTerritoryManager.instance.mainPlayer.resultCameraMovePoint;
		m_uiAnimator = GameObject.Find("StageName").GetComponent<Animator>();

		StartFlg();
    }

    // Update is called once per frame
    void LateUpdate()
    {
		if (isCompleteStartMoveEnter) isCompleteStartMoveEnter = false;
		if (isCompleteResultMoveEnter) isCompleteResultMoveEnter = false;

		if (startFlg == true) 
        {
			if (m_state == 0)
			{
				followingObject.transform.localPosition = new Vector3(0.0f, 1.0f, 4.0f);

				transform.position = Vector3.Lerp(m_moveStartPosition, followingObject.transform.position,
					m_moveTimer.elapasedTime / m_startCameraMoveSeconds);

				transform.LookAt(lookObject.transform.position);

				if (m_moveTimer.elapasedTime >= m_startCameraMoveSeconds)
				{
					transform.position = followingObject.transform.position;
					m_state = 1;
					m_moveTimer.Start();
				}
			}
			else if (m_state == 1)
			{
				if (m_moveTimer.elapasedTime >= m_startCameraWaitSeconds)
				{
					m_cameraSrart.StartMove();
					isCompleteStartMoveEnter = true;
					MoveCameraPoint();
					m_moveTimer.Stop();
				}
			}
		}

        // リザルト
        if (resultFlg == true) 
        {
			if (m_state == 0)
			{
				// プレイヤーの向き
				playerObject.transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);

				followingObject.transform.localPosition = new Vector3(0.0f, 1.5f, 4.0f);

				// カメラポイントオブジェクトについていく
				transform.position = Vector3.Lerp(m_moveStartPosition, followingObject.transform.position,
					m_moveTimer.elapasedTime / m_resultCameraMoveSeconds);

				// プレイヤーを見る
				transform.LookAt(lookObject.transform);

				if (m_moveTimer.elapasedTime >= m_resultCameraMoveSeconds)
				{
					lookObject.transform.localPosition = new Vector3(-2.0f, 1.5f, 0.0f);
					m_moveStartPosition = transform.position;

					m_state = 1;
					//MoveLookPoint();
					m_moveTimer.Start();
				}
			}
			else if (m_state == 1)
			{
				transform.position = Vector3.Lerp(m_moveStartPosition, lookObject.transform.position,
					m_moveTimer.elapasedTime / m_resultCameraSecondMoveSeconds);
				// プレイヤーを見る
				transform.LookAt(lookObject.transform.position);

				if (m_moveTimer.elapasedTime >= m_resultCameraSecondMoveSeconds)
				{
					isCompleteResultMoveEnter = true;
					//MoveLookPoint();
					m_moveTimer.Stop();
				}
			}
		}
	}

    public void StartFlg()
    {
        startFlg = true;
        resultFlg = false;
		m_state = 0;
		m_moveStartPosition = transform.position;
		//m_covering.isMoving = false;

		m_moveTimer.Start();
	}

	public void ResultFlg()
    {
        //m_covering.isMoving = false;
        resultFlg = true;
		startFlg = false;
		m_state = 0;
		m_moveStartPosition = transform.position;

		m_moveTimer.Start();
	}

	private void MoveCameraPoint()
    {
	//	followingObject.transform.localPosition = new Vector3(0.0f, 33.0f, -13.0f);
//			Vector3.Lerp(followingObject.transform.localPosition, new Vector3(0.0f, 33.0f, -13.0f), startCameraSpeed);

		//this.transform.position =
		//        Vector3.Slerp(this.transform.position, followingObject.transform.position, startCameraSpeed);

		//this.transform.LookAt(lookObject.transform);
		//m_covering.isMoving = true;
		startFlg = false;
        //playerObject.GetComponent<PlayerInput>().isEnableInput = true;
        m_uiAnimator.SetTrigger("SetPosition");
    }

	//void MoveLookPoint()
	//{
	//	lookObject.transform.localPosition = new Vector3(-2.0f, 1.5f, 0.0f);
	//		//Vector3.Lerp(lookObject.transform.localPosition, , resultCameraSpeed);
	//}

}
