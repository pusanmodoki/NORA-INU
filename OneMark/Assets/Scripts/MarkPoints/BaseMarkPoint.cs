﻿using System.Collections;
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
	/// <summary>有効化カウンター</summary>
	public float effectiveCounter { get; protected set; } = 0.0f;
	/// <summary>有効化カウンター (0 ~ 1)</summary>
	public float effectiveCounter01 { get { return effectiveCounter / effectiveMaxLimiter; } }
	/// <summary>マーキングdelta count (上昇)</summary>
	public float acendingDeltaCount { get { return m_linkAscendingPerSeconds * Time.deltaTime * MarkPointManager.instance.countScale; } }
	/// <summary>マーキングdelta count (減少)</summary>
	public float decreasingDeltaCount { get { return m_unlinkDecreasingPerSeconds * Time.deltaTime * MarkPointManager.instance.countScale; } }
	/// <summary>マーキング最大時間</summary>
	public float effectiveMaxLimiter { get { return m_effectiveMaxLimiter; } }
	/// <summary>初回リンク時のカウンターボーナス</summary>
	public float effectiveFirstLinkBonus { get { return m_effectiveMaxLimiter * m_effectiveFirstLinkBonusRatio; } }
	/// <summary>リンクしているPlayerのID</summary>
	public int linkPlayerID { get { return m_drawingLinkPlayerID; } private set { m_drawingLinkPlayerID = value; } }
	/// <summary>リンクしているServantのID</summary>
	public int linkServantID { get { return m_drawingLinkServantID; } private set { m_drawingLinkServantID = value; } }
	/// <summary>リンクしている？</summary>
	public bool isLinked { get { return linkPlayerID != -1; } }
	/// <summary>Playerが近くにいる？</summary>
	public bool isPlayerNearby { get; private set; } = false;
	/// <summary>初期ポイントとしてタイマーロック中？</summary>
	public bool isLockFirstPoint { get; private set; } = false;
	/// <summary>強制カウンター増加状態？</summary>
	public bool isForceAscendingEffective { get; private set; } = false;
	/// <summary>リンク中Safetyエリアに加えるか？</summary>
	public bool isJoinSafetyAreaWhenLink { get { return m_isJoinSafetyAreaWhenLink; } }

	//Debug only
#if UNITY_EDITOR
	[Header("Debug Only"), SerializeField]
	float m_dEffectiveCounter = 0.0f;
	[SerializeField]
	bool m_dIsPlayerNearby = false;
	[SerializeField]
	bool m_dIsLockFirstPoint = false;
#endif

	/// <summary>リンクしているPlayerのID</summary>
	[Space, SerializeField, Tooltip("リンクしているPlayerのID")]
	int m_drawingLinkPlayerID = -1;
	/// <summary>リンクしているServantのID</summary>
	[SerializeField, Tooltip("リンクしているServantのID")]
	int m_drawingLinkServantID = -1;

    [Header("Values"), SerializeField, Space, Space]
    Animator m_selectAnimation = null;
	/// <summary>リンク時のゲージ上昇速度 per seconds</summary>
	[SerializeField, Tooltip("リンク時のゲージ上昇速度 per seconds")]
	float m_linkAscendingPerSeconds = 1.0f;
	/// <summary>リンク解消時のゲージ減少速度 per seconds</summary>
	[SerializeField, Tooltip("リンク解消時のゲージ減少速度 per seconds")]
	float m_unlinkDecreasingPerSeconds = 1.0f;
	/// <summary>マーキング最大時間</summary>
	[SerializeField, Tooltip("マーキング最大時間")]
	float m_effectiveMaxLimiter = 7.5f;
	/// <summary>初回リンク時のカウンターボーナス割合 (0.0f ~ 1.0f)</summary>
	[SerializeField, Range(0.0f, 1.0f), Tooltip("初回リンク時のカウンターボーナス割合 (0.0f ~ 1.0f)")]
	float m_effectiveFirstLinkBonusRatio = 0.5f;
	/// <summary>リンク中Safetyエリアに加えるか？</summary>
	[SerializeField, Tooltip("リンク中Safetyエリアに加えるか？"), Space]
	bool m_isJoinSafetyAreaWhenLink = true;

	/// <summary>
	/// [LinkPoint] (Virtual)
	/// ポイントがリンクされた際にコールバックされる関数
	/// </summary>
	public virtual void LinkPoint() { }
	/// <summary>
	/// [UnlinkPoint] (Virtual)
	/// ポイントがリンク解除された際にコールバックされる関数
	/// </summary>
	public virtual void UnlinkPoint() { }
	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public abstract void UpdatePoint();

	/// <summary>
	/// [AddFirstLinkBonus]
	/// カウンタにAddFirstLinkBonus加算
	/// </summary>
	public void AddFirstLinkBonus() { effectiveCounter += effectiveFirstLinkBonus; }
	/// <summary>
	/// [SetPlayerNearby]
	/// Set PlayerNearby
	/// </summary>
	public void SetPlayerNearby(bool isSet) { isPlayerNearby = isSet; }
	/// <summary>
	/// [SetForceAscendingEffective]
	/// Set ForceAscendingEffective
	/// </summary>
	public void SetForceAscendingEffective(bool isSet) { isForceAscendingEffective = isSet; }
	/// <summary>
	/// [SetLockFirstPoint]
	/// Set LockFirstPoint
	/// </summary>
	public void SetLockFirstPoint(bool isSet)
	{
		if ((isLockFirstPoint ^ isSet) & isSet)
			effectiveCounter = effectiveMaxLimiter;

		isLockFirstPoint = isSet;
	}



	/// <summary>
	/// [ChangeAgent]
	/// DogAgentとの紐付けを更新する
	/// 引数1: 紐付けるDogAgent
	/// </summary>
	public void ChangeAgent(DogAIAgent dogAIAgent)
	{
		if (isLinked)
			linkServantID = dogAIAgent != null ? dogAIAgent.aiAgentInstanceID : -1;
	}
	/// <summary>
	/// [LinkPlayer]
	/// プレイヤーとの紐付けを行う
	/// 引数1: 紐付けるPlayer
	/// 引数2: 紐付けるDogAgent
	/// </summary>
	public void LinkPlayer(GameObject player, DogAIAgent dogAIAgent)
	{
		if (linkPlayerID == player.GetInstanceID())
			return;

		bool isOldLinked = isLinked;
		//ID登録
		linkPlayerID = player.GetInstanceID();
		linkServantID = dogAIAgent != null ? dogAIAgent.aiAgentInstanceID : -1;
		//Managerに紐付け登録
		if (!isOldLinked)
			PlayerAndTerritoryManager.instance.allPlayers[linkPlayerID].AddMarkPoint(this);
		
		//Callback
		LinkPoint();
	}
	/// <summary>
	/// [UnlinkPlayer]
	/// プレイヤーとの紐付け解除を行う
	/// </summary>
	public void UnlinkPlayer()
	{
		//Managerの紐付け登録解除
		PlayerAndTerritoryManager.instance.allPlayers[linkPlayerID].RemoveMarkPoint(this);
		if (linkServantID != -1)
			ServantManager.instance.GetServant(linkServantID).SetSitAndStay(false, this);

		//ID登録解除
		linkPlayerID = -1;
		linkServantID = -1;
		//Callback
		UnlinkPoint();
	}
	/// <summary>
	/// [UpdateBasePoint]
	/// BasePointの更新を行う
	/// </summary>
	public void UpdateBasePoint()
	{
		//Debug only
#if UNITY_EDITOR
		m_dEffectiveCounter = effectiveCounter;
		m_dIsPlayerNearby = isPlayerNearby;
		m_dIsLockFirstPoint = isLockFirstPoint;
#endif

		if (isPlayerNearby | isLockFirstPoint | isForceAscendingEffective)
			effectiveCounter += acendingDeltaCount;
		else
			effectiveCounter -= decreasingDeltaCount;

		effectiveCounter = Mathf.Clamp(effectiveCounter, 0.0f, effectiveMaxLimiter);

		if (isLinked && effectiveCounter <= 0.0f)
			UnlinkPlayer();
	}

    public GameObject SelectThisPoint()
    {
        m_selectAnimation.SetInteger("State", 1);
        return gameObject;
    }

    public void RemovedThisPoint()
    {
        m_selectAnimation.SetInteger("State", 0);
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
