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
	
	public void LinkPlayer(GameObject player, DogAIAgent dogAIAgent)
	{
		linkPlayerID = player.GetInstanceID();
		linkServantID = dogAIAgent != null ? dogAIAgent.aiAgentInstanceID : -1;
		m_timer.Start();

		PlayerAndTerritoryManager.instance.GetPlayer(linkPlayerID).AddMarkPoint(this);
	}
	public void UnlinkPlayer()
	{
		PlayerAndTerritoryManager.instance.GetPlayer(linkPlayerID).RemoveMarkPoint(this);
		if (linkServantID != -1)
			ServantManager.instance.GetServant(linkServantID).SetSitAndStay(false, this);

		m_timer.Stop();
		linkPlayerID = -1;
		linkServantID = -1;
	}
	public void ResetTimer()
	{
		m_timer.Start();
	}

	public void UpdateBasePoint()
	{
		if (isLinked && timeRemaining <= 0.0f)
			UnlinkPlayer();
	}

	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public abstract void UpdatePoint();

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
