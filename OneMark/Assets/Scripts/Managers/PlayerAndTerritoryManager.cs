﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

/// <summary>
/// Player情報とテリトリーを管理するPlayerAndTerritoryManager 
/// </summary>
[DefaultExecutionOrder(-100)]
public class PlayerAndTerritoryManager : MonoBehaviour
{
	public delegate void ChangeTerritoryCallback();

	/// <summary>
	/// Playerの情報を記録するPlayerInfo
	/// </summary>
	public class PlayerInfo
	{
		/// <summary>
		/// Managerが使用するUsedManager
		/// </summary>
		public class UsedManager
		{
			/// <summary>[コンストラクタ]</summary>
			public UsedManager(float calucrateInterval, float calucrateFeatureInterval)
			{
				intervalTimer = new Timer();
				featureIntervalTimer = new Timer();
				nextForceCalucrateFrameCount = 0;
				isCalucrateNextUpdate = false;
				this.calucrateInterval = calucrateInterval;
				this.calucrateFeatureInterval = calucrateFeatureInterval;

				intervalTimer.Start();
				featureIntervalTimer.Start();
			}

			/// <summary>テリトリー計算タイマー</summary>
			public Timer intervalTimer;
			/// <summary>未来のテリトリー計算タイマー</summary>
			public Timer featureIntervalTimer;
			/// <summary>テリトリー計算間隔</summary>
			public float calucrateInterval;
			/// <summary>未来のテリトリー計算間隔</summary>
			public float calucrateFeatureInterval;

			/// <summary>次回テリトリー計算するフレーム数</summary>
			public int nextForceCalucrateFrameCount;
			/// <summary>次回の通常更新で計算する？</summary>
			public bool isCalucrateNextUpdate;
		}

		/// <summary>[コンストラクタ]</summary>
		public PlayerInfo(GameObject gameObject, PlayerManagerIntermediary territoryIntermediary,
			PlayerMaualCollisionAdministrator maualCollisionAdministrator, PlayerNavMeshController navMeshController,
			PlayerInput input,
			BoxCastFlags groundFlag, UnityEngine.AI.NavMeshAgent navMeshAgent, Rigidbody rigidBody, 
			GameObject[] followPoints, GameObject resultCameraLookPoint, GameObject resultCameraMovePoint)
		{
			this.gameObject = gameObject;
			this.transform = gameObject.transform;
			this.managerIntermediary = territoryIntermediary;
			this.maualCollisionAdministrator = maualCollisionAdministrator;
			this.input = input;
			this.navMeshController = navMeshController;
			this.groundFlag = groundFlag;
			this.navMeshAgent = navMeshAgent;
			this.rigidBody = rigidBody;
			this.followPoints = followPoints;
			this.resultCameraLookPoint = resultCameraLookPoint;
			this.resultCameraMovePoint = resultCameraMovePoint;
			instanceID = gameObject.GetInstanceID();
		}

		/// <summary>Player game object</summary>
		public GameObject gameObject { get; private set; }
		/// <summary>Player transform</summary>
		public Transform transform { get; private set; }
		/// <summary>Player manager intermediary</summary>
		public PlayerManagerIntermediary managerIntermediary { get; private set; }
		/// <summary>Player maual collision administrator</summary>
		public PlayerMaualCollisionAdministrator maualCollisionAdministrator { get; private set; }
		/// <summary>Player input</summary>
		public PlayerInput input { get; private set; }
		/// <summary>Player nav mesh controller</summary>
		public PlayerNavMeshController navMeshController { get; private set; }
		/// <summary>Player ground flag</summary>
		public BoxCastFlags groundFlag { get; private set; }
		/// <summary>Player nav mesh agent</summary>
		public UnityEngine.AI.NavMeshAgent navMeshAgent { get; private set; }
		/// <summary>Player rigid body</summary>
		public Rigidbody rigidBody { get; private set; }
		/// <summary>Player instance id</summary>
		public int instanceID { get; private set; }
		/// <summary>All territory mark points (クラス)</summary>
		public List<BaseMarkPoint> allTerritorys { get; private set; } = new List<BaseMarkPoint>();
		/// <summary>外周のterritory mark points (クラス)</summary>
		public List<BaseMarkPoint> borderTerritorys { get; private set; } = new List<BaseMarkPoint>();
		/// <summary>ボリュームを加えたテリトリー圏 (ポジション)</summary>
		public List<Vector3> territorialArea { get; private set; } = new List<Vector3>();
		/// <summary>ボリュームを加えた少し未来のテリトリー圏 (ポジション)</summary>
		public List<Vector3> featureTerritorialArea { get; private set; } = new List<Vector3>();
		/// <summary>Follow point game objects</summary>
		public GameObject[] followPoints { get; private set; } = null;
		/// <summary>Result camera look point game objects</summary>
		public GameObject resultCameraLookPoint { get; private set; } = null;
		/// <summary>Result camera move point game objects</summary>
		public GameObject resultCameraMovePoint { get; private set; } = null;
		/// <summary>テリトリー変更時にCallされるコールバック (計算時ではなく, AddMarkPoint, RemoveMarkPoint実行でCall)</summary>
		public ChangeTerritoryCallback changeTerritoryCallback { get; set; } = null;
		/// <summary>Link event now?</summary>
		public bool isLinkEvent { get; private set; }
		/// <summary>Used manager valus</summary>
		public UsedManager usedManager { get; private set; } = new UsedManager(0, 0);

		/// <summary>
		/// [SetLinkEvent]
		/// Set isLinkEvent
		/// </summary>
		public void SetLinkEvent(bool isSet) { isLinkEvent = isSet; }
		/// <summary>
		/// [AddMarkPoint]
		/// MarkPointを追加する
		/// 引数1: MarkPoint
		/// </summary>
		public void AddMarkPoint(BaseMarkPoint point)
		{
			if (allTerritorys.Contains(point))
			{
#if UNITY_EDITOR
				Debug.LogError("Error!! PlayerInfo->AddMarkPoint\n added point");
#endif
				return;
			}

			//追加->Callback->計算予約
			allTerritorys.Add(point);
			changeTerritoryCallback?.Invoke();
			PlayerAndTerritoryManager.instance.ReserveCalucrateTerritory(instanceID);
		}
		/// <summary>
		/// [RemoveMarkPoint]
		/// MarkPointを削除する
		/// 引数1: MarkPoint
		/// </summary>
		public void RemoveMarkPoint(BaseMarkPoint point)
		{
			if (!allTerritorys.Contains(point))
			{
#if UNITY_EDITOR
				Debug.LogError("Error!! PlayerInfo->RemoveMarkPoint\n not added point");
#endif
				return;
			}

			//削除->Callback->計算予約
			allTerritorys.Remove(point);
			changeTerritoryCallback?.Invoke();
			PlayerAndTerritoryManager.instance.ReserveCalucrateTerritory(instanceID);
		}
	}


	/// <summary>Static instance</summary>
	public static PlayerAndTerritoryManager instance { get; private set; } = null;

	/// <summary>All players</summary>
	public ReadOnlyDictionary<int, PlayerInfo> allPlayers { get; private set; } = null;
	/// <summary>Main player</summary>
	public PlayerInfo mainPlayer { get; private set; } = null;

	/// <summary>ボリューム円の分割数</summary>
	[SerializeField, Range(0, 100), Tooltip("ボリューム円の分割数")]
	int m_divisionVolume = 10;

	/// <summary>ボリューム円の半径</summary>
	[SerializeField, Space, Range(0, 10), Tooltip("ボリューム円の半径")]
	float m_radiusVolume = 3.0f;
	/// <summary>テリトリー計算間隔</summary>
	[SerializeField, Range(0, 2), Tooltip("テリトリー計算間隔")]
	float m_calucrateTerritoryIntervalSeconds = 0.2f;

	/// <summary>ボリューム円の半径</summary>
	[SerializeField, Space, Range(0, 10), Tooltip("ボリューム円の半径")]
	float m_featureRadiusVolume = 3.1f;
	/// <summary>未来のテリトリー計算間隔</summary>
	[SerializeField, Range(0, 2), Tooltip("未来のテリトリー計算間隔")]
	float m_calucrateFeatureTerritoryIntervalSeconds = 0.2f;
	/// <summary>未来のテリトリー判断に使用する、除外とみなす残り時間</summary>
	[SerializeField, Range(0, 5), Tooltip("未来のテリトリー判断に使用する、除外とみなす残り時間")]
	float m_featureTerritoryExcludeSeconds = 3.0f;

	/// <summary>Manage players</summary>
	Dictionary<int, PlayerInfo> m_players = null;
	/// <summary>Grahamソート用リスト(効率化のためメンバに)</summary>
	List<GrahamScan.CustomFormat> m_grahamResult = new List<GrahamScan.CustomFormat>();
	/// <summary>ボリューム座標</summary>
	Vector3[] m_volumePoints = null;
	/// <summary>ボリューム座標 (feature)</summary>
	Vector3[] m_featureVolumePoints = null;
	/// <summary>計算間隔用タイマー</summary>
	Timer m_intervalTimer = new Timer();


	/// <summary>[Awake]</summary>
	void Awake()
	{
		//インスタンス登録
		instance = this;
		m_players = new Dictionary<int, PlayerInfo>();
		allPlayers = new ReadOnlyDictionary<int, PlayerInfo>(m_players);

		//タイマー作動
		m_intervalTimer.Start();

		//テリトリーボリュームの計算
		m_volumePoints = new Vector3[m_divisionVolume];

		float angle = 0.0f, divisionAngle = Mathf.PI * 2.0f / m_divisionVolume;

		for (int i = 0; i < m_divisionVolume; ++i)
		{
			angle += divisionAngle;
			m_volumePoints[i] = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
		}
	}
	/// <summary>[Update]</summary>
	void Update()
	{
		int frameCount = Time.frameCount;
		bool isUpdate = false;

		if (m_intervalTimer.elapasedTime >= m_calucrateTerritoryIntervalSeconds)
		{
			isUpdate = true;
			m_intervalTimer.Start();
		}

		//予約があれば計算
		foreach (var e in m_players)
		{
			if (isUpdate & e.Value.usedManager.isCalucrateNextUpdate)
			{
				e.Value.usedManager.isCalucrateNextUpdate = false;
				CalucrateTerritory(e.Value);
			}
			else if (e.Value.usedManager.nextForceCalucrateFrameCount == frameCount)
				CalucrateTerritory(e.Value);
		}
	}


	/// <summary>
	/// [ReserveCalucrateTerritory]
	/// Playerのテリトリー計算を予約する
	/// 引数1: Player object->GetInstanceID()
	/// 引数2: 次フレームで計算？, default = false
	/// </summary>
	public void ReserveCalucrateTerritory(int playerID, bool isForceNextFrame = false)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_players.ContainsKey(playerID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->ReserveCalucrateTerritory\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		if (isForceNextFrame)
			m_players[playerID].usedManager.nextForceCalucrateFrameCount = Time.frameCount + 1;
		else
			m_players[playerID].usedManager.isCalucrateNextUpdate = true;
	}
	/// <summary>
	/// [CalucrateTerritory]
	/// Playerのテリトリーを計算する, 基本使わないこと
	/// 引数1: Player infomation
	/// </summary>
	public void CalucrateTerritory(PlayerInfo playerInfo)
	{
		//リストクリア
		playerInfo.territorialArea.Clear();
		m_grahamResult.Clear();
		
		//ソート用リスト構築
		for (int i = 0, count = playerInfo.allTerritorys.Count; i < count; ++i)
			m_grahamResult.Add(new GrahamScan.CustomFormat(playerInfo.allTerritorys[i].transform.position, i));

		//グラハムソート
		int result = GrahamScan.Run(m_grahamResult);
		if (result < m_grahamResult.Count - 1)
			m_grahamResult.RemoveRange(result, m_grahamResult.Count - result);

		//結果をリストに構築
		for (int i = 0; i < result; ++i)
			playerInfo.borderTerritorys.Add(playerInfo.allTerritorys[m_grahamResult[i].userID]);

		//ボリューム追加した状態でグラハムソート
		for (int i = 0, count = m_grahamResult.Count; i < count; ++i)
		{
			for (int k = 0, length = m_volumePoints.Length; k < length; ++k)
				m_grahamResult.Add(new GrahamScan.CustomFormat(m_grahamResult[i].position + (m_volumePoints[k] * m_radiusVolume), -1));
		}
		result = GrahamScan.Run(m_grahamResult);

		//結果をリストに構築
		for (int i = 0; i < result; ++i)
			playerInfo.territorialArea.Add(m_grahamResult[i].position);
	}


	/// <summary>
	/// [HitCircleTerritory]
	/// PlayerのテリトリーエリアとXZ円で当たり判定を行う
	/// 引数1: Player object->GetInstanceID()
	/// 引数2: Circle position
	/// 引数2: Circle forward direction
	/// 引数3: Circle radius
	/// </summary>
	public bool HitCircleTerritory(int playerID, Vector3 position, Vector3 direction, float radius)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_players.ContainsKey(playerID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->HitCircleTerritory\n ContainsKey(instanceID) == false");
			return false;
		}
#endif

		return CollisionTerritory.HitCircleTerritory(
			m_players[playerID].territorialArea, position, direction, radius, 1000.0f);
	}
	/// <summary>
	/// [RaycastTerritory]
	/// PlayerのテリトリーエリアとXZでRaycast判定を行う
	/// 引数1: Player object->GetInstanceID()
	/// 引数2: Ray position
	/// 引数3: Ray direction
	/// 引数4: Ray distance
	/// 引数5: Ray normal (out)
	/// 引数5: Ray hit point (out)
	/// </summary>
	public bool RaycastTerritory(int playerID, Vector3 position, Vector3 direction, 
		float distance, out Vector3 normal, out Vector3 hitPoint)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_players.ContainsKey(playerID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->GetServant\n ContainsKey(instanceID) == false");
			normal = hitPoint = Vector3.zero;
			return false;
		}
#endif

		return CollisionTerritory.HitRayTerritory(m_players[playerID].territorialArea,
			position, direction, distance, out normal, out hitPoint);
	}


	/// <summary>
	/// [AddPlayer]
	/// Playerを登録する
	/// 引数1: Player object
	/// 引数2: Player PlayerManagerIntermediary
	/// 引数3: Player PlayerMaualCollisionAdministrator
	/// 引数4: Player follow points
	/// 引数5: This main player?, default = true
	/// </summary>
	public void AddPlayer(GameObject player, PlayerManagerIntermediary managerIntermediary,
		PlayerMaualCollisionAdministrator maualCollisionAdministrator, PlayerNavMeshController navMeshController,
		PlayerInput input,
		BoxCastFlags groundFlag, UnityEngine.AI.NavMeshAgent navMeshAgent, Rigidbody rigidBody, 
		GameObject[] followPoints, GameObject resultCameraLookPoint, GameObject resultCameraMovePoint, bool isMainPlayer = true)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (m_players.ContainsKey(player.GetInstanceID()))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->AddPlayer\n ContainsKey(instanceID) == true");
			return;
		}
#endif

		PlayerInfo info = new PlayerInfo(player, managerIntermediary, maualCollisionAdministrator, navMeshController,
			input, groundFlag, navMeshAgent, rigidBody,  followPoints, resultCameraLookPoint, resultCameraMovePoint);

		m_players.Add(player.GetInstanceID(), info);
		if (isMainPlayer)
			mainPlayer = info;

		ServantManager.instance.RegisterPlayer(info.instanceID);
	}
	/// <summary>
	/// [RemovePlayer]
	/// Playerを登録解除する
	/// 引数1: Player object
	/// </summary>
	public void RemovePlayer(GameObject player)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_players.ContainsKey(player.GetInstanceID()))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->RemovePlayer\n ContainsKey(instanceID) == false");
			return;
		}
#endif

		m_players.Remove(player.GetInstanceID());
		if (mainPlayer.gameObject.GetInstanceID() == player.GetInstanceID())
			mainPlayer = null;

		ServantManager.instance.UnregisterPlayer(player.GetInstanceID());
	}
}