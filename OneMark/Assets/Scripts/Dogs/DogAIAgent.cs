using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// <summary>Ground flag</summary>
	public BoxCastFlags groundFlag { get { return m_groundFlag; } } 
	/// <summary>Link player</summary>
	public GameObject linkPlayer { get; private set; } = null;
	/// <summary>Link player</summary>
	public PlayerAndTerritoryManager.PlayerInfo linkPlayerInfo
	{
		get { return linkPlayer != null ? PlayerAndTerritoryManager.instance.allPlayers[linkPlayer.GetInstanceID()] : null; }
	}
	/// <summary>Link mark point</summary>
	public BaseMarkPoint linkMarkPoint { get; private set; } = null;
	/// <summary>GoMarking時の円形当たり判定で使うCenter</summary>
	public Vector3 indicatedDogCenter { get { return m_indicatedDogCenter; } }
	/// <summary>GoMarking時の円形当たり判定で使う半径</summary>
	public float indicatedDogRadius { get { return m_indicatedDogRadius; } }
	/// <summary>Player別の自身のIndex</summary>
	public int linkPlayerServantsOwnIndex { get { return m_linkPlayerServantsOwnIndex; } }
	/// <summary>Is wait & run now?</summary>
	public bool isWaitAndRunSelf { get; private set; } = false;
	/// <summary>Playerに同行中？</summary>
	public bool isAccompanyingPlayer { get {
			return isLinkPlayer & !isWaitAndRunSelf &
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
	/// <summary>This ground flag</summary>
	[SerializeField, Tooltip("This ground flag")]
	BoxCastFlags m_groundFlag = null;
	/// <summary>GoMarking時の円形当たり判定で使うCenter</summary>
	[SerializeField, Tooltip("GoMarking時の円形当たり判定で使うCenter")]
	Vector3 m_indicatedDogCenter = Vector3.zero;
	/// <summary>GoMarking時の円形当たり判定で使う半径</summary>
	[SerializeField, Tooltip("GoMarking時の円形当たり判定で使う半径")]
	float m_indicatedDogRadius = 2;
	/// <summary>OffMeshLink controller</summary>
	[SerializeField, Space, Tooltip("OffMeshLink controller")]
	DogOffMeshLinkController m_offMeshLinkController = new DogOffMeshLinkController();
	/// <summary>Speed changer</summary>
	[SerializeField, Space, Tooltip("Speed changer")]
	DogSpeedChanger m_speedChanger = new DogSpeedChanger();


	//Debug only
#if UNITY_EDITOR
	/// <summary>isSitAndStay</summary>
	[Header("Dog AI Agent Debug Only"), Tooltip("isSitAndStay"), Space, SerializeField]
	bool m_dIsWaitAndRun = false;
#endif
	
	/// <summary>Link mark point</summary>
	BaseMarkPoint m_linkMarkPoint = null;
	/// <summary>Player別の自身のIndex</summary>
	int m_linkPlayerServantsOwnIndex = -1;
	/// <summary>GoSoStartOfMarkingが実行されたがOffMeshLink上にいる場合は予約される</summary>
	bool m_isReservationStartOfMarking = false;
	/// <summary>ComeBecauseEndOfMarkingが実行されたがOffMeshLink上にいる場合は予約される</summary>
	bool m_isReservationEndOfMarking = false;
	/// <summary>m_isReservationEndOfMarking時に保存されるSetBoolIsNextSearch</summary>
	bool m_isSetBoolIsNextSearch = false;

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
		if (linkPlayer == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n Link player == null");
#endif
			return false;
		}
		else if (markPoint == null)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogAIAgent->GoSoStartOfMarking: markPoint == null");
#endif
			return false;
		}
		else if (isLinkMarkPoint || 
			m_rushingAndMarkingFunction.functionState != DogRushingAndMarking.State.Null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Error!! DogAIAgent->GoSoStartOfMarking: Already running");
#endif
			return false;
		}

		//事前情報入力
		m_rushingAndMarkingFunction.SetAdvanceInformation(markPoint);
		m_isReservationStartOfMarking = false;
		m_linkMarkPoint = markPoint;

		//OffMeshLink上なら予約
		if (navMeshAgent.isOnOffMeshLink)
			m_isReservationStartOfMarking = true;
		//関数割り込み実行
		else
		{
			m_linkMarkPoint.ChangeAgent(this);
			ForceSpecifyFunction(m_rushingAndMarkingFunction);
		}
		return true;
	}
	/// <summary>
	/// [ComeBecauseEndOfMarking]
	/// マーキング->Stay命令を終了する
	/// return: 行動実行する場合はtrue, できない場合はfalse
	/// </summary>
	public bool ComeBecauseEndOfMarking(bool isBoxCastHit)
	{
		//念の為nullチェック
		if (m_rushingAndMarkingFunction == null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n Rushing And Marking Function == null");
#endif
			return false;
		}
		if (linkPlayer == null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n Link player == null");
#endif
			return false;
		}
		else if (!isWaitAndRunSelf)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Warning!! DogAIAgent->ComeBecauseEndOfMarking\n GoSoStartOfMarking is not running.");
#endif
			return false;
		}
		else if (!isLinkMarkPoint ||
			m_rushingAndMarkingFunction.functionState != DogRushingAndMarking.State.Null)
		{
#if UNITY_EDITOR
			Debug.LogWarning("Error!! DogAIAgent->GoSoStartOfMarking\n GoSoStartOfMarking already running");
#endif
			return false;
		}

		m_isReservationEndOfMarking = false;

		//Hit
		if (isBoxCastHit)
		{
			//OffMeshLink上なら予約
			if (navMeshAgent.isOnOffMeshLink)
			{
				m_isReservationEndOfMarking = true;
				m_isSetBoolIsNextSearch = false;
			}
			else
			{
				//Animation Set
				m_animationController.editAnimation.TriggerReturnWakeUp();
				m_animationController.editAnimation.isWakeUpNextSearch = false;
				m_linkMarkPoint.ChangeAgent(null);
				m_linkMarkPoint = null;
			}
			return true;
		}
		else
		{
			//OffMeshLink上なら予約
			if (navMeshAgent.isOnOffMeshLink)
			{
				m_isReservationEndOfMarking = true;
				m_isSetBoolIsNextSearch = true;
			}
			else
			{
				//Animation Set
				m_animationController.editAnimation.TriggerReturnWakeUp();
				m_animationController.editAnimation.isWakeUpNextSearch = true;
				m_linkMarkPoint.ChangeAgent(null);
				m_linkMarkPoint = null;
			}
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
	/// [SetWaitAndRun]
	/// 待て！周りを走れ！を設定する
	/// 引数1: Set value
	/// 引数2: Link mark point
	/// </summary>
	public void SetWaitAndRun(bool isSet, BaseMarkPoint markPoint)
	{
		if (markPoint == null || isSet == isWaitAndRunSelf)
			return;

		isWaitAndRunSelf = isSet;
		if (isSet)
		{
			//強制的な増加を設定
			markPoint.SetForceAscendingEffective(true);

			linkMarkPoint = markPoint;
			//navMeshAgent.updatePosition = false;
			//navMeshAgent.updateRotation = false;
		}
		else
		{
			//強制的な増加を解除
			markPoint.SetForceAscendingEffective(false);

			linkMarkPoint = null;
			//navMeshAgent.updatePosition = true;
			//navMeshAgent.updateRotation = true;
			//navMeshAgent.Warp(transform.position);
		}


	//Debug only
#if UNITY_EDITOR
	m_dIsWaitAndRun = isSet;
#endif
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
		//予約確認
		if (m_isReservationStartOfMarking && !navMeshAgent.isOnOffMeshLink)
		{
			m_isReservationStartOfMarking = false;
			m_linkMarkPoint.ChangeAgent(this);
			ForceSpecifyFunction(m_rushingAndMarkingFunction);
		}
		if (m_isReservationEndOfMarking && !navMeshAgent.isOnOffMeshLink)
		{
			m_isReservationEndOfMarking = false;
			m_animationController.editAnimation.TriggerReturnWakeUp();
			m_animationController.editAnimation.isWakeUpNextSearch = m_isSetBoolIsNextSearch;
			m_linkMarkPoint.ChangeAgent(null);
			m_linkMarkPoint = null;
		}

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


	//Debug Only
#if UNITY_EDITOR
	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.LocalToWorldPosition(m_indicatedDogCenter), m_indicatedDogRadius);
	}
#endif
}
