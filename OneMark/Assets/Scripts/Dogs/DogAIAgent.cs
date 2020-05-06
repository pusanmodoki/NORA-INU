using System.Collections;
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
	/// <summary>Link mark point</summary>
	public DogAnimationController animationController { get { return m_animationController; } } 
	/// <summary>Link player</summary>
	public GameObject linkPlayer { get; private set; } = null;
	/// <summary>Link mark point</summary>
	public BaseMarkPoint linkMarkPoint { get; private set; } = null;
	/// <summary>Player別の自身のIndex</summary>
	public int linkPlayerServantsOwnIndex { get { return m_linkPlayerServantsOwnIndex; } }
	/// <summary>Is sit & stay now?</summary>
	public bool isSitAndStaySelf { get; private set; } = false;
	/// <summary>Playerに同行中？</summary>
	public bool isAccompanyingPlayer { get {
			return isLinkPlayer & !isSitAndStaySelf &
				m_rushingAndMarkingFunction.functionState == DogRushingAndMarking.State.Null; } }
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
	/// <summary>This animation controller</summary>
	[SerializeField, Tooltip("This animation controller")]
	DogAnimationController m_animationController = null;
	/// <summary>プレイヤーの命令に従うかどうかプレイヤーとエネミーでRaycastする時のLayerMask</summary>
	[SerializeField, Tooltip("プレイヤーの命令に従うかどうかプレイヤーとエネミーでRaycastする時のLayerMask")]
	LayerMaskEx m_playerObeyLayerMask = 0;
	/// <summary>プレイヤーとマークポイントでRaycastする時のLayerMask</summary>
	[SerializeField, Tooltip("プレイヤーとマークポイントでRaycastする時のLayerMask")]
	LayerMaskEx m_playerObeyMarkPointLayerMask = 0;
	/// <summary>OffMeshLink controller</summary>
	[SerializeField, Space, Tooltip("OffMeshLink controller")]
	DogOffMeshLinkController m_offMeshLinkController = new DogOffMeshLinkController();
	/// <summary>Speed changer</summary>
	[SerializeField, Space, Tooltip("Speed changer")]
	DogSpeedChanger m_speedChanger = new DogSpeedChanger();

	/// <summary>RaycastHits</summary>
	RaycastHit[] m_raycastHits = null;
	/// <summary>Colliders</summary>
	Collider[] m_colliders = null;
	/// <summary>Player別の自身のIndex</summary>
	int m_linkPlayerServantsOwnIndex = -1;

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
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->GoSoStartOfMarking\n Already running");
#endif
			return false;
		}

		Vector3 position = transform.position, targetPosition = linkPlayer.transform.position;
		Vector3 direction = (new Vector3(targetPosition.x, 0.0f, targetPosition.z) -
			new Vector3(position.x, 0.0f, position.z)).normalized;
		float distance = (targetPosition - position).sqrMagnitude;
		int instanceID = linkPlayer.GetInstanceID();

		//Raycast, Overlap判定(Player)
		m_raycastHits = Physics.RaycastAll(position, direction, distance, m_playerObeyLayerMask);
		m_colliders = Physics.OverlapSphere(position, distance, m_playerObeyLayerMask);

		if (linkPlayer != null
			&& (m_raycastHits.ContainsInstanceID(instanceID) | m_colliders.ContainsInstanceID(instanceID)))
		{
			position = targetPosition;
			targetPosition = markPoint.transform.position;

			direction = (new Vector3(targetPosition.x, 0.0f, targetPosition.z) -
				new Vector3(position.x, 0.0f, position.z)).normalized;
			distance = (targetPosition - position).sqrMagnitude;
			instanceID = markPoint.gameObject.GetInstanceID();

			//Raycast, Overlap判定(MarkPoint)
			m_raycastHits = Physics.RaycastAll(position, direction, distance, m_playerObeyMarkPointLayerMask);
			m_colliders = Physics.OverlapSphere(position, distance, m_playerObeyMarkPointLayerMask);
			if (m_raycastHits.ContainsInstanceID(instanceID) | m_colliders.ContainsInstanceID(instanceID))
			{
				//事前情報入力
				m_rushingAndMarkingFunction.SetAdvanceInformation(markPoint, markPoint.transform.position);
				//関数割り込み実行
				ForceSpecifyFunction(m_rushingAndMarkingFunction);

				return true;
			}
			else return false;
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
			Debug.Log("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n Rushing And Marking Function == null");
#endif
			return false;
		}
		else if (!isSitAndStaySelf)
		{
#if UNITY_EDITOR
			Debug.Log("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n GoSoStartOfMarking is not running.");
#endif
			return false;
		}
		else if (!isLinkMarkPoint ||
			m_rushingAndMarkingFunction.functionState != DogRushingAndMarking.State.Null)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->GoSoStartOfMarking\n GoSoStartOfMarking already running");
#endif
			return false;
		}

		Vector3 position = transform.position, targetPosition = linkPlayer.transform.position;
		Vector3 direction = (new Vector3(targetPosition.x, 0.0f, targetPosition.z) - 
			new Vector3(position.x, 0.0f, position.z)).normalized;

		//Raycast判定(Player)
		m_raycastHits = Physics.RaycastAll(position, direction, (targetPosition - position).sqrMagnitude, m_playerObeyLayerMask);
		if (linkPlayer != null && m_raycastHits.ContainsInstanceID(linkPlayer))
		{
			//Animation Set
			m_animationController.editAnimation.SetTriggerWakeUp();
			m_animationController.editAnimation.SetBoolIsNextSearch(false);
			return true;
		}
		else
		{
			//Animation Set
			m_animationController.editAnimation.SetTriggerWakeUp();
			m_animationController.editAnimation.SetBoolIsNextSearch(true);
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
			ServantManager.instance.RegisterPlayerOfServant(this, playerObject, out m_linkPlayerServantsOwnIndex);
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
		{
			//強制的な増加を設定
			markPoint.SetForceAscendingEffective(true);

			linkMarkPoint = markPoint;
			navMeshAgent.updatePosition = false;
			navMeshAgent.updateRotation = false;
		}
		else
		{
			//強制的な増加を解除
			markPoint.SetForceAscendingEffective(false);

			linkMarkPoint = null;
			navMeshAgent.updatePosition = true;
			navMeshAgent.updateRotation = true;
			navMeshAgent.Warp(transform.position);
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
