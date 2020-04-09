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
	/// <summary>Manage dogs</summary>
	Dictionary<int, BaseMarkPoint> m_points = null;

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
