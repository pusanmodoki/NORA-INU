using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

/// <summary>
/// MarkPointを管理するMarkPointManager
/// </summary>
[DefaultExecutionOrder(-100)]
public class MarkPointManager : MonoBehaviour
{
	/// <summary>Static instance</summary>
	public static MarkPointManager instance { get; private set; } = null;

	/// <summary>All mark points</summary>
	public ReadOnlyDictionary<int, BaseMarkPoint> allPoints { get; private set; } = null;
	/// <summary>リンク時のゲージ上昇速度 per seconds</summary>
	public float linkAscendingPerSeconds { get { return m_linkAscendingPerSeconds; } }
	/// <summary>リンク解消時のゲージ減少速度 per seconds</summary>
	public float unlinkDecreasingPerSeconds { get { return m_unlinkDecreasingPerSeconds; } }
	/// <summary>初回リンク時のカウンターボーナス</summary>
	public float effectiveFirstLinkBonus { get { return m_effectiveMaxLimiter * m_effectiveFirstLinkBonusRatio; } }
	/// <summary>マーキング最大時間</summary>
	public float effectiveMaxLimiter { get { return m_effectiveMaxLimiter; } }

	/// <summary>Manage dogs</summary>
	Dictionary<int, BaseMarkPoint> m_points = null;

	/// <summary>リンク時のゲージ上昇速度 per seconds</summary>
	[SerializeField, Tooltip("リンク時のゲージ上昇速度 per seconds")]
	float m_linkAscendingPerSeconds = 1.0f;
	/// <summary>リンク解消時のゲージ減少速度 per seconds</summary>
	[SerializeField, Tooltip("リンク解消時のゲージ減少速度 per seconds")]
	float m_unlinkDecreasingPerSeconds = 1.0f;
	/// <summary>マーキング最大時間</summary>
	[SerializeField, Tooltip("マーキング最大時間")]
	float m_effectiveMaxLimiter = 7.5f;
	/// <summary>初回リンク時のカウンターボーナス割合</summary>
	[SerializeField, Range(0.0f, 1.0f), Tooltip("初回リンク時のカウンターボーナス割合")]
	float m_effectiveFirstLinkBonusRatio = 0.5f;

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;
		m_points = new Dictionary<int, BaseMarkPoint>();
		allPoints = new ReadOnlyDictionary<int, BaseMarkPoint>(m_points);
	}
	/// <summary>[OnDestroy]</summary>
	void OnDestroy()
	{
		instance = null;
	}
	/// <summary>[Update]</summary>
	void Update()
	{
		foreach (var e in m_points)
		{
			e.Value.UpdateBasePoint();
			e.Value.UpdatePoint();
		}
	}

	/// <summary>
	/// [AddMarkPoint]
	/// BaseMarkPointを登録する
	/// 引数1: BaseMarkPoint
	/// </summary>
	public void AddMarkPoint(BaseMarkPoint point)
	{
		m_points.Add(point.pointInstanceID, point);
	}
	/// <summary>
	/// [RemoveMarkPoint]
	/// BaseMarkPointを登録解除する
	/// 引数1: BaseMarkPoint
	/// </summary>
	public void RemoveMarkPoint(BaseMarkPoint point)
	{
		m_points.Remove(point.pointInstanceID);
	}
}
