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
		/// <summary>[コンストラクタ]</summary>
		public PlayerInfo(GameObject gameObject, PlayerTerritoryIntermediary territoryIntermediary,
		PlayerMaualCollisionAdministrator maualCollisionAdministrator, GameObject[] followPoints)
		{
			this.gameObject = gameObject;
			this.territoryIntermediary = territoryIntermediary;
			this.maualCollisionAdministrator = maualCollisionAdministrator;
			this.followPoints = followPoints;
			instanceID = gameObject.GetInstanceID();
		}

		/// <summary>Player game object</summary>
		public GameObject gameObject { get; private set; }
		/// <summary>Player Player territory intermediary</summary>
		public PlayerTerritoryIntermediary territoryIntermediary { get; private set; }
		/// <summary>Player Player maual collision administrator</summary>
		public PlayerMaualCollisionAdministrator maualCollisionAdministrator { get; private set; }
		/// <summary>Player instance id</summary>
		public int instanceID { get; private set; }
		/// <summary>All territory mark points (クラス)</summary>
		public List<BaseMarkPoint> allTerritorys { get; private set; } = new List<BaseMarkPoint>();
		/// <summary>外周のterritory mark points (クラス)</summary>
		public List<BaseMarkPoint> borderTerritorys { get; private set; } = new List<BaseMarkPoint>();
		/// <summary>All territory mark points (ポイント)</summary>
		public List<Vector3> allTerritoryPoints { get; private set; } = new List<Vector3>();
		/// <summary>外周のterritory mark points (ポイント)</summary>
		public List<Vector3> borderTerritoryPoints { get; private set; } = new List<Vector3>();
		/// <summary>ボリュームを加えたテリトリー圏 (ポジション)</summary>
		public List<Vector3> territorialArea { get; private set; } = new List<Vector3>();
		/// <summary>Follow point game objects</summary>
		public GameObject[] followPoints { get; private set; } = null;
		/// <summary>テリトリー変更時にCallされるコールバック (計算時ではなく, AddMarkPoint, RemoveMarkPoint実行でCall)</summary>
		public ChangeTerritoryCallback changeTerritoryCallback { get; set; } = null;

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

	/// <summary>
	/// 自クラス内でPlayerInfoを保存するStoragePlayer
	/// </summary>
	public class StoragePlayer
	{
		/// <summary>[コンストラクタ]</summary>
		public StoragePlayer(GameObject player, PlayerTerritoryIntermediary territoryIntermediary,
		PlayerMaualCollisionAdministrator maualCollisionAdministrator, GameObject[] followPoints)
		{
			playerInfo = new PlayerInfo(player, territoryIntermediary, maualCollisionAdministrator, followPoints);
			nextCalucrateFrameCount = 0;
			isCalucrateNextUpdate = false;
		}

		/// <summary>Player info</summary>
		public PlayerInfo playerInfo;
		/// <summary>次回テリトリー計算するフレーム</summary>
		public int nextCalucrateFrameCount;
		/// <summary>次回の通常更新で計算する？</summary>
		public bool isCalucrateNextUpdate;
	}


	/// <summary>Static instance</summary>
	public static PlayerAndTerritoryManager instance { get; private set; } = null;

	/// <summary>All players</summary>
	public ReadOnlyDictionary<int, StoragePlayer> allPlayers { get; private set; } = null;
	/// <summary>Main player</summary>
	public PlayerInfo mainPlayer { get; private set; } = null;

	/// <summary>ボリューム円の分割数</summary>
	[SerializeField, Range(0, 100), Tooltip("ボリューム円の分割数")]
	int m_divisionVolume = 10;
	/// <summary>ボリューム円の半径</summary>
	[SerializeField, Range(0, 10), Tooltip("ボリューム円の半径")]
	float m_radiusVolume = 3.0f;
	/// <summary>テリトリー計算間隔</summary>
	[SerializeField, Range(0, 2), Tooltip("テリトリー計算間隔")]
	float m_calucrateTerritoryInterval = 0.2f;

	/// <summary>Manage players</summary>
	Dictionary<int, StoragePlayer> m_players = null;
	/// <summary>Grahamソート用リスト(効率化のためメンバに)</summary>
	List<GrahamScan.CustomFormat> m_grahamResult = new List<GrahamScan.CustomFormat>();
	/// <summary>ボリューム座標</summary>
	Vector3[] m_volumePoints = null;
	/// <summary>計算間隔用タイマー</summary>
	Timer m_intervalTimer = new Timer();


	/// <summary>[Awake]</summary>
	void Awake()
	{
		//インスタンス登録
		instance = this;
		m_players = new Dictionary<int, StoragePlayer>();
		allPlayers = new ReadOnlyDictionary<int, StoragePlayer>(m_players);

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

		if (m_intervalTimer.elapasedTime >= m_calucrateTerritoryInterval)
		{
			isUpdate = true;
			m_intervalTimer.Start();
		}

		//予約があれば計算
		foreach (var e in m_players)
		{
			if (isUpdate & e.Value.isCalucrateNextUpdate)
			{
				e.Value.isCalucrateNextUpdate = false;
				CalucrateTerritory(e.Value.playerInfo);
			}
			else if (e.Value.nextCalucrateFrameCount == frameCount)
				CalucrateTerritory(e.Value.playerInfo);
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
			m_players[playerID].nextCalucrateFrameCount = Time.frameCount + 1;
		else
			m_players[playerID].isCalucrateNextUpdate = true;
	}
	/// <summary>
	/// [CalucrateTerritory]
	/// Playerのテリトリーを計算する, 基本使わないこと
	/// 引数1: Player infomation
	/// </summary>
	public void CalucrateTerritory(PlayerInfo playerInfo)
	{
		//リストクリア
		playerInfo.allTerritoryPoints.Clear();
		playerInfo.borderTerritorys.Clear();
		playerInfo.borderTerritoryPoints.Clear();
		playerInfo.territorialArea.Clear();
		m_grahamResult.Clear();
		//ポジション記録, ソート用リスト構築
		for (int i = 0, count = playerInfo.allTerritorys.Count; i < count; ++i)
		{
			playerInfo.allTerritoryPoints.Add(playerInfo.allTerritorys[i].transform.position);
			m_grahamResult.Add(new GrahamScan.CustomFormat(playerInfo.allTerritoryPoints[i], i));
		}

		//グラハムソート
		int result = GrahamScan.Run(m_grahamResult);
		if (result < m_grahamResult.Count - 1)
			m_grahamResult.RemoveRange(result, m_grahamResult.Count - result);

		//結果を各リストに構築
		for (int i = 0; i < result; ++i)
		{
			playerInfo.borderTerritorys.Add(playerInfo.allTerritorys[m_grahamResult[i].userID]);
			playerInfo.borderTerritoryPoints.Add(playerInfo.allTerritoryPoints[m_grahamResult[i].userID]);
		}

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
	/// 引数3: Circle radius
	/// </summary>
	public bool HitCircleTerritory(int playerID, Vector3 position, float radius)
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
			m_players[playerID].playerInfo.territorialArea, position, 100.0f, -radius);
	}
	/// <summary>
	/// [RaycastTerritory]
	/// PlayerのテリトリーエリアとXZでRaycast判定を行う
	/// 引数1: Player object->GetInstanceID()
	/// 引数2: Ray position
	/// 引数3: Ray direction
	/// 引数4: Ray distance
	/// 引数5: Ray normal (out)
	/// </summary>
	public bool RaycastTerritory(int playerID, Vector3 position, Vector3 direction, float distance, out Vector3 normal)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (!m_players.ContainsKey(playerID))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->GetServant\n ContainsKey(instanceID) == false");
			normal = Vector3.zero;
			return false;
		}
#endif

		return CollisionTerritory.HitRayTerritory(m_players[playerID].playerInfo.territorialArea,
			position, direction, distance, out normal);
	}


	/// <summary>
	/// [AddPlayer]
	/// Playerを登録する
	/// 引数1: Player object
	/// 引数2: Player PlayerTerritoryIntermediary
	/// 引数3: Player PlayerMaualCollisionAdministrator
	/// 引数4: Player follow points
	/// 引数5: This main player?, default = true
	/// </summary>
	public void AddPlayer(GameObject player, PlayerTerritoryIntermediary territoryIntermediary,
		PlayerMaualCollisionAdministrator maualCollisionAdministrator, GameObject[] followPoints, bool isMainPlayer = true)
	{
		//debug only, invalid key対策
#if UNITY_EDITOR
		if (m_players.ContainsKey(player.GetInstanceID()))
		{
			Debug.LogError("Error!! PlayerAndTerritoryManager->AddPlayer\n ContainsKey(instanceID) == true");
			return;
		}
#endif

		StoragePlayer info = new StoragePlayer(player, territoryIntermediary, maualCollisionAdministrator, followPoints);

		m_players.Add(player.GetInstanceID(), info);
		if (isMainPlayer)
			mainPlayer = info.playerInfo;

		ServantManager.instance.RegisterPlayer(info.playerInfo.instanceID);
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