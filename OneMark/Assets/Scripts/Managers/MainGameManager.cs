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

    [SerializeField]
    private FollowObject production;

    private AudioSource bgm;

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
        production.StartFlg();
        bgm = this.GetComponent<AudioSource>();
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
        bgm.Stop();
        PlayerAndTerritoryManager.instance.mainPlayer.gameObject.GetComponent<PlayerInput>().GameClearAnimation();
        ResultCall.GameClear();
        production.ResultFlg();
		m_isEnd = true;
	}

	void GameOver()
	{
        bgm.Stop();
        PlayerAndTerritoryManager.instance.mainPlayer.gameObject.GetComponent<PlayerInput>().GameOverAnimation();
        ResultCall.GameOver();
        production.ResultFlg();
        m_isEnd = true;
	}
}
