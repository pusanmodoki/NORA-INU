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
	public struct TemporarilyDeactiveInfo : IEqualityComparer<TemporarilyDeactiveInfo>
	{
		public TemporarilyDeactiveInfo(int pointInstanceID, float deacitiveSeconds)
		{
			this.pointInstanceID = pointInstanceID;
			this.deacitiveSeconds = deacitiveSeconds;
			this.timer = new Timer(); timer.Start();
		}

		public int pointInstanceID { get; private set; }
		public float deacitiveSeconds { get; private set; }
		public bool isDesignatedTimeElapsed { get { return timer.elapasedTime > deacitiveSeconds; } }
		Timer timer { get; set; }

		public bool Equals(TemporarilyDeactiveInfo x, TemporarilyDeactiveInfo y)
		{
			return x.pointInstanceID == y.pointInstanceID;
		}

		public int GetHashCode(TemporarilyDeactiveInfo obj)
		{
			return obj.GetHashCode();
		}
	}

	/// <summary>Static instance</summary>
	public static MarkPointManager instance { get; private set; } = null;

	/// <summary>All mark points</summary>
	public ReadOnlyDictionary<int, BaseMarkPoint> allPoints { get; private set; } = null;
	/// <summary>count scaler</summary>
	public float countScale { get; private set; } = 1.0f;

	/// <summary>Manage points</summary>
	Dictionary<int, BaseMarkPoint> m_points = null;
	/// <summary>非アクティブ化ポイントリスト</summary>
	List<TemporarilyDeactiveInfo> m_temporarilyDeactivePoints = new List<TemporarilyDeactiveInfo>();

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
		for (int i = 0; i < m_temporarilyDeactivePoints.Count; ++i)
		{
			if (m_temporarilyDeactivePoints[i].isDesignatedTimeElapsed)
			{
				allPoints[m_temporarilyDeactivePoints[i].pointInstanceID].SetDeacticve(false);
				m_temporarilyDeactivePoints.RemoveAt(i);
				--i;
			}
		}

		if (!MainGameManager.instance.isPauseStay && !TutorialUIManager.instance.isOnTutorial)
		{
			foreach (var e in m_points)
			{
				if (e.Value.gameObject.activeSelf)
				{
					e.Value.UpdateBasePoint();
					e.Value.UpdatePoint();
				}
			}
		}
	}

	/// <summary>
	/// [SetCountScale]
	/// Set the countScale
	/// </summary>
	public void SetCountScale(float set) { countScale = set; }
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

	/// <summary>
	/// [RegisterTemporarilyDeactive]
	/// BaseMarkPointを一時的に非アクティブ化する
	/// 引数1: BaseMarkPoint
	/// 引数2: 非アクティブ化秒数
	/// </summary>
	public void RegisterTemporarilyDeactive(BaseMarkPoint markPoint, float deactiveSeconds)
	{
		TemporarilyDeactiveInfo newInfo = new TemporarilyDeactiveInfo(markPoint.pointInstanceID, deactiveSeconds);

		if (!m_temporarilyDeactivePoints.Contains(newInfo))
			m_temporarilyDeactivePoints.Add(newInfo);

		markPoint.SetDeacticve(true);
	}
}
