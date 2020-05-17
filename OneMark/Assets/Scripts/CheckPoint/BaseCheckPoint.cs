using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CheckPointのベースとなるBaseCheckPoint
/// </summary>
public abstract class BaseCheckPoint : MonoBehaviour
{
	/// <summary>Instance counter(static)</summary>
	static int m_instanceIDCounter = 0;

	/// <summary>Instance id (mark point)</summary>
	public int pointInstanceID { get; private set; } = -1;

	/// <summary>リンクしているPlayerのID</summary>
	public int linkPlayerID { get { return m_drawingLinkPlayerID; } private set { m_drawingLinkPlayerID = value; } }
	/// <summary>リンクしている？</summary>
	public bool isLinked { get { return linkPlayerID != -1; } }

	/// <summary>リンクしているPlayerのID</summary>
	[SerializeField, Tooltip("当たり判定の半径")]
	float m_collisionRadius = 0.5f;
	/// <summary>当たり判定を行う間隔</summary>
	[SerializeField, Tooltip("当たり判定を行う間隔")]
	float m_collisionJudgementInterval = 0.1f;
	/// <summary>リンクしているPlayerのID</summary>
	[SerializeField, Tooltip("リンクしているPlayerのID")]
	int m_drawingLinkPlayerID = -1;

	/// <summary>Interval timer</summary>
	Timer m_intervalTimer = new Timer();

	/// <summary>
	/// [UpdatePoint] (Virtual)
	/// ポイントの更新を行う
	/// </summary>
	public abstract void UpdatePoint();


	/// <summary>
	/// [LinkPlayer]
	/// プレイヤーとの紐付けを行う
	/// 引数1: 紐付けるPlayerのInstance ID
	/// </summary>
	public virtual void LinkPlayer(int playerInstanceID)
	{
		linkPlayerID = playerInstanceID;
	}
	/// <summary>
	/// [UnlinkPlayer]
	/// プレイヤーとの紐付け解除を行う
	/// </summary>
	public virtual void UnlinkPlayer()
	{
		linkPlayerID = -1;
	}

	/// <summary>
	/// [UpdateBasePoint]
	/// BasePointの更新を行う
	/// </summary>
	public void UpdateBasePoint()
	{
		//処理インターバル判断
		if (m_intervalTimer.elapasedTime < m_collisionJudgementInterval)
			return;

		//処理負荷軽減
		Vector3 position = transform.position;
		//ヒットしたPlayerのID
		int hitPlayer = -1;
		//Playerにヒットした回数
		int hitCount = 0;

		//当たり判定ループ
		foreach (var e in PlayerAndTerritoryManager.instance.allPlayers)
		{
			//当たったらID保存、カウンタインクリメント
			if (CollisionTerritory.HitCircleTerritory(e.Value.territorialArea, position, Vector3.forward, m_collisionRadius))
			{
				hitPlayer = e.Value.instanceID;
				++hitCount;
			}
		}

		//ヒットしていなかったら解除
		if (hitCount == 0 && isLinked)
			UnlinkPlayer();
		//ヒットしていたら登録
		else if(hitCount == 1 && !isLinked)
			LinkPlayer(hitPlayer);
		//ないだろうけど上書きされたら解除->登録
		else if (linkPlayerID != hitPlayer)
		{
			UnlinkPlayer();
			LinkPlayer(hitPlayer);
		}

		//タイマー再スタート
		m_intervalTimer.Start();
	}

	/// <summary>[Awake]</summary>
	void Awake()
	{
		pointInstanceID = m_instanceIDCounter++;

		CheckPointManager.instance.AddCheckPoint(this);

		m_intervalTimer.Start();
	}
	/// <summary>[OnDestroy]</summary>
	void OnDestroy()
	{
		if (CheckPointManager.instance != null)
			CheckPointManager.instance.RemoveCheckPoint(this);
	}
}
