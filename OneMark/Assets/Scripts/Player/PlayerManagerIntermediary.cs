using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerとManagerを仲介するPlayerManagerIntermediary
/// </summary>
public class PlayerManagerIntermediary : MonoBehaviour
{
	/// <summary>This PlayerInfo</summary>
	public PlayerAndTerritoryManager.PlayerInfo thisInfo { get; private set; } = null;

	/// <summary>This PlayerMaualCollisionAdministrator</summary>
	[SerializeField, Tooltip("This PlayerMaualCollisionAdministrator")]
	PlayerMaualCollisionAdministrator m_playerMaualCollisionAdministrator = null;
	/// <summary>This PlayerNavMeshController</summary>
	[SerializeField, Tooltip("This PlayerNavMeshController")]
	PlayerNavMeshController m_navMeshController = null;
	/// <summary>This PlayerInput</summary>
	[SerializeField, Tooltip("This PlayerInput")]
	PlayerInput m_input = null;
	/// <summary>This GroundFlag</summary>
	[SerializeField, Tooltip("This GroundFlag")]
	BoxCastFlags m_groundFlag = null;
	/// <summary>This AreaMesh</summary>
	[SerializeField, Tooltip("This AreaMesh")]
	AreaMesh m_areaMesh = null;
	/// <summary>This AreaBorderMesh</summary>
	[SerializeField, Tooltip("This AreaBorderMesh")]
	AreaBorderMesh m_areaBorderMesh = null;
	/// <summary>This NavMeshAgent</summary>
	[SerializeField, Tooltip("This NavMeshAgent")]
	UnityEngine.AI.NavMeshAgent m_navMeshAgent = null;
	/// <summary>This NavMeshAgent</summary>
	[SerializeField, Tooltip("This Rigidbody")]
	Rigidbody m_rigidBody = null;
	/// <summary>Servant follow points</summary>
	[SerializeField, Tooltip("Servant follow points")]
	GameObject[] m_followPoints = new GameObject[3];

	/// <summary>Result camera look point</summary>
	[SerializeField, Space, Tooltip("Result camera look point")]
	GameObject m_resultCameraLookPoint = null;
	/// <summary>Result camera move point</summary>
	[SerializeField, Tooltip("Result camera move point")]
	GameObject m_resultCameraMovePoint = null;

	//[SerializeField, Space, Tooltip("LineRenderer(とりあえず)")]
	//LineRenderer m_lineRenderer = null;
	//[SerializeField, Space, Tooltip("LineRenderer(とりあえず)")]
	//LineRenderer m_lineRenderer1 = null;

	/// <summary>This game object instance id</summary>
	int m_instanceID = 0;
	/// <summary>初期ポイント</summary>
	BaseMarkPoint m_firstPoint = null;
	/// <summary>初期ポイントがポーズ中か否か</summary>
	bool m_isPauseFirstPoint = false;

	public void GameEnd(bool isGameClear)
	{
		if (isGameClear) m_input.GameClearAnimation();
		else m_input.GameOverAnimation();
		m_areaBorderMesh.meshRenderer.enabled = false;
		m_playerMaualCollisionAdministrator.targetMarker.DisableMarker();
	}

	/// <summary>
	/// [ChangeTerritory]
	/// テリトリー変更のコールバック
	/// </summary>
	public void ChangeTerritory(bool isAdd)
	{
		if (m_isPauseFirstPoint & isAdd)
		{
			m_firstPoint.SetLockFirstPoint(false);
			m_isPauseFirstPoint = false;

			PlayerAndTerritoryManager.instance.allPlayers[m_firstPoint.linkPlayerID].
				changeTerritoryCallback -= ChangeTerritory;
		}
	}
	
	/// <summary>[Awake]</summary>
	void Awake()
    {
		//Managerにインスタンス追加
		PlayerAndTerritoryManager.instance.AddPlayer(gameObject, this, m_playerMaualCollisionAdministrator, 
			m_navMeshController, m_input, m_groundFlag, m_navMeshAgent, m_rigidBody, m_areaMesh, 
			m_areaBorderMesh, m_followPoints, m_resultCameraLookPoint, m_resultCameraMovePoint);

		//現在のステージ情報を取得
		var settings = DataManager.instance.nowStageSettings;

		transform.position = settings.playerPosition;
		transform.rotation = settings.playerRotation;
		m_instanceID = gameObject.GetInstanceID();

		//初期ポイントを探す
		GameObject firstPointObject = GameObject.Find(settings.firstPointName);

		//マークポイントを取得
		if (firstPointObject != null)
			m_firstPoint = firstPointObject.GetComponent<BaseMarkPoint>();

		//初期ポイントが見つかった
		if (m_firstPoint != null)
		{
			//PlayerInfo取得
			var playerInfo = PlayerAndTerritoryManager.instance.allPlayers[m_instanceID];
			//コールバック追加
			playerInfo.changeTerritoryCallback += ChangeTerritory;
			//ポイントをリンクさせる
			m_firstPoint.LinkPlayer(gameObject, null);
			//即時計算要請
			PlayerAndTerritoryManager.instance.CalucrateTerritory(playerInfo);
			PlayerAndTerritoryManager.instance.CalucrateSafetyTerritory(playerInfo);
			//Info設定
			m_playerMaualCollisionAdministrator.SetPlayerInfo(playerInfo);
			thisInfo = playerInfo;

			m_firstPoint.SetLockFirstPoint(true);
			m_isPauseFirstPoint = true;
		}
		else
		{
			//Debug only, エラーログを表示
#if UNITY_EDITOR
			Debug.LogError("Error!! PlayerManagerIntermediary->Start\n FirstPoint not found");
#endif
		}
	}

	/// <summary>
	/// [Update]
	/// とりあえずの処理たち
	/// </summary>
	//void Update()
	//{
		//var playerInfo = PlayerAndTerritoryManager.instance.allPlayers[m_instanceID];

		//if (m_lineRenderer != null)
		//{
		//	if (playerInfo.territorialArea.Count > 1)
		//	{
		//		m_lineRenderer.positionCount = playerInfo.territorialArea.Count + 1;
		//		for (int i = 0; i < playerInfo.territorialArea.Count; ++i)
		//			m_lineRenderer.SetPosition(i, playerInfo.territorialArea[i]);
		//		m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, playerInfo.territorialArea[0]);
		//	}
		//	else
		//	{
		//		m_lineRenderer.positionCount = 0;
		//	}
		//}
		//if (m_lineRenderer1 != null)
		//{
		//	if (playerInfo.safetyTerritorialArea.Count > 1)
		//	{
		//		m_lineRenderer1.positionCount = playerInfo.safetyTerritorialArea.Count + 1;
		//		for (int i = 0; i < playerInfo.safetyTerritorialArea.Count; ++i)
		//			m_lineRenderer1.SetPosition(i, playerInfo.safetyTerritorialArea[i]);
		//		m_lineRenderer1.SetPosition(m_lineRenderer1.positionCount - 1, playerInfo.safetyTerritorialArea[0]);
		//	}
		//	else
		//	{
		//		m_lineRenderer1.positionCount = 0;
		//	}
		//}
	//}
}
