using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Playerの当たり判定管理を行うPlayerMaualCollisionAdministrator 
/// </summary>
public class PlayerMaualCollisionAdministrator : MonoBehaviour
{
	/// <summary>
	/// Visibility用に使用するVisibilityContainer
	/// </summary>
	struct VisibilityContainer : System.IEquatable<VisibilityContainer>
	{
		/// <summary>[コンストラクタ]</summary>
		public VisibilityContainer(BaseMarkPoint markPoint)
		{
			this.markPoint = markPoint;
			this.forwardToPositionDot = 0.0f;
			this.instanceID = markPoint != null ? markPoint.pointInstanceID : -1;
		}
		/// <summary>
		/// [SetPoint]
		/// Set MarkPoint
		/// </summary>
		public void SetPoint(BaseMarkPoint markPoint)
		{
			this.markPoint = markPoint;
			this.instanceID = markPoint != null ? markPoint.pointInstanceID : -1;
		}
		/// <summary>[Equals]</summary>
		public bool Equals(VisibilityContainer other)
		{
			return instanceID == other.instanceID;
		}

		/// <summary>Mark point</summary>
		public BaseMarkPoint markPoint;
		/// <summary>forward to position dot</summary>
		public float forwardToPositionDot;
		/// <summary>fMark point instance id</summary>
		public int instanceID;
	}

	public float collisionRadius { get { return m_collisionRadius; } }
	/// <summary>ポイントタイマーロック判定に使用する距離</summary>
	public float personalDistance { get { return m_personalRadius; } }
	/// <summary>視界判定に使用する距離</summary>
	public float visibilityDistance { get { return m_visibilityDistance; } }
	/// <summary>Visibility angle (get only)</summary>
	public float visibilityAngle { get { return m_visibilityAngle; } }
	/// <summary>視界判定に使用し, 本当にヒットするか判定する距離</summary>
	public float reallyVisibilityDistance { get { return m_reallyVisibilityDistance; } }
	/// <summary>視界判定時優先的にヒットさせる距離</summary>
	public float visibilityPreferentialDistance { get { return m_visibilityPreferentialDistance; } }

	/// <summary>Forward方向のテリトリー法線(β)</summary>
	public Vector3 territoryForwardSideNormal { get { return m_territoryForwardSideNormal; } }
	/// <summary>territory hit stay (Updateで値を更新)</summary>
	public bool isTerritoryStay { get { return m_isPublicFlags[0]; } private set { m_isPublicFlags[0] = value; } }
	/// <summary>Territory hit enter (Updateで値を更新)</summary>
	public bool isTerritoryEnter { get { return m_isPublicFlags[1]; } private set { m_isPublicFlags[1] = value; } }
	/// <summary>Territory hit exit (Updateで値を更新)</summary>
	public bool isTerritoryExit { get { return m_isPublicFlags[2]; } private set { m_isPublicFlags[2] = value; } }
	/// <summary>Territory segment x body only hit stay (Updateで値を更新)</summary>
	public bool isTerritorySegmentStay { get { return m_isPublicFlags[3]; } private set { m_isPublicFlags[3] = value; } }
	/// <summary>Territory segment x body only hit enter (Updateで値を更新)</summary>
	public bool isTerritorySegmentEnter { get { return m_isPublicFlags[4]; } private set { m_isPublicFlags[4] = value; } }
	/// <summary>Territory segment x body only hit exit (Updateで値を更新)</summary>
	public bool isTerritorySegmentExit { get { return m_isPublicFlags[5]; } private set { m_isPublicFlags[5] = value; } }

	/// <summary>territory hit stay (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritoryStay { get { return m_isPublicFlags[6]; } private set { m_isPublicFlags[6] = value; } }
	/// <summary>Territory hit enter (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritoryEnter { get { return m_isPublicFlags[7]; } private set { m_isPublicFlags[7] = value; } }
	/// <summary>Territory hit exit (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritoryExit { get { return m_isPublicFlags[8]; } private set { m_isPublicFlags[8] = value; } }
	/// <summary>Territory segment x body only hit stay (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritorySegmentStay { get { return m_isPublicFlags[9]; } private set { m_isPublicFlags[9] = value; } }
	/// <summary>Territory segment x body only hit enter (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritorySegmentEnter { get { return m_isPublicFlags[10]; } private set { m_isPublicFlags[10] = value; } }
	/// <summary>Territory segment x body only hit exit (FixedUpdateで値を更新, raw value)</summary>
	public bool isFixedTerritorySegmentExit { get { return m_isPublicFlags[11]; } private set { m_isPublicFlags[11] = value; } }


	/// <summary>Hit visibility point(not hit = null)</summary>
	public BaseMarkPoint hitVisibilityMarkPoint { get; private set; } = null;
	/// <summary>Visibility hit stay</summary>
	public bool isVisibilityStay { get { return m_isPublicFlags[12]; } private set { m_isPublicFlags[12] = value; } }
	/// <summary>Visibility hit enter</summary>
	public bool isVisibilityEnter { get { return m_isPublicFlags[13]; } private set { m_isPublicFlags[14] = value; } }
	/// <summary>Visibility hit exit</summary>
	public bool isVisibilityExit { get { return m_isPublicFlags[14]; } private set { m_isPublicFlags[14] = value; } }
	/// <summary>Visibility hit using preferential point</summary>
	public bool isVisibilityUsingPreferential { get { return m_isPublicFlags[15]; } private set { m_isPublicFlags[15] = value; } }
	public bool isBitFar { get { return m_isPublicFlags[16]; } private set { m_isPublicFlags[16] = value; } }

	//debug only
#if UNITY_EDITOR
	/// <summary>Draw gizmo mesh (debug only)</summary>
	public Mesh dGizmoMesh { get; set; } = null;
	/// <summary>Draw gizmo mesh iD (debug only)</summary>
	public int dMeshID { get; set; } = -1;
	/// <summary>Visibility is draw gizmos? (debug only)</summary>
	public bool dIsDrawGizmos { get { return m_dIsDrawVisibilityGizmos; } set { m_dIsDrawVisibilityGizmos = value; } }

	/// <summary>Draw gizmos? (debug only)</summary>
	[SerializeField, Header("Debug Only"), Tooltip("Draw gizmos? (debug only)")]
	bool m_dIsDrawGizmos = true;
	/// <summary>Visibility is draw gizmos? (debug only)</summary>
	[SerializeField, Tooltip("Visibility is draw gizmos? (debug only)")]
	bool m_dIsDrawVisibilityGizmos = true;
#endif

	/// <summary>当たり判定のレイヤーマスク</summary>
	[Header("Always"), SerializeField, Tooltip("当たり判定の実施間隔")]
	float m_judgmentInterval = 0.1f;

	/// <summary>当たり判定の半径</summary>
	[Header("Sphere Collision"), SerializeField, Tooltip("当たり判定の半径")]
	float m_collisionRadius = 0.5f;

	/// <summary>ターゲットマーカー</summary>
	[Header("Visibility"), SerializeField, Tooltip("ターゲットマーカー")]
    TargetMarker m_targetMarker = null;
	/// <summary>視界当たり判定のレイヤーマスク</summary>
	[SerializeField, Tooltip("視界当たり判定のレイヤーマスク")]
	LayerMaskEx m_visibilityLayerMask = 0;
	/// <summary>Visibility angle</summary>
	[SerializeField, Space, Range(0.0f, 180.0f), Tooltip("Visibility angle")]
	float m_visibilityAngle = 90;
	/// <summary>視界判定に使用する距離</summary>
	[SerializeField, Tooltip("視界判定に使用する距離")]
	float m_visibilityDistance = 10;
	/// <summary>視界判定に使用する距離</summary>
	[SerializeField, Tooltip("視界判定に使用し, 本当にヒットするか判定する距離")]
	float m_reallyVisibilityDistance = 10;
	/// <summary>視界判定時優先的にヒットさせる距離</summary>
	[SerializeField, Tooltip("視界判定時優先的にヒットさせる距離")]
	float m_visibilityPreferentialDistance = 2.0f;

	/// <summary>MarkPointのレイヤーマスク</summary>
	[Header("Markpoint Monitoring"), SerializeField, Tooltip("MarkPointのレイヤーマスク")]
	LayerMaskEx m_markPointLayerMask = 0;
	/// <summary>ポイントタイマーロック判定に使用する距離</summary>
	[SerializeField, Tooltip("ポイントタイマーロック判定に使用する距離")]
	float m_personalRadius = 4;
    [SerializeField, Tooltip("マーキングするターゲット")]
    GameObject m_nowTargetObject = null;

	/// <summary>GoMarking時にMarkPointへのBoxCast判定で使う情報</summary>
	[Header("Instructions to dogs"), SerializeField, Tooltip("GoMarking時にMarkPointへのRaycast判定で使う情報")]
	RaycastInfos m_instructionsMarkPointInfos = new RaycastInfos(0, Vector3.up);
	/// <summary>GoMarking時にMarkPointへのBoxCast判定した際に障害物と判断するLayer</summary>
	[SerializeField, Tooltip("GoMarking時にMarkPointへのBoxCast判定した際に障害物と判断するLayer")]
	LayerMaskEx m_instructionsMarkPointObstacleLayerMask = 0;
	/// <summary>EndMarking時にオス犬へのBoxCast判定で使う情報</summary>
	[SerializeField, Tooltip("EndMarking時にオス犬へのRaycast判定で使う情報")]
	RaycastInfos m_instructionsReturnDogInfos = default;
	/// <summary>EndMarking時にオス犬へのBoxCast判定した際に障害物と判断するLayer</summary>
	[SerializeField, Tooltip("EndMarking時にオス犬へのBoxCast判定した際に障害物と判断するLayer")]
	LayerMaskEx m_instructionsReturnDogObstacleLayerMask = 0;
	/// <summary>GoMarking時の円形当たり判定で使う自身のCenter</summary>
	[SerializeField, Tooltip("GoMarking時の円形当たり判定で使う自身のCenter")]
	Vector3 m_instructionsGoingDogCenter = Vector3.zero;
	/// <summary>GoMarking時の円形当たり判定で使う自身の半径</summary>
	[SerializeField, Tooltip("GoMarking時の円形当たり判定で使う自身の半径")]
	float m_instructionsGoingDogRadius = 10;

	/// <summary>CheckMarkPointで使用するヒットリスト</summary>
	List<BaseMarkPoint> m_checkPoints = new List<BaseMarkPoint>();

	List<int> m_searchIndexes = new List<int>();

	/// <summary>PlayerInfo</summary>
	PlayerAndTerritoryManager.PlayerInfo m_playerInfo = null;
	/// <summary>Forward方向のテリトリー法線</summary>
	Vector3 m_territoryForwardSideNormal = Vector3.zero;
	/// <summary>Judgment timer</summary>
	Timer m_judgmentTimer = new Timer();
	/// <summary>Angle->radian->cos</summary>
	float m_angleToCosine = 0.0f;
	/// <summary>Fixed update completed -> true</summary>
	bool m_isFixedUpdateCompleted = false;
	/// <summary>public bool flags array</summary>
	bool[] m_isPublicFlags = new bool[17];

	/// <summary>
	/// [SetPlayerInfo]
	/// Player情報を設定する
	/// </summary>
	public void SetPlayerInfo(PlayerAndTerritoryManager.PlayerInfo info)
	{
		//Set Info
		if (m_playerInfo == null && info != null)
			m_playerInfo = info;
	}
	/// <summary>
	/// [IsHitInstructionsGoingDog]
	/// Dogと当たり判定を行う
	/// </summary>
	public bool IsHitInstructionsGoingDog(DogAIAgent dogAIAgent)
	{
		if (dogAIAgent == null) return false;

		Vector3 position = transform.LocalToWorldPosition(m_instructionsGoingDogCenter);
		Vector3 target = dogAIAgent.transform.LocalToWorldPosition(dogAIAgent.indicatedDogCenter);
		float radius = (m_instructionsGoingDogRadius + dogAIAgent.indicatedDogRadius);
		position.y = target.y = 0.0f;

		return (target - position).sqrMagnitude < radius * radius;
	}
	/// <summary>
	/// [IsHitInstructionsGoingDog]
	/// DogとBoxCast判定を行う
	/// </summary>
	public bool IsHitInstructionsReturnDog(DogAIAgent dogAIAgent)
	{
		if (dogAIAgent == null) return false;

		Vector3 position = m_instructionsReturnDogInfos.WorldCenter(transform);
		Vector3 target = dogAIAgent.transform.position;
		float toTargetSqrMagnitude = (target - position).sqrMagnitude;
		int instanceID = dogAIAgent.transform.root.GetInstanceID();

		var collisions = m_instructionsReturnDogInfos.RaycastAll(transform, (target - position).normalized, toTargetSqrMagnitude);

		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (m_instructionsReturnDogObstacleLayerMask.EqualBitsForGameObject(collisions[i].transform.gameObject)
				&& (collisions[i].point - position).sqrMagnitude < toTargetSqrMagnitude)
				return false;
		}
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (collisions[i].transform.root.GetInstanceID() == instanceID)
				return true;
		}
		
		return false;
	}
	public void ResetTerritoryHitInfo(bool isHit)
	{
		isTerritoryStay = isHit;
		isTerritoryEnter = false;
		isTerritoryExit = false;

		isFixedTerritoryStay = isHit;
		isFixedTerritoryEnter = false;
		isFixedTerritoryExit = false;
	}


	/// <summary>[Awake]</summary>
	void Awake()
	{
		//視界角度を変換
		m_angleToCosine = Mathf.Cos(m_visibilityAngle * 0.5f * Mathf.Deg2Rad);
		//念の為ね
		for (int i = 0, length = m_isPublicFlags.Length; i < length; ++i)
			m_isPublicFlags[i] = false;
		//Timer start
		m_judgmentTimer.Start();
	}
	/// <summary>[Start]</summary>
	void Start()
	{
		//Timer start
		m_judgmentTimer.Start();
	}
	/// <summary>[Update]</summary>
	void Update()
	{
		//判定間隔計算
		if (m_judgmentTimer.elapasedTime >= m_judgmentInterval)
		{
			//負荷軽減
			Vector3 position = transform.position;

			//Visibilityチェック
			CheckVisibility(ref position);
			//MarkPointチェック
			CheckMarkPoint(ref position);

			CheckShortestPoints(ref position, true);

			//タイマーリスタート
			m_judgmentTimer.Start();
		}
		else
		{
			//負荷軽減
			Vector3 position = transform.position;

			CheckShortestPoints(ref position, false);
		}

		//フラグ判定
		if (m_isFixedUpdateCompleted)
		{
			m_isFixedUpdateCompleted = false;
			isTerritoryEnter = (isTerritoryStay ^ isFixedTerritoryStay) & isFixedTerritoryStay;
			isTerritoryExit = (isTerritoryStay ^ isFixedTerritoryStay) & isTerritoryStay;
			isTerritoryStay = isFixedTerritoryStay;
			isTerritorySegmentEnter = (isTerritorySegmentStay ^ isFixedTerritorySegmentStay) & isFixedTerritorySegmentStay;
			isTerritorySegmentExit = (isTerritorySegmentStay ^ isFixedTerritorySegmentStay) & isTerritorySegmentStay;
			isTerritorySegmentStay = isFixedTerritorySegmentStay;
		}
	}
	/// <summary>[FixedUpdate]</summary>
	private void FixedUpdate()
	{
		//負荷軽減
		Vector3 position = transform.position;
		//Territoryチェック
		CheckTerritory(ref position);
	}

	//Debug only
#if UNITY_EDITOR
	/// <summary>[OnValidate](debug only)</summary>
	void OnValidate()
	{
		dGizmoMesh = null;
	}
#endif

	/// <summary>
	/// [CheckVisibility]
	/// 視界の当たり判定を行う
	/// </summary>
	void CheckVisibility(ref Vector3 position)
	{
		//処理負荷軽減
		Vector3 forward = transform.forward;
		//判定用構造体
		VisibilityContainer container = new VisibilityContainer();
		//候補を保存するリスト
		List<VisibilityContainer> markPoints = new List<VisibilityContainer>();
		List<VisibilityContainer> preferentialMarkPoints = new List<VisibilityContainer>();
		//Overlap collisions
		var collisions = Physics.OverlapSphere(position, m_visibilityDistance, m_visibilityLayerMask);

		Vector3 positionXZ = position; positionXZ.y = 0.0f;
		bool isOldStay = isVisibilityStay;
		isVisibilityUsingPreferential = false;

		//Collision判定ループ
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			//GetComponent
			var point = collisions[i].GetComponent<BaseMarkPoint>();

			//Nullかリンクありでコンティニュー
			if (point == null || (point.isLockFirstPoint) || point.isDeactive) continue;

			Vector3 pointXZ = point.transform.position; pointXZ.y = 0.0f;
			Vector3 absolute = pointXZ - positionXZ;

			//Point格納
			container.SetPoint(point);
			float a = absolute.magnitude;
			//リストに存在しなければforwardとのDotを計算してリスト追加, 距離が近ければ優先リストに追加
			if (absolute.sqrMagnitude < m_visibilityPreferentialDistance * m_visibilityPreferentialDistance)
			{
				if (!preferentialMarkPoints.Contains(container))
				{
					container.forwardToPositionDot = Vector3.Dot(forward, absolute.normalized);
					preferentialMarkPoints.Add(container);
				}
			}
			else if (!markPoints.Contains(container))
			{
				container.forwardToPositionDot = Vector3.Dot(forward, absolute.normalized);
				markPoints.Add(container);
			}
		}

		//優先リストを処理するかで分岐
		if (preferentialMarkPoints.Count > 0)
		{
			if (!JudgeHitVisibility(preferentialMarkPoints, true))
			{
				JudgeHitVisibility(markPoints, false);
				if (isVisibilityStay && !hitVisibilityMarkPoint.isTarget)
					hitVisibilityMarkPoint.SelectThisPoint();
			}
			else
				isVisibilityUsingPreferential = true;
		}
		else
			JudgeHitVisibility(markPoints, false);

		if (isVisibilityStay && ((hitVisibilityMarkPoint.transform.position.ToYZero() - positionXZ).sqrMagnitude
			> m_reallyVisibilityDistance * m_reallyVisibilityDistance))
		{
			hitVisibilityMarkPoint.RemovedThisPoint();
			isVisibilityEnter = (isOldStay ^ false) & false;
			isVisibilityExit = (isOldStay ^ false) & isOldStay;
			isVisibilityStay = false;
			isBitFar = true;
		}
		else isBitFar = false;
	}

	/// <summary>
	/// [JudgeHitVisibility]
	/// ヒットリストを使用して当たり判定計算を行う
	/// </summary>
	bool JudgeHitVisibility(List<VisibilityContainer> markPoints,  bool isMaybeRunItAgain)
	{
		Vector3 hitPoint;
		
		//視界判定ループ
		for (int i = 0; i < markPoints.Count;)
		{
			//視界判定trueなら++i
			if (markPoints[i].forwardToPositionDot > m_angleToCosine)
				++i;
			//視界判定falseならRemove
			else
				markPoints.RemoveAt(i);
		}

		//この時点でCount0なら終了
		if (markPoints.Count == 0)
		{
			if (isMaybeRunItAgain) return false;

			if (hitVisibilityMarkPoint != null) hitVisibilityMarkPoint.RemovedThisPoint();

			hitVisibilityMarkPoint = null;
			EditVisibilityHitFlag(false);
			m_targetMarker.DisableMarker();
			return false;
		}
		//この時点でCount1ならヒット確定として終了
		else if (markPoints.Count == 1)
		{
			if (!IsRaycastMarkPoint(markPoints[0].markPoint, out hitPoint))
			{
				if (isMaybeRunItAgain) return false;

				if (hitVisibilityMarkPoint != null)
				{
					hitVisibilityMarkPoint.RemovedThisPoint();
					m_targetMarker.ClearMarker();
				}

				hitVisibilityMarkPoint = null;
				EditVisibilityHitFlag(false);
				m_targetMarker.EnableMarker(hitPoint);
				return false;
			}

			if (hitVisibilityMarkPoint != markPoints[0].markPoint && hitVisibilityMarkPoint != null)
			{
				hitVisibilityMarkPoint.RemovedThisPoint();
				m_targetMarker.ClearMarker();
			}

			EditVisibilityHitFlag(true);
			hitVisibilityMarkPoint = markPoints[0].markPoint;
			m_nowTargetObject = hitVisibilityMarkPoint.SelectThisPoint();
			m_targetMarker.EnableMarker(m_nowTargetObject.transform.position);
			return true;
		}

		//forwardとの角度差でソートを行う
		Vector3 minNotHitPoint = Vector3.zero;
		float minDot = 0.0f;
		int minIndex = -1;
		float minDotNotHit = 0.0f;
		int minIndexNotHit = -1;
		bool isResult = false;
		for (int i = 0, count = markPoints.Count; i < count; ++i)
		{
			isResult = IsRaycastMarkPoint(markPoints[i].markPoint, out hitPoint);
			if (isResult && minDot < markPoints[i].forwardToPositionDot)
			{
				minDot = markPoints[i].forwardToPositionDot;
				minIndex = i;
			}
			else if (!isResult && hitPoint != Vector3.zero
				&& minDotNotHit < markPoints[i].forwardToPositionDot)
			{
				minNotHitPoint = hitPoint;
				minDotNotHit = markPoints[i].forwardToPositionDot;
				minIndexNotHit = i;
			}
		}

		if (minIndex >= 0)
		{
			//もっとも角度差が小さいものを選択する
			if (hitVisibilityMarkPoint != markPoints[minIndex].markPoint && hitVisibilityMarkPoint != null)
			{
				hitVisibilityMarkPoint.RemovedThisPoint();
				m_targetMarker.ClearMarker();
			}

			hitVisibilityMarkPoint = markPoints[minIndex].markPoint;
			m_nowTargetObject = hitVisibilityMarkPoint.SelectThisPoint();
			m_targetMarker.EnableMarker(m_nowTargetObject.transform.position);
			EditVisibilityHitFlag(true);
		}
		else if (minIndexNotHit >= 0)
		{
			if (isMaybeRunItAgain) return false;

			if (hitVisibilityMarkPoint != null)
			{
				hitVisibilityMarkPoint.RemovedThisPoint();
				m_targetMarker.ClearMarker();
			}

			hitVisibilityMarkPoint = null;
			EditVisibilityHitFlag(false);

			m_targetMarker.EnableMarker(minNotHitPoint);
			return false;
		}
		else
		{
			if (isMaybeRunItAgain) return false;

			if (hitVisibilityMarkPoint != null) hitVisibilityMarkPoint.RemovedThisPoint();

			hitVisibilityMarkPoint = null;
			EditVisibilityHitFlag(false);
			m_targetMarker.DisableMarker();
			return false;
		}
		return true;
	}
	/// <summary>
	/// [IsBoxCastMarkPoint]
	/// MarkPointとBoxCast判定を行う
	/// </summary>
	public bool IsRaycastMarkPoint(BaseMarkPoint markPoint, out Vector3 hitPoint)
	{
		hitPoint = Vector3.zero;
		if (markPoint == null) return false;

		Vector3 position = m_instructionsMarkPointInfos.WorldCenter(transform);
		Vector3 target = markPoint.transform.position;
		float toTargetSqrMagnitude = (target - position).sqrMagnitude;
		int instanceID = markPoint.transform.root.GetInstanceID();
		
		var collisions = m_instructionsMarkPointInfos.RaycastAll(transform, (target - position).normalized, toTargetSqrMagnitude);
	
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (m_instructionsMarkPointObstacleLayerMask.EqualBitsForGameObject(collisions[i].transform.gameObject)
				&& (collisions[i].point - position).sqrMagnitude < toTargetSqrMagnitude)
			{
				hitPoint = collisions[i].point;
				return false;
			}
		}
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (collisions[i].transform.root.GetInstanceID() == instanceID)
			{
				hitPoint = collisions[i].point;
				return true;
			}
		}

		hitPoint = Vector3.zero;
		return false;
	}
	/// <summary>
	/// [EditVisibilityHitFlag]
	/// Edited visibility hit flag
	/// </summary>
	bool EditVisibilityHitFlag(bool isNewValue)
	{
		isVisibilityEnter = (isVisibilityStay ^ isNewValue) & isNewValue;
		isVisibilityExit = (isVisibilityStay ^ isNewValue) & isVisibilityStay;
		return isVisibilityStay = isNewValue;
	}

	/// <summary>
	/// [CheckTerritory]
	/// テリトリーの当たり判定を行う
	/// </summary>
	void CheckTerritory(ref Vector3 position)
	{
		//ヒット計算
		bool isHitBody = false;
		bool isResult = CollisionTerritory.HitCircleAndRayTerritory(m_playerInfo.territorialArea, 
			position, transform.forward, out m_territoryForwardSideNormal, m_collisionRadius, 1000.0f, out isHitBody);

		//フラグ判定
		m_isFixedUpdateCompleted = true;

		isFixedTerritoryEnter = (isFixedTerritoryStay ^ isResult) & isResult;
		isFixedTerritoryExit = (isFixedTerritoryStay ^ isResult) & isFixedTerritoryStay;
		isFixedTerritoryStay = isResult;

		isFixedTerritorySegmentEnter = (isFixedTerritorySegmentStay ^ isHitBody) & isHitBody;
		isFixedTerritorySegmentExit = (isFixedTerritorySegmentStay ^ isHitBody) & isFixedTerritorySegmentStay;
		isFixedTerritorySegmentStay = isHitBody;
	}

	/// <summary>
	/// [CheckMarkPoint]
	/// マークポイントの当たり判定を行う
	/// </summary>
	void CheckMarkPoint(ref Vector3 position)
	{
		//まずは全てUnpauseさせる
		foreach (var e in m_checkPoints)
			e.SetPlayerNearby(false);
		m_checkPoints.Clear();

		//Overlap
		var collisions = Physics.OverlapSphere(position, m_personalRadius, m_markPointLayerMask);

		//Not hit -> return
		if (collisions.Length == 0)
			return;

		//格納ループ
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			var component = collisions[i].GetComponent<BaseMarkPoint>();
			if (component != null && component.isLinked && !m_checkPoints.Contains(component))
				m_checkPoints.Add(component);
		}

		//Pauseさせるループ
		for (int i = 0, count = m_checkPoints.Count; i < count; ++i)
			m_checkPoints[i].SetPlayerNearby(true);
	}

	void CheckShortestPoints(ref Vector3 position, bool isFixedUpdate)
	{
		Vector3 minPoint = default, point;
		float minDistance = 1000000.0f;
		float distance = 0.0f;
		int minIndex = 0;

		m_searchIndexes.Clear();

		if (m_playerInfo.borderTerritorys.Count > 1)
		{
			int index0, index1;
			for (int i = 0, count = m_playerInfo.borderTerritorys.Count - 1; i < count; ++i)
			{
				distance = (CalculatePerpendicular(m_playerInfo.borderTerritorys[i],
					m_playerInfo.borderTerritorys[i + 1], ref position) - position).sqrMagnitude;

				if (minDistance > distance)
				{
					minDistance = distance;
					minIndex = i;
				}
			}

			distance = (CalculatePerpendicular(m_playerInfo.borderTerritorys[m_playerInfo.borderTerritorys.Count - 1],
					m_playerInfo.borderTerritorys[0], ref position) - position).sqrMagnitude;

			if (minDistance > distance)
			{
				index0 = m_playerInfo.borderTerritorys[m_playerInfo.borderTerritorys.Count - 1].pointInstanceID;
				index1 = m_playerInfo.borderTerritorys[0].pointInstanceID;
			}
			else
			{
				index0 = m_playerInfo.borderTerritorys[minIndex].pointInstanceID;
				index1 = m_playerInfo.borderTerritorys[minIndex + 1].pointInstanceID;
			}


			for (int i = 0, count = m_playerInfo.territorialAreaBelongIndexes.Count; i < count; ++i)
			{
				if (m_playerInfo.territorialAreaBelongIndexes[i] == index0 || m_playerInfo.territorialAreaBelongIndexes[i] == index1)
					m_searchIndexes.Add(i);
			}
		}
		else
		{
			for (int i = 0, count = m_playerInfo.territorialAreaBelongIndexes.Count; i < count; ++i)
			{
				m_searchIndexes.Add(i);
			}
		}


		{
			minDistance = 1000000.0f;
			minIndex = 0;

			for (int i = 0, count = m_searchIndexes.Count; i < count; ++i)
			{
				if (m_playerInfo.territorialArea.Count - 1 > m_searchIndexes[i])
				{
					point = CalculatePerpendicular(ref position, m_searchIndexes[i], m_searchIndexes[i] + 1);
					distance = (point - position).sqrMagnitude;
				}
				else
				{
					point = CalculatePerpendicular(ref position, m_searchIndexes[i], 0);
					distance = (point- position).sqrMagnitude;
				}

				if (minDistance > distance)
				{
					minPoint = point;
					minDistance = distance;
					minIndex = i;
				}
			}

			if (m_playerInfo.territorialArea.Count - 1 > m_searchIndexes[minIndex])
				m_playerInfo.SetShortestTerritoryPoint(minPoint, minIndex, minIndex + 1, isFixedUpdate);
			else
				m_playerInfo.SetShortestTerritoryPoint(minPoint, minIndex, 0, isFixedUpdate);
		}
	}
	Vector3 CalculatePerpendicular(ref Vector3 position, int index0, int index1)
	{
		Vector3 positionXZ = position;
		Vector3 startToEnd = m_playerInfo.territorialArea[index1] - m_playerInfo.territorialArea[index0];
		Vector3 startToPosition = position - m_playerInfo.territorialArea[index0];
		positionXZ.y = startToPosition.y = startToEnd.y = 0.0f;

		float t = Vector3.Dot(startToEnd.normalized, startToPosition) / startToEnd.magnitude;

		if (t < 0) return m_playerInfo.territorialArea[index0];
		else if (t > 1) return m_playerInfo.territorialArea[index1];
		else return position + startToEnd * t- startToPosition;
	}
	Vector3 CalculatePerpendicular(BaseMarkPoint start, BaseMarkPoint end, ref Vector3 position)
	{
		Vector3 positionXZ = position, startPositionXZ = start.transform.position.ToYZero();
		Vector3 startToEnd = end.transform.position.ToYZero() - startPositionXZ;
		Vector3 startToPosition = position - startPositionXZ;
		positionXZ.y = startToPosition.y = startToEnd.y = 0.0f;

		float t = Vector3.Dot(startToEnd.normalized, startToPosition) / startToEnd.magnitude;

		if (t < 0) return startPositionXZ;
		else if (t > 1) return end.transform.position.ToYZero();
		else return position + startToEnd * t - startToPosition;
	}


	//debug only
#if UNITY_EDITOR
	static readonly Color m_cdPersonalColor = new Color(0.95f, 0.1f, 0.95f);

	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		if (UnityEditor.EditorApplication.isPlaying)
			Gizmos.DrawCube(m_playerInfo.shortestTerritoryBorderPoint, new Vector3(0.1f, 10, 0.1f));

		if (!m_dIsDrawGizmos) return;

		Vector3 position = transform.position;

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position + Vector3.up, m_collisionRadius);


		Gizmos.color = m_cdPersonalColor;
		Gizmos.DrawWireSphere(transform.position, m_personalRadius);


		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.LocalToWorldPosition(m_instructionsGoingDogCenter), m_instructionsGoingDogRadius);

		Vector3 vec;
		bool isHit = IsRaycastMarkPoint(hitVisibilityMarkPoint, out vec);
		m_instructionsMarkPointInfos.DOnDrawGizmos(transform, isHit ? Color.red : Color.yellow,
			hitVisibilityMarkPoint != null ? (hitVisibilityMarkPoint.transform.position - transform.position).normalized : transform.forward,
			!isHit || vec != Vector3.zero ? vec.magnitude : 10.0f);

		int instanceID = gameObject.GetInstanceID();
		if (PlayerAndTerritoryManager.instance == null)
			m_instructionsReturnDogInfos.DOnDrawGizmos(transform, Color.white, transform.forward, 15.0f);
		else
		{
			Vector3 centerPosition = m_instructionsReturnDogInfos.WorldCenter(transform);
			foreach (var e in ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()])
			{
				isHit = IsHitInstructionsReturnDog(e);
				m_instructionsReturnDogInfos.DOnDrawGizmos(transform, isHit ? Color.black : Color.white,
					(e.transform.position - centerPosition).normalized, Vector3.Distance(transform.position, e.transform.position));
			}
		}
	}
#endif
}
