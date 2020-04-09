using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MarkPointを管理するMarkPointManager
/// </summary>
[DefaultExecutionOrder(-100)]
public class MarkPointManager : MonoBehaviour
{
	/// <summary>Static instance</summary>
	public static MarkPointManager instance { get; private set; } = null;

	/// <summary>Manage dogs</summary>
	Dictionary<int, BaseMarkPoint> m_points = new Dictionary<int, BaseMarkPoint>();

	/// <summary>[Awake]</summary>
	void Awake()
	{
		instance = this;
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
	public void AddMarkPoint(BaseMarkPoint dogAgent)
	{
		m_points.Add(dogAgent.pointInstanceID, dogAgent);
	}
	/// <summary>
	/// [RemoveMarkPoint]
	/// BaseMarkPointを登録解除する
	/// 引数1: BaseMarkPoint
	/// </summary>
	public void RemoveMarkPoint(BaseMarkPoint dogAgent)
	{
		m_points.Remove(dogAgent.pointInstanceID);
	}

	/// <summary>
	/// [GetMarkPoint]
	/// BaseMarkPointを取得する
	/// 引数1: BaseMarkPoint.pointInstanceID
	/// </summary>
	public BaseMarkPoint GetMarkPoint(int instanceID)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_points.ContainsKey(instanceID))
		{
			Debug.LogError("Error!! ServantManager->GetServant\n ContainsKey(instanceID) == false");

			if (m_points.Count > 0)
			{
				var iterator = m_points.GetEnumerator();
				iterator.MoveNext();
				return iterator.Current.Value;
			}
			else return null;
		}
#endif

		return m_points[instanceID];
	}
}
