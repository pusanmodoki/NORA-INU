using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class MainGameManager : MonoBehaviour
{
	public enum ResultState
	{
		Null,
		GameClear,
		GameOverWait,
		GameEnd
	}

	/// <summary>Static instance</summary>
	public static MainGameManager instance { get; private set; } = null;
	
	public Vector2 stageSize { get { return m_stageSize; } private set { m_stageSize = value; } }
	public ResultState resultState { get; private set; } = ResultState.Null;
	public float waitGamerOverCount01 { get { return Mathf.Clamp01(m_gameOverWaitTimer.elapasedTime / m_waitGameOverSeconds); } }

	[SerializeField]
    private Vector2 m_stageSize = new Vector3(30.0f, 30.0f);
	[SerializeField]
	float m_waitResultClearSeconds = 2.0f;
	[SerializeField]
	float m_waitResultOverSeconds = 1.0f;
	[SerializeField]
	float m_waitGameOverSeconds = 3.0f;

	ReadOnlyDictionary<int, BaseCheckPoint> m_allCheckPoints = null;
	bool m_isEnd = false;
	
    FollowObject m_mainCamera = null;
	Timer m_gameOverWaitTimer = new Timer();
	Timer m_resultTimer = new Timer();

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
	}

	/// <summary>[LateUpdate]</summary>
	void LateUpdate()
	{
		if (m_isEnd) return;
		
		if (resultState == ResultState.Null)
		{
			int checkCounter = 0;

			foreach (var e in m_allCheckPoints)
				if (e.Value.isLinked) ++checkCounter;

			if (m_allCheckPoints.Count == checkCounter)
			{
				resultState = ResultState.GameClear;
				PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableInput = false;
				MarkPointManager.instance.SetCountScale(0.0f);
				m_resultTimer.Start();
			}
			if (PlayerAndTerritoryManager.instance.mainPlayer.manualCollisionAdministrator.isTerritoryExit)
			{
				resultState = ResultState.GameOverWait;
				PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableActionInput = false;
				PlayerAndTerritoryManager.instance.mainPlayer.SetGameOverGracePeiod(true);
				m_gameOverWaitTimer.Start();
			}
		}
		else if (resultState == ResultState.GameOverWait)
		{
			if (PlayerAndTerritoryManager.instance.mainPlayer.manualCollisionAdministrator.isTerritoryStay)
			{
				resultState = ResultState.Null;
				PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableActionInput = true;
				PlayerAndTerritoryManager.instance.mainPlayer.SetGameOverGracePeiod(false);
				m_gameOverWaitTimer.Stop();
			}
			else if (waitGamerOverCount01 >= 1.0f)
			{
				resultState = ResultState.GameEnd;
				PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableInput = false;
				MarkPointManager.instance.SetCountScale(0.0f);
				m_resultTimer.Start();
			}
		}
		else if (resultState == ResultState.GameClear)
		{
			if (m_resultTimer.elapasedTime > m_waitResultClearSeconds)
				GameClear();
		}
		else if (resultState == ResultState.GameEnd)
		{
			if (m_resultTimer.elapasedTime > m_waitResultOverSeconds)
				GameOver();
		}
	}

	void GameClear()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.input.GameClearAnimation();
		PlayerAndTerritoryManager.instance.mainPlayer.areaBorderMesh.meshRenderer.enabled = false;
		AudioManager.instance.FadeoutAndChangeBgm("MoveResult", "GameClear");
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameClear", true);
		// ResultCall.GameClear();
		m_mainCamera.ResultFlg();
		m_isEnd = true;
		
	}

	void GameOver()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.input.GameOverAnimation();
		PlayerAndTerritoryManager.instance.mainPlayer.areaBorderMesh.meshRenderer.enabled = false;
		AudioManager.instance.FadeoutAndChangeBgm("MoveResult", "GameOver");
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameOver", true);
		m_mainCamera.ResultFlg();
		//  ResultCall.GameOver();
		m_isEnd = true;
	}
}
