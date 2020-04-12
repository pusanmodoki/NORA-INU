﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AIComponent;

/// <summary>
/// AIAgentをCustomしたDogAIAgent
/// </summary>
public class DogAIAgent : AIAgent
{
	/// <summary>Speed changer</summary>
	public DogSpeedChanger speedChanger { get { return m_speedChanger; } }
	/// <summary>Link player</summary>
	public GameObject linkPlayer { get; private set; } = null;
	/// <summary>Link mark point</summary>
	public BaseMarkPoint linkMarkPoint { get; private set; } = null;
	/// <summary>Is sit & stay now?</summary>
	public bool isSitAndStaySelf { get; private set; } = false;
	/// <summary>Is linked player?</summary>
	public bool isLinkPlayer { get { return linkPlayer != null; } }
	/// <summary>Is linked mark point?</summary>
	public bool isLinkMarkPoint { get { return linkMarkPoint != null; } }

	/// <summary>初期登録を行う場合のPlayer name</summary>
	[Header("Dog AI Agent Members"), Space, SerializeField, Tooltip("初期登録を行う場合のPlayer name")]
	string m_findPlayerObjectName = "";
	/// <summary>This DogRushingAndMarking</summary>
	[SerializeField, Tooltip("This DogRushingAndMarking")]
	DogRushingAndMarking m_rushingAndMarkingFunction = null;
	/// <summary>プレイヤーの命令に従うかどうか確認するRaycastのLayerMask</summary>
	[SerializeField, Tooltip("プレイヤーの命令に従うかどうか確認するRaycastのLayerMask")]
	LayerMaskEx m_playerObeyLayerMask = 0;
	/// <summary>OffMeshLink controller</summary>
	[SerializeField, Space, Tooltip("OffMeshLink controller")]
	DogOffMeshLinkController m_offMeshLinkController = new DogOffMeshLinkController();
	/// <summary>Speed changer</summary>
	[SerializeField, Space, Tooltip("Speed changer")]
	DogSpeedChanger m_speedChanger = new DogSpeedChanger();

	/// <summary>RaycastHit</summary>
	RaycastHit m_raycastHit = new RaycastHit();

	/// <summary>
	/// [GoSoStartOfMarking]
	/// マーキング開始命令を指示する
	/// return: 行動実行する場合はtrue, できない場合はfalse
	/// 引数1: 目標マークポイント
	/// </summary>
	public bool GoSoStartOfMarking(BaseMarkPoint markPoint)
	{
		//念の為nullチェック
		if (m_rushingAndMarkingFunction == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->GoSoStartOfMarking\n This Rushing And Marking Function == null");
#endif
			return false;
		}
		else if (markPoint == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->GoSoStartOfMarking\n markPoint == null");
#endif
			return false;
		}
		else if (isLinkMarkPoint || 
			m_rushingAndMarkingFunction.functionState != DogRushingAndMarking.State.Null)
			return false;

		Vector3 position = transform.position, targetPosition = linkPlayer.transform.position;
		Vector3 direction = (new Vector3(targetPosition.x, 0.0f, targetPosition.z) -
			new Vector3(position.x, 0.0f, position.z)).normalized;

		//Raycast判定
		if (linkPlayer != null &&
			Physics.Raycast(position, direction, out m_raycastHit, 10000.0f, m_playerObeyLayerMask) &&
			m_raycastHit.transform.gameObject.GetInstanceID() == linkPlayer.GetInstanceID())
		{
			//事前情報入力
			m_rushingAndMarkingFunction.SetAdvanceInformation(markPoint, markPoint.transform.position);
			//関数割り込み実行
			ForceSpecifyFunction(m_rushingAndMarkingFunction);

			return true;
		}
		else
		{
			return false;
		}
	}
	/// <summary>
	/// [ComeBecauseEndOfMarking]
	/// マーキング->Stay命令を終了する
	/// return: 行動実行する場合はtrue, できない場合はfalse
	/// </summary>
	public bool ComeBecauseEndOfMarking()
	{
		//念の為nullチェック
		if (m_rushingAndMarkingFunction == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->ComeBecauseEndOfMarking\n Rushing And Marking Function == null");
#endif
			return false;
		}
		else if (!isSitAndStaySelf)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->ComeBecauseEndOfMarking\n GoSoStartOfMarking is not running.");
#endif
			return false;
		}
		else if (!isLinkMarkPoint ||
			m_rushingAndMarkingFunction.functionState != DogRushingAndMarking.State.Null)
			return false;

		Vector3 position = transform.position, targetPosition = linkPlayer.transform.position;
		Vector3 direction = (new Vector3(targetPosition.x, 0.0f, targetPosition.z) - 
			new Vector3(position.x, 0.0f, position.z)).normalized;

		//Raycast判定
		if (linkPlayer != null &&
			Physics.Raycast(position, direction, out m_raycastHit, 10000.0f, m_playerObeyLayerMask) &&
			m_raycastHit.transform.gameObject.GetInstanceID() == linkPlayer.GetInstanceID())
		{
			//ステイ終了
			SetSitAndStay(false, linkMarkPoint);

			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// [SetObeyPlayer]
	/// Playerとの主従関係を登録する
	/// 引数1: set value
	/// </summary>
	public void SetObeyPlayer(GameObject playerObject)
	{
		if (playerObject != null
			&& PlayerAndTerritoryManager.instance.allPlayers.ContainsKey(playerObject.GetInstanceID()))
		{
			linkPlayer = playerObject;
			ServantManager.instance.RegisterPlayerOfServant(this, playerObject);
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->SetObeyPlayer\n Not register or null player object");
#endif
		}
	}
	/// <summary>
	/// [SetSitAndStay]
	/// おすわり！待て！を設定する
	/// 引数1: Set value
	/// 引数2: Link mark point
	/// </summary>
	public void SetSitAndStay(bool isSet, BaseMarkPoint markPoint)
	{
		if (markPoint == null || isSet == isSitAndStaySelf)
			return;

		isSitAndStaySelf = isSet;
		if (isSet)
			linkMarkPoint = markPoint;
		else
		{
			linkMarkPoint = null;
			markPoint.isPauseTimer = false;
		}
	}


	/// <summary>[Start]</summary>
	new void Start()
	{
		base.Start();
		speedChanger.Start();
		
		ServantManager.instance.AddServant(this);

		GameObject player = GameObject.Find(m_findPlayerObjectName);
		if (player != null)
			SetObeyPlayer(player);
		else
		{
#if UNITY_EDITOR
			Debug.Log("DogAIAgent: Player is null when instantiate. instance id->" + aiAgentInstanceID);
#endif
		}

		m_offMeshLinkController.InitContoroller(this);
	}
	/// <summary>[Update]</summary>
	new void Update()
	{
		m_offMeshLinkController.Update();
		base.Update();
		speedChanger.Update();
	}
	/// <summary>[FixedUpdate]</summary>
	new void FixedUpdate()
	{
		m_offMeshLinkController.FixedUpdate();
		base.FixedUpdate();
	}
	/// <summary>[OnDestroy]</summary>
	void OnDestroy()
	{
		if (ServantManager.instance != null)
		{
			ServantManager.instance.UnregisterPlayerOfServant(this, linkPlayer);
			ServantManager.instance.RemoveServant(this);
		}
	}
}
