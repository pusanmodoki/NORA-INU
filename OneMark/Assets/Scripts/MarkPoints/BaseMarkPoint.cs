using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MarkPointのベースとなるBaseMarkPoint 
/// </summary>
public abstract class BaseMarkPoint : MonoBehaviour
{
	/// <summary>Instance counter(static)</summary>
	static int m_instanceIDCounter = 0;
	
	/// <summary>Instance id (mark point)</summary>
	public int pointInstanceID { get; private set; } = -1;
	/// <summary>マーキング有効時間</summary>
	public float effectiveSeconds { get { return m_effectiveSeconds; } }
	/// <summary>マーキング有効な残り時間</summary>
	public float timeRemaining { get { return isLinked ? m_effectiveSeconds - m_timer.elapasedTime : 0.0f; } }
	/// <summary>リンクしているPlayerのID</summary>
	public int linkPlayerID { get { return m_drawingLinkPlayerID; } private set { m_drawingLinkPlayerID = value; } }
	/// <summary>リンクしているServantのID</summary>
	public int linkServantID { get { return m_drawingLinkServantID; } private set { m_drawingLinkServantID = value; } }
	/// <summary>リンクしている？</summary>
	public bool isLinked { get { return linkPlayerID != -1; } }
	/// <summary>残り時間計測タイマーのポーズ</summary>
	public bool isPauseTimer { get { return m_timer.isPause; } set { if (isLinked) { if (value) m_timer.Pause(); else m_timer.Unpause(); } } }

	/// <summary>マーキング有効時間</summary>
	[SerializeField, Tooltip("マーキング有効時間")]
	float m_effectiveSeconds = 5.0f;
	/// <summary>リンクしているPlayerのID</summary>
	[SerializeField, Tooltip("リンクしているPlayerのID")]
	int m_drawingLinkPlayerID = -1;
	/// <summary>リンクしているServantのID</summary>
	[SerializeField, Tooltip("リンクしているServantのID")]
	int m_drawingLinkServantID = -1;

	/// <summary>残り時間タイマー</summary>
	TimerAdvance m_timer = new TimerAdvance();


	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public abstract void UpdatePoint();


	/// <summary>
	/// [LinkPlayer]
	/// プレイヤーとの紐付けを行う
	/// 引数1: 紐付けるPlayer
	/// 引数2: 紐付けるDogAgent
	/// </summary>
	public void LinkPlayer(GameObject player, DogAIAgent dogAIAgent)
	{
		//ID登録
		linkPlayerID = player.GetInstanceID();
		linkServantID = dogAIAgent != null ? dogAIAgent.aiAgentInstanceID : -1;
		//Managerに紐付け登録
		PlayerAndTerritoryManager.instance.allPlayers[linkPlayerID].playerInfo.AddMarkPoint(this);

		//Timer計測開始
		m_timer.Start();
	}
	/// <summary>
	/// [UnlinkPlayer]
	/// プレイヤーとの紐付け解除を行う
	/// </summary>
	public void UnlinkPlayer()
	{
		//Managerの紐付け登録解除
		PlayerAndTerritoryManager.instance.allPlayers[linkPlayerID].playerInfo.RemoveMarkPoint(this);
		if (linkServantID != -1)
			ServantManager.instance.GetServant(linkServantID).SetSitAndStay(false, this);

		//ID登録解除
		linkPlayerID = -1;
		linkServantID = -1;
		//Timer計測停止
		m_timer.Stop();
	}

	/// <summary>
	/// [ResetTimer]
	/// Timerを初期化する
	/// </summary>
	public void ResetTimer()
	{
		m_timer.Start();
	}
	/// <summary>
	/// [UpdateBasePoint]
	/// BasePointの更新を行う
	/// </summary>
	public void UpdateBasePoint()
	{
		if (isLinked && timeRemaining <= 0.0f)
			UnlinkPlayer();
	}

	/// <summary>[Awake]</summary>
	void Awake()
	{
		pointInstanceID = ++m_instanceIDCounter;

		MarkPointManager.instance.AddMarkPoint(this);
	}
	/// <summary>[OnDestroy]</summary>
	void OnDestroy()
	{
		if (MarkPointManager.instance != null)
			MarkPointManager.instance.RemoveMarkPoint(this);
	}
}
