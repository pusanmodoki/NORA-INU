using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaualCollisionAdministrator : MonoBehaviour
{
	struct VisibilityContainer : System.IEquatable<VisibilityContainer>
	{
		public VisibilityContainer(BaseMarkPoint markPoint)
		{
			this.markPoint = markPoint;
			this.forwardToPositionDot = 0.0f;
			this.instanceID = markPoint != null ? markPoint.pointInstanceID : -1;
		}
		public void SetPoint(BaseMarkPoint markPoint)
		{
			this.markPoint = markPoint;
			this.instanceID = markPoint != null ? markPoint.pointInstanceID : -1;
		}
		public bool Equals(VisibilityContainer other)
		{
			return instanceID == other.instanceID;
		}

		public BaseMarkPoint markPoint;
		public float forwardToPositionDot;
		public int instanceID;
	}

	public Vector3 territoryShortestNormal { get { return m_territoryShortestNormal; } } 
	public bool isTerritoryStay { get; private set; } = false;
	public bool isTerritoryEnter { get; private set; } = false;
	public bool isTerritoryExit { get; private set; } = false;

	public BaseMarkPoint hitVisibilityMarkPoint { get; private set; } = null;
	public float visibilityAngle { get { return m_visibilityAngle; } }
	public float visibilityDistance { get { return m_visibilityDistance; } }
	public bool isVisibilityStay { get; private set; } = false;
	public bool isVisibilityEnter { get; private set; } = false;
	public bool isVisibilityExit { get; private set; } = false;

	//debug only
#if UNITY_EDITOR
	/// <summary>Draw gizmo mesh (debug only)</summary>
	public Mesh dGizmoMesh { get; set; } = null;
	/// <summary>Draw gizmo mesh iD (debug only)</summary>
	public int dMeshID { get; set; } = -1;
	/// <summary>Visibility is draw gizmos? (debug only)</summary>
	public bool dIsDrawGizmos { get { return m_dIsDrawGizmos; } set { m_dIsDrawGizmos = value; } }

	/// <summary>Visibility is draw gizmos? (debug only)</summary>
	[SerializeField, Header("Debug Only"), Tooltip("Visibility is draw gizmos? (debug only)")]
	bool m_dIsDrawGizmos = true;
#endif


	[Header("Always"), SerializeField, Tooltip("当たり判定のレイヤーマスク")]
	float m_judgmentInterval = 0.1f;

	[Header("Sphere Collision"), SerializeField, Tooltip("当たり判定の半径")]
	float m_collisionRadius = 0.5f;

	[Header("Visibility"), SerializeField, Tooltip("当たり判定のレイヤーマスク")]
	LayerMaskEx m_visibilityLayerMask = 0;
	/// <summary>Visibility Distance (forward)</summary>
	[SerializeField, Tooltip("Visibility distance (forward)")]
	float m_visibilityDistance = 10;
	/// <summary>Visibility angle</summary>
	[SerializeField, Range(0.0f, 180.0f), Tooltip("Visibility angle")]
	float m_visibilityAngle = 90;

	/// <summary>PlayerInfo</summary>
	PlayerAndTerritoryManager.PlayerInfo m_playerInfo = null;
	Vector3 m_territoryShortestNormal = Vector3.zero;
	/// <summary>Judgment timer</summary>
	Timer m_judgmentTimer = new Timer();
	/// <summary>Angle->radian->cos</summary>
	float m_angleToCosine = 0.0f;

	public void SetPlayerInfo(PlayerAndTerritoryManager.PlayerInfo info)
	{
		//Set Info
		if (m_playerInfo == null && info != null)
			m_playerInfo = info;
	}

	// Start is called before the first frame update
	void Start()
	{
		//Timer start
		m_judgmentTimer.Start();

		//視界角度を変換
		m_angleToCosine = Mathf.Cos(m_visibilityAngle * 0.5f * Mathf.Deg2Rad);
	}

	// Update is called once per frame
	public void Update()
	{
		if (m_judgmentTimer.elapasedTime < m_judgmentInterval)
			return;

		Vector3 position = transform.position;

		CheckTerritory(ref position);
		CheckVisibility(ref position);

		m_judgmentTimer.Start();
	}

	void CheckVisibility(ref Vector3 position)
	{
		Vector3 forward = transform.forward;
		VisibilityContainer container = new VisibilityContainer();
		List<VisibilityContainer> markPoints = new List<VisibilityContainer>();
		var collisions = Physics.OverlapSphere(position, m_visibilityDistance, m_visibilityLayerMask);

		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			var point = collisions[i].GetComponent<BaseMarkPoint>();

			if (point == null || !point.isLinked) continue;

			container.SetPoint(point);

			if (!markPoints.Contains(container))
			{
				container.forwardToPositionDot = Vector3.Dot(forward, (point.transform.position - position).normalized);
				markPoints.Add(container);
			}
		}

		for (int i = 0; i < markPoints.Count;)
		{
			if (markPoints[i].forwardToPositionDot > m_angleToCosine)
				++i;
			else
				markPoints.RemoveAt(i);
		}

		if (markPoints.Count == 0)
		{
			EditVisibilityHitFlag(false);
			return;
		}
		else if (markPoints.Count == 1)
		{ 
			hitVisibilityMarkPoint = markPoints[0].markPoint;
			EditVisibilityHitFlag(true);
			return;
		}

		float minDot = 10000.0f;
		int minIndex = -1;
		for (int i = 0, count = markPoints.Count; i < count; ++i)
		{
			if (minDot < markPoints[i].forwardToPositionDot)
			{
				minDot = markPoints[i].forwardToPositionDot;
				minIndex = i;
			}
		}

		hitVisibilityMarkPoint = markPoints[markPoints.Count - 1].markPoint;
		EditVisibilityHitFlag(true);
	}

	bool EditVisibilityHitFlag(bool isNewValue)
	{
		isVisibilityEnter = (isVisibilityStay ^ isNewValue) & isNewValue;
		isVisibilityExit = (isVisibilityStay ^ isNewValue) & isVisibilityStay;
		return isVisibilityStay = isNewValue;
	}

	void CheckTerritory(ref Vector3 position)
	{
		bool isResult = CollisionTerritory.HitCircleAndRayTerritory(
			m_playerInfo.territorialArea, position, out m_territoryShortestNormal, m_collisionRadius);

		isTerritoryEnter = (isTerritoryStay ^ isResult) & isResult;
		isTerritoryExit = (isTerritoryStay ^ isResult) & isTerritoryStay;
		isTerritoryStay = isResult;
	}
}
