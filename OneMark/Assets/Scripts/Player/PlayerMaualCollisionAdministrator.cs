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

	/// <summary>ポイントタイマーロック判定に使用する距離</summary>
	public float personalDistance { get { return m_personalRadius; } }
	/// <summary>視界判定に使用する距離</summary>
	public float visibilityDistance { get { return m_visibilityDistance; } }
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
	/// <summary>Visibility angle (get only)</summary>
	public float visibilityAngle { get { return m_visibilityAngle; } }
	/// <summary>Visibility hit stay</summary>
	public bool isVisibilityStay { get { return m_isPublicFlags[12]; } private set { m_isPublicFlags[12] = value; } }
	/// <summary>Visibility hit enter</summary>
	public bool isVisibilityEnter { get { return m_isPublicFlags[13]; } private set { m_isPublicFlags[14] = value; } }
	/// <summary>Visibility hit exit</summary>
	public bool isVisibilityExit { get { return m_isPublicFlags[14]; } private set { m_isPublicFlags[14] = value; } }
	/// <summary>Visibility hit using preferential point</summary>
	public bool isVisibilityUsingPreferential { get { return m_isPublicFlags[15]; } private set { m_isPublicFlags[15] = value; } }

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
	[Header("Instructions to dogs"), SerializeField, Tooltip("GoMarking時にMarkPointへのBoxCast判定で使う情報")]
	BoxCastInfos m_instructionsMarkPointInfos = default;
	/// <summary>GoMarking時にMarkPointへのBoxCast判定した際に障害物と判断するLayer</summary>
	[SerializeField, Tooltip("GoMarking時にMarkPointへのBoxCast判定した際に障害物と判断するLayer")]
	LayerMaskEx m_instructionsMarkPointObstacleLayerMask = 0;
	/// <summary>EndMarking時にオス犬へのBoxCast判定で使う情報</summary>
	[SerializeField, Tooltip("EndMarking時にオス犬へのBoxCast判定で使う情報")]
	BoxCastInfos m_instructionsReturnDogInfos = default;
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
	bool[] m_isPublicFlags = new bool[16];

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
	/// [IsHitInstructionsMarkPoint]
	/// MarkPointとBoxCast判定を行う
	/// </summary>
	public bool IsHitInstructionsMarkPoint(BaseMarkPoint markPoint)
	{
		if (markPoint == null) return false;

		Vector3 position = m_instructionsMarkPointInfos.WorldCenter(transform);
		Vector3 target = markPoint.transform.position;
		var collisions = Physics.BoxCastAll(position, m_instructionsMarkPointInfos.overlapSize, (target - position).normalized,
			transform.rotation, Vector3.Distance(target, position), m_instructionsMarkPointInfos.layerMask);

		int instanceID = markPoint.transform.GetInstanceID();
		bool isHit = false;

		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (m_instructionsMarkPointObstacleLayerMask.EqualBitsForGameObject(collisions[i].transform.gameObject)
				&& (collisions[i].point - position).sqrMagnitude < (target - position).sqrMagnitude)
				return false;
			else if (collisions[i].transform.GetInstanceID() == instanceID)
				isHit = true;
		}

		return isHit;
	}
	/// <summary>
	/// [IsHitInstructionsGoingDog]
	/// DogとBoxCast判定を行う
	/// </summary>
	public bool IsHitInstructionsGoingDog(DogAIAgent dogAIAgent)
	{
		if (dogAIAgent == null) return false;

		Vector3 position = transform.LocalToWorldPosition(m_instructionsGoingDogCenter);
		Vector3 target = dogAIAgent.transform.LocalToWorldPosition(dogAIAgent.indicatedDogCenter);
		float radius = (m_instructionsGoingDogRadius + dogAIAgent.indicatedDogRadius);
		radius *= radius;
		position.y = target.y = 0.0f;

		return (target - position).sqrMagnitude < radius;
	}
	/// <summary>
	/// [IsHitInstructionsGoingDog]
	/// Dogと当たり判定を行う
	/// </summary>
	public bool IsHitInstructionsReturnDog(DogAIAgent dogAIAgent)
	{
		if (dogAIAgent == null) return false;

		Vector3 position = m_instructionsReturnDogInfos.WorldCenter(transform);
		Vector3 target = dogAIAgent.transform.position;

		var collisions = Physics.BoxCastAll(position, m_instructionsReturnDogInfos.overlapSize, 
			(target - position).normalized, transform.rotation, Vector3.Distance(target, position) * 2.0f, m_instructionsReturnDogInfos.layerMask);

		int instanceID = dogAIAgent.transform.GetInstanceID();
		bool isHit = false;

		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			if (m_instructionsReturnDogObstacleLayerMask.EqualBitsForGameObject(collisions[i].transform.gameObject)
				&& (collisions[i].point - position).sqrMagnitude < (target - position).sqrMagnitude)
				return false;
			else if (collisions[i].transform.GetInstanceID() == instanceID)
				isHit = true;
		}

		return isHit;
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

			//タイマーリスタート
			m_judgmentTimer.Start();
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
		isVisibilityUsingPreferential = false;

		//Collision判定ループ
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			//GetComponent
			var point = collisions[i].GetComponent<BaseMarkPoint>();

			//Nullかリンクありでコンティニュー
			if (point == null || (point.isLockFirstPoint)) continue;

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
			var old = hitVisibilityMarkPoint;
			JudgeHitVisibility(preferentialMarkPoints);

			if (!isVisibilityStay)
			{
				hitVisibilityMarkPoint = old;
				JudgeHitVisibility(markPoints);
			}
			else
				isVisibilityUsingPreferential = true;
		}
		else
			JudgeHitVisibility(markPoints);
	}

	/// <summary>
	/// [JudgeHitVisibility]
	/// ヒットリストを使用して当たり判定計算を行う
	/// </summary>
	void JudgeHitVisibility(List<VisibilityContainer> markPoints)
	{
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
			if (hitVisibilityMarkPoint != null) hitVisibilityMarkPoint.RemovedThisPoint();

			hitVisibilityMarkPoint = null;
			EditVisibilityHitFlag(false);
			m_targetMarker.SetTarget(null);
			return;
		}
		//この時点でCount1ならヒット確定として終了
		else if (markPoints.Count == 1)
		{
			if (hitVisibilityMarkPoint != markPoints[0].markPoint)
			{
				if (hitVisibilityMarkPoint != null) hitVisibilityMarkPoint.RemovedThisPoint();

				hitVisibilityMarkPoint = markPoints[0].markPoint;
				m_nowTargetObject = hitVisibilityMarkPoint.SelectThisPoint();
				m_targetMarker.SetTarget(m_nowTargetObject);
				EditVisibilityHitFlag(true);
			}
			return;
		}

		//forwardとの角度差でソートを行う
		float minDot = 0.0f;
		int minIndex = -1;
		for (int i = 0, count = markPoints.Count; i < count; ++i)
		{
			if (minDot < markPoints[i].forwardToPositionDot)
			{
				minDot = markPoints[i].forwardToPositionDot;
				minIndex = i;
			}
		}

		//もっとも角度差が小さいものを選択する
		if (hitVisibilityMarkPoint != markPoints[minIndex].markPoint)
		{
			if (hitVisibilityMarkPoint != null) hitVisibilityMarkPoint.RemovedThisPoint();

			hitVisibilityMarkPoint = markPoints[minIndex].markPoint;
			m_nowTargetObject = hitVisibilityMarkPoint.SelectThisPoint();
			m_targetMarker.SetTarget(m_nowTargetObject);
			EditVisibilityHitFlag(true);
		}
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
			if (component != null && !m_checkPoints.Contains(component))
				m_checkPoints.Add(component);
		}

		//Pauseさせるループ
		for (int i = 0, count = m_checkPoints.Count; i < count; ++i)
			m_checkPoints[i].SetPlayerNearby(true);
	}


	//debug only
#if UNITY_EDITOR
	static readonly Color m_cdPersonalColor = new Color(0.95f, 0.1f, 0.95f);

	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		if (!m_dIsDrawGizmos) return;

		Vector3 position = transform.position;

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position + Vector3.up, m_collisionRadius);


		Gizmos.color = m_cdPersonalColor;
		Gizmos.DrawWireSphere(transform.position, m_personalRadius);


		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.LocalToWorldPosition(m_instructionsGoingDogCenter), m_instructionsGoingDogRadius);

		bool isHit = IsHitInstructionsMarkPoint(hitVisibilityMarkPoint);
		m_instructionsMarkPointInfos.DOnDrawGizmos(transform, isHit ? Color.red : Color.yellow,
			hitVisibilityMarkPoint != null ? (hitVisibilityMarkPoint.transform.position - transform.position).normalized : transform.forward,
			hitVisibilityMarkPoint != null ? Vector3.Distance(transform.position, hitVisibilityMarkPoint.transform.position) : 10.0f);

		int instanceID = gameObject.GetInstanceID();
		if (PlayerAndTerritoryManager.instance == null)
			m_instructionsReturnDogInfos.DOnDrawGizmos(transform, Color.white, transform.forward, 15.0f);
		else
		{
			foreach(var e in ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()])
			{
				isHit = IsHitInstructionsReturnDog(e);
				m_instructionsReturnDogInfos.DOnDrawGizmos(transform, isHit ? Color.black : Color.white,
					(e.transform.position - transform.position).normalized, Vector3.Distance(transform.position, e.transform.position));
			}
		}
	}
#endif
}
