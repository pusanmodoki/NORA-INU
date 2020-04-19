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
	public float personalDistance { get { return m_personalDistance; } }
	/// <summary>視界判定に使用する距離</summary>
	public float visibilityDistance { get { return m_visibilityDistance; } }

	/// <summary>Forward方向のテリトリー法線(β)</summary>
	public Vector3 territoryForwardSideNormal { get { return m_territoryForwardSideNormall; } }
	/// <summary>Territory hit stay</summary>
	public bool isTerritoryStay { get; private set; } = false;
	/// <summary>Territory hit enter</summary>
	public bool isTerritoryEnter { get; private set; } = false;
	/// <summary>Territory hit exit</summary>
	public bool isTerritoryExit { get; private set; } = false;

	/// <summary>Hit visibility point(not hit = null)</summary>
	public BaseMarkPoint hitVisibilityMarkPoint { get; private set; } = null;
	/// <summary>Visibility angle (get only)</summary>
	public float visibilityAngle { get { return m_visibilityAngle; } }
	/// <summary>Visibility hit stay</summary>
	public bool isVisibilityStay { get; private set; } = false;
	/// <summary>Visibility hit enter</summary>
	public bool isVisibilityEnter { get; private set; } = false;
	/// <summary>Visibility hit exit</summary>
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

	/// <summary>当たり判定のレイヤーマスク</summary>
	[Header("Always"), SerializeField, Tooltip("当たり判定の実施間隔")]
	float m_judgmentInterval = 0.1f;

	/// <summary>当たり判定の半径</summary>
	[Header("Sphere Collision"), SerializeField, Tooltip("当たり判定の半径")]
	float m_collisionRadius = 0.5f;	

	/// <summary>当たり判定のレイヤーマスク</summary>
	[Header("Visibility"), SerializeField, Tooltip("当たり判定のレイヤーマスク")]
	LayerMaskEx m_visibilityLayerMask = 0;
	/// <summary>Visibility angle</summary>
	[SerializeField, Range(0.0f, 180.0f), Tooltip("Visibility angle")]
	float m_visibilityAngle = 90;
	/// <summary>視界判定に使用する距離</summary>
	[SerializeField, Tooltip("視界判定に使用する距離")]
	float m_visibilityDistance = 10;

	/// <summary>MarkPointのレイヤーマスク</summary>
	[Header("Markpoint Monitoring"), SerializeField, Tooltip("MarkPointのレイヤーマスク")]
	LayerMaskEx m_markPointLayerMask = 0;
	/// <summary>ポイントタイマーロック判定に使用する距離</summary>
	[SerializeField, Tooltip("ポイントタイマーロック判定に使用する距離")]
	float m_personalDistance = 10;
	
	/// <summary>CheckMarkPointで使用するヒットリスト</summary>
	List<BaseMarkPoint> m_checkPoints = new List<BaseMarkPoint>();
	/// <summary>PlayerInfo</summary>
	PlayerAndTerritoryManager.PlayerInfo m_playerInfo = null;
	/// <summary>Forward方向のテリトリー法線</summary>
	Vector3 m_territoryForwardSideNormall = Vector3.zero;
	/// <summary>Judgment timer</summary>
	Timer m_judgmentTimer = new Timer();
	/// <summary>NavMesh build interval timer</summary>
	Timer m_navMeshBuildIntervalTimer = new Timer();
	/// <summary>Angle->radian->cos</summary>
	float m_angleToCosine = 0.0f;

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

	/// <summary>[Start]</summary>
	void Start()
	{
		//視界角度を変換
		m_angleToCosine = Mathf.Cos(m_visibilityAngle * 0.5f * Mathf.Deg2Rad);

		//Timer start
		m_judgmentTimer.Start();
		m_navMeshBuildIntervalTimer.Start();
	}
	/// <summary>[Update]</summary>
	void Update()
	{
		//判定間隔計算
		if (m_judgmentTimer.elapasedTime >= m_judgmentInterval)
		{
			//負荷軽減
			Vector3 position = transform.position;

			//Territoryチェック
			CheckTerritory(ref position);
			//Visibilityチェック
			CheckVisibility(ref position);
			//MarkPointチェック
			CheckMarkPoint(ref position);

			//タイマーリスタート
			m_judgmentTimer.Start();
		}
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
		//Overlap collisions
		var collisions = Physics.OverlapSphere(position, m_visibilityDistance, m_visibilityLayerMask);

		//Collision判定ループ
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			//GetComponent
			var point = collisions[i].GetComponent<BaseMarkPoint>();

			//Nullかリンクありでコンティニュー
			if (point == null || (point.isLinked & point.isPauseTimer)) continue;
			
			//Point格納
			container.SetPoint(point);
			//リストに存在しなければforwardとのDotを計算してリスト追加
			if (!markPoints.Contains(container))
			{
				container.forwardToPositionDot = Vector3.Dot(forward, (point.transform.position - position).normalized);
				markPoints.Add(container);
			}
		}

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
			EditVisibilityHitFlag(false);
			return;
		}
		//この時点でCount1ならヒット確定として終了
		else if (markPoints.Count == 1)
		{ 
			hitVisibilityMarkPoint = markPoints[0].markPoint;
			EditVisibilityHitFlag(true);
			return;
		}

		//forwardとの角度差でソートを行う
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

		//もっとも角度差が小さいものを選択する
		hitVisibilityMarkPoint = markPoints[markPoints.Count - 1].markPoint;
		EditVisibilityHitFlag(true);
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
		bool isResult = CollisionTerritory.HitCircleAndRayTerritory(
			m_playerInfo.territorialArea, position, out m_territoryForwardSideNormall, m_collisionRadius);

		//フラグ判定
		isTerritoryEnter = (isTerritoryStay ^ isResult) & isResult;
		isTerritoryExit = (isTerritoryStay ^ isResult) & isTerritoryStay;
		isTerritoryStay = isResult;
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
		var collisions = Physics.OverlapSphere(position, m_personalDistance, m_markPointLayerMask);

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
}
