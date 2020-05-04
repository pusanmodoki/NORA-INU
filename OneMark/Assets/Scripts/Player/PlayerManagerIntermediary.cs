using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerとManagerを仲介するPlayerManagerIntermediary
/// </summary>
public class PlayerManagerIntermediary : MonoBehaviour
{
	/// <summary>初期オブジェクト名</summary>
	[SerializeField, Tooltip("初期オブジェクト名")]
	string m_firstPointName = "";
	/// <summary>This PlayerMaualCollisionAdministrator</summary>
	[SerializeField, Tooltip("This PlayerMaualCollisionAdministrator")]
	PlayerMaualCollisionAdministrator m_playerMaualCollisionAdministrator = null;
	/// <summary>This GroundFlag</summary>
	[SerializeField, Tooltip("This GroundFlag")]
	BoxCastFlags m_groundFlag = null;
	/// <summary>This NavMeshAgent</summary>
	[SerializeField, Tooltip("This NavMeshAgent")]
	UnityEngine.AI.NavMeshAgent m_navMeshAgent = null;
	/// <summary>Servant follow points</summary>
	[SerializeField, Tooltip("Servant follow points")]
	GameObject[] m_followPoints = new GameObject[3];

	[SerializeField, Space, Tooltip("LineRenderer(とりあえず)")]
	LineRenderer m_lineRenderer = null;

	/// <summary>This game object instance id</summary>
	int m_instanceID = 0;
	/// <summary>初期ポイント</summary>
	BaseMarkPoint m_firstPoint = null;
	/// <summary>初期ポイントがポーズ中か否か</summary>
	bool m_isPauseFirstPoint = false;

	/// <summary>
	/// [ChangeTerritory]
	/// テリトリー変更のコールバック
	/// </summary>
	public void ChangeTerritory()
	{
		if (m_isPauseFirstPoint)
		{
			m_firstPoint.SetLockFirstPoint(false);
			m_firstPoint.isPauseTimer = false;
			m_isPauseFirstPoint = false;

			PlayerAndTerritoryManager.instance.allPlayers[m_firstPoint.linkPlayerID].
				playerInfo.changeTerritoryCallback -= ChangeTerritory;
		}
	}
	
	/// <summary>[Awake]</summary>
	void Awake()
    {
		//Managerにインスタンス追加
		PlayerAndTerritoryManager.instance.AddPlayer(gameObject, this, 
			m_playerMaualCollisionAdministrator, m_groundFlag, m_navMeshAgent, m_followPoints);
		//初期ポイントを探す
		GameObject firstPoint = GameObject.Find(m_firstPointName);

		m_instanceID = gameObject.GetInstanceID();

		//マークポイントを取得
		if (firstPoint != null)
			m_firstPoint = firstPoint.GetComponent<BaseMarkPoint>();

		//初期ポイントが見つかった
		if (m_firstPoint != null)
		{
			//PlayerInfo取得
			var playerInfo = PlayerAndTerritoryManager.instance.allPlayers[m_instanceID].playerInfo;
			//コールバック追加
			playerInfo.changeTerritoryCallback += ChangeTerritory;
			//ポイントをリンクさせる
			m_firstPoint.LinkPlayer(gameObject, null);
			//即時計算要請
			PlayerAndTerritoryManager.instance.CalucrateTerritory(playerInfo);
			//Info設定
			m_playerMaualCollisionAdministrator.SetPlayerInfo(playerInfo);

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
	void Update()
	{
		var playerInfo = PlayerAndTerritoryManager.instance.allPlayers[m_instanceID].playerInfo;

		if (m_lineRenderer != null)
		{
			if (playerInfo.territorialArea.Count > 1)
			{
				m_lineRenderer.positionCount = playerInfo.territorialArea.Count + 1;
				for (int i = 0; i < playerInfo.territorialArea.Count; ++i)
					m_lineRenderer.SetPosition(i, playerInfo.territorialArea[i]);
				m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, playerInfo.territorialArea[0]);
			}
			else
			{
				m_lineRenderer.positionCount = 0;
			}
		}
	}
}
