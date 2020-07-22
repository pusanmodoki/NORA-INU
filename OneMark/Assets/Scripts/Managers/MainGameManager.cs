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
	
	public ResultState resultState { get; private set; } = ResultState.Null;
	public Vector2 stageSize { get { return m_stageSize; } private set { m_stageSize = value; } }
	public float waitGameOverCount01 { get { return Mathf.Clamp01(m_gameOverWaitTimer.elapasedTime / m_waitGameOverSeconds); } }
	public bool isPauseStay { get; private set; } = false; 
	public bool isPauseEnter { get { return Time.frameCount == m_pauseEnterFrame; } }
	public bool isPauseExit { get { return Time.frameCount == m_pauseExitFrame; } }
	public bool isGameEnd { get; private set; } = false;
	public void SetPauseStayFalse() { isPauseStay = false; m_pauseExitFrame = Time.frameCount + 1; }

	[SerializeField]
    private Vector2 m_stageSize = new Vector3(30.0f, 30.0f);
	[SerializeField]
	float m_waitResultClearSeconds = 2.0f;
	[SerializeField]
	float m_waitResultOverSeconds = 1.0f;
	[SerializeField]
	float m_waitGameOverSeconds = 3.0f;

	ReadOnlyDictionary<int, BaseCheckPoint> m_allCheckPoints = null;
	
    FollowObject m_mainCamera = null;
	Timer m_gameOverWaitTimer = new Timer();
	Timer m_resultTimer = new Timer();
	int m_pauseEnterFrame = 0;
	int m_pauseExitFrame = 0;

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

	void Update()
	{
		
	}

	/// <summary>[LateUpdate]</summary>
	void LateUpdate()
	{
		bool isOldExit = isPauseExit;
		
		if (isGameEnd || isPauseStay)
			return;

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

			if (resultState == ResultState.Null && OneMarkSceneManager.instance.nowStageSceneIndex.x > 0
				&& !isOldExit && Input.GetButtonDown("ActionPause"))
			{
				isPauseStay = true;
				m_pauseEnterFrame = Time.frameCount + 1;
				OneMarkSceneManager.instance.SetActiveAccessoryScene("Pause", true);
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
			else if (waitGameOverCount01 >= 1.0f)
			{
				resultState = ResultState.GameEnd;
				PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableInput = false;
				MarkPointManager.instance.SetCountScale(0.0f);
				m_resultTimer.Start();
			}
		}
		else if (resultState == ResultState.GameClear)
		{
			if (!TutorialUIManager.instance.isOnTutorial && m_resultTimer.elapasedTime > m_waitResultClearSeconds)
			{
				MarkPointManager.instance.GameEnd();
				GameClear();
			}
		}
		else if (resultState == ResultState.GameEnd)
		{
			if (!TutorialUIManager.instance.isOnTutorial && m_resultTimer.elapasedTime > m_waitResultOverSeconds)
			{
				MarkPointManager.instance.GameEnd();
				GameOver();
			}
		}
	}

	void GameClear()
	{
		PlayerAndTerritoryManager.instance.mainPlayer.managerIntermediary.GameEnd(true);
		AudioManager.instance.FadeoutAndChangeBgm("MoveResult", "GameClear");
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameClear", true);
		// ResultCall.GameClear();
		m_mainCamera.ResultFlg();
		isGameEnd = true;

		DataManager.instance.saveData.UpdateNumClearStages();
		DataManager.instance.WriteSaveData();
	}

	void GameOver()
	{
		PlayerAndTerritoryManager.instance.mainPlayer.managerIntermediary.GameEnd(false);
		AudioManager.instance.FadeoutAndChangeBgm("MoveResult", "GameOver");
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameOver", true);
		m_mainCamera.ResultFlg();
		//  ResultCall.GameOver();
		isGameEnd = true;
	}
}
