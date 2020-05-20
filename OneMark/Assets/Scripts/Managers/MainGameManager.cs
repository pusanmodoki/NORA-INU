using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class MainGameManager : MonoBehaviour
{
	enum ResultState
	{
		Null,
		GameClear,
		GameEnd
	}

	/// <summary>Static instance</summary>
	public static MainGameManager instance { get; private set; } = null;

	//[SerializeField, Tooltip("とりあえず")]
	//GameObject m_clearBoard = null;
	//[SerializeField, Tooltip("とりあえず")]
	//GameObject m_overBoard = null;

	ReadOnlyDictionary<int, BaseCheckPoint> m_allCheckPoints = null;
	bool m_isEnd = false;

    [SerializeField]
    private Vector2 m_stageSize = new Vector3(30.0f, 30.0f);
	[SerializeField]
	float m_waitResultClearSeconds = 2.0f;
	[SerializeField]
	float m_waitResultOverSeconds = 1.0f;

	public Vector2 stageSize { get { return m_stageSize; } private set { m_stageSize = value; } }

    private FollowObject m_mainCamera = null;

	ResultState m_resultState = ResultState.Null;
	Timer m_resultTimer = new Timer();
	bool isGameOver = false;

    /// <summary>[Awake]</summary>
    void Awake()
	{
		//インスタンス登録
		instance = this;

		//m_mainCamera.transform.position = Vector3.zero;
		//m_mainCamera.StartFlg();

		//m_clearBoard.SetActive(false);
		//m_overBoard.SetActive(false);
	}

	/// <summary>[Start]</summary>
	void Start()
	{
		m_allCheckPoints = CheckPointManager.instance.allPoints;
        m_mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowObject>();
		Debug.Log(m_mainCamera != null);
	}

	/// <summary>[LateUpdate]</summary>
	void LateUpdate()
	{
		if (m_isEnd) return;

		if (m_resultState == ResultState.Null)
		{
			int checkCounter = 0;

			foreach (var e in m_allCheckPoints)
				if (e.Value.isLinked) ++checkCounter;

			if (m_allCheckPoints.Count == checkCounter)
			{
				m_resultState = ResultState.GameClear;
				m_resultTimer.Start();
			}
			if (PlayerAndTerritoryManager.instance.mainPlayer.maualCollisionAdministrator.isTerritoryExit)
			{
				m_resultState = ResultState.GameEnd;
				m_resultTimer.Start();
			}
		}
		else if (m_resultState == ResultState.GameClear)
		{
			if (m_resultTimer.elapasedTime > m_waitResultClearSeconds)
				GameClear();
		}
		else if (m_resultState == ResultState.GameEnd)
		{
			if (m_resultTimer.elapasedTime > m_waitResultOverSeconds)
				GameOver();
		}
	}

	void GameClear()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.input.GameClearAnimation();
		AudioManager.instance.StopBgm(AudioManager.instance.bgmForNowScene.loadBgmKeys[0]);
		MarkPointManager.instance.SetCountScale(0.0f);
        ResultCall.GameClear();
        m_mainCamera.ResultFlg();
		m_isEnd = true;
	}

	void GameOver()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.input.GameOverAnimation();
		AudioManager.instance.StopBgm(AudioManager.instance.bgmForNowScene.loadBgmKeys[0]);
		MarkPointManager.instance.SetCountScale(0.0f);
        ResultCall.GameOver();
		m_isEnd = true;
	}
}
