using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class MainGameManager : MonoBehaviour
{
	/// <summary>Static instance</summary>
	public static MainGameManager instance { get; private set; } = null;

	//[SerializeField, Tooltip("とりあえず")]
	//GameObject m_clearBoard = null;
	//[SerializeField, Tooltip("とりあえず")]
	//GameObject m_overBoard = null;

	ReadOnlyDictionary<int, BaseCheckPoint> m_allCheckPoints = null;
	bool m_isEnd = false;

    /// <summary>[Awake]</summary>
    void Awake()
	{
		//インスタンス登録
		instance = this;

		//m_clearBoard.SetActive(false);
		//m_overBoard.SetActive(false);
	}

	/// <summary>[Start]</summary>
	void Start()
	{
		m_allCheckPoints = CheckPointManager.instance.allPoints;
	}

	/// <summary>[LateUpdate]</summary>
	void LateUpdate()
	{
		if (m_isEnd) return;

		int checkCounter = 0;

		foreach (var e in m_allCheckPoints)
			if (e.Value.isLinked) ++checkCounter;

		if (m_allCheckPoints.Count == checkCounter)
			GameClear();

		if (PlayerAndTerritoryManager.instance.mainPlayer.maualCollisionAdministrator.isTerritoryExit)
			GameOver();
	}

	void GameClear()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.gameObject.GetComponent<PlayerInput>().GameClearAnimation();
        ResultCall.GameClear();
		m_isEnd = true;
	}

	void GameOver()
	{
        PlayerAndTerritoryManager.instance.mainPlayer.gameObject.GetComponent<PlayerInput>().GameOverAnimation();
        ResultCall.GameOver();
		m_isEnd = true;
	}
}
