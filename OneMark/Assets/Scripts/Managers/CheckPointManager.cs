using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CheckPointManager : MonoBehaviour
{
	/// <summary>Static instance</summary>
	public static CheckPointManager instance { get; private set; } = null;

	/// <summary>Manage check points</summary>
	public ReadOnlyDictionary<int, BaseCheckPoint> allPoints { get; private set; } = null;

	/// <summary>Manage check points</summary>
	Dictionary<int, BaseCheckPoint> m_points = null;

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;
		m_points = new Dictionary<int, BaseCheckPoint>();
		allPoints = new ReadOnlyDictionary<int, BaseCheckPoint>(m_points);
	}
	/// <summary>[Update]</summary>
	void Update()
	{
		foreach(var e in m_points)
		{
			e.Value.UpdateBasePoint();
			e.Value.UpdatePoint();
		}
	}

	/// <summary>
	/// [AddCheckPoint]
	/// BaseCheckPointを登録する
	/// 引数1: BaseCheckPoint
	/// </summary>
	public void AddCheckPoint(BaseCheckPoint point)
	{
		m_points.Add(point.pointInstanceID, point);
	}
	/// <summary>
	/// [RemoveCheckPoint]
	/// BaseCheckPointを登録解除する
	/// 引数1: BaseCheckPoint
	/// </summary>
	public void RemoveCheckPoint(BaseCheckPoint point)
	{
		m_points.Remove(point.pointInstanceID);
	}
}
