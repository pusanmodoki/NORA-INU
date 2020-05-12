using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseUniqueOffMeshLink : MonoBehaviour
{
	public Transform pointTransform { get; private set; } = null;
	public Transform agentTransform { get; private set; } = null;
	public Rigidbody agentRigidBody { get; private set; } = null;
	public BoxCastFlags agentGroundFlag { get; private set; } = null;
	public NavMeshAgent navMeshAgent { get; private set; } = null;
	public OffMeshLinkData offMeshLinkData { get; private set; } = default;
	public Vector3 startPoint { get; private set; } = Vector3.zero;
	public Vector3 endPoint { get; private set; } = Vector3.zero;
	public Vector3 moveTargetPoint { get; private set; } = Vector3.zero;
	public int maxAgentToUse { get { return m_maxAgentToUse; } }
	public int nowUseAgent { get; private set; } = 0;
	public bool isOffMeshLink { get; private set; } = false;
	public bool isTransmiter { get { return agentTransform == null; } }

#if UNITY_EDITOR
	[SerializeField, Header("Debug Only"), Tooltip("現在のエージェント数")]
	int m_dNowUseAgent = 0;
#endif
	[SerializeField, Header("Parameters"), Tooltip("1度に使用できる最大エージェント数, infinity = -1")]
	int m_maxAgentToUse = 1;


	public bool AcquisitionRightToUse()
	{
		if (nowUseAgent < (m_maxAgentToUse >= 0 ? m_maxAgentToUse : int.MaxValue))
		{
			++nowUseAgent;

#if UNITY_EDITOR
			m_dNowUseAgent = nowUseAgent;
#endif
			return true;
		}
		else
			return false;
	}

	public void UnacquisitionRightToUse()
	{
		--nowUseAgent;
#if UNITY_EDITOR
		m_dNowUseAgent = nowUseAgent;
#endif
	}

	public void Link(Transform agentTransform, Transform pointTransform, Rigidbody rigidBody, 
		NavMeshAgent navMeshAgent, BoxCastFlags agentGroundFlag, ref Vector3 startPoint, ref Vector3 endPoint, ref Vector3 targetPoint)
	{
		this.agentTransform = agentTransform;
		this.pointTransform = pointTransform;
		this.agentRigidBody = rigidBody;
		this.navMeshAgent = navMeshAgent;
		this.agentGroundFlag = agentGroundFlag;
		this.agentRigidBody = rigidBody;
		offMeshLinkData = navMeshAgent.currentOffMeshLinkData;

		isOffMeshLink = offMeshLinkData.offMeshLink != null;

		this.startPoint = startPoint;
		this.endPoint = endPoint;
		moveTargetPoint = targetPoint;

		StartOffMeshLink();
	}
	public void Unlink()
	{
		//RigidBodyをKinematicに、Velocity = 0
		agentRigidBody.isKinematic = true;
		agentRigidBody.useGravity = false;
		agentRigidBody.velocity = Vector3.zero;
		//OffMeshLink終了, Stop解除
		if (navMeshAgent.isOnOffMeshLink)
			navMeshAgent.CompleteOffMeshLink();
		navMeshAgent.isStopped = false;
		navMeshAgent.updatePosition = true;
		navMeshAgent.updateRotation = true;

		//destination保存
		Vector3 destination = navMeshAgent.destination;
		//Warp
		navMeshAgent.Warp(endPoint);
		//destination再設定
		navMeshAgent.destination = destination;
	}

	public bool CalledUpdateOffMeshLink()
	{
		return UpdateOffMeshLink();
	}
	public bool CalledFixedUpdateOffMeshLink()
	{
		return FixedUpdateOffMeshLink();
	}

	protected abstract void StartOffMeshLink();

	protected abstract bool UpdateOffMeshLink();

	protected abstract bool FixedUpdateOffMeshLink();

	protected void SwapEnableRigidAndAgent()
	{
		if (agentRigidBody.isKinematic)
		{
			//RigidBodyをInkinematicに、Velocity = 0
			agentRigidBody.isKinematic = false;
			agentRigidBody.useGravity = true;
			agentRigidBody.velocity = Vector3.zero;
			//Stop
			navMeshAgent.isStopped = true;
			navMeshAgent.updateRotation = false;
			navMeshAgent.updatePosition = false;
			navMeshAgent.velocity = Vector3.zero;
		}
		else
		{
			//RigidBodyをKinematicに、Velocity = 0
			agentRigidBody.isKinematic = true;
			agentRigidBody.useGravity = false;
			agentRigidBody.velocity = Vector3.zero;
			//Stop解除
			navMeshAgent.isStopped = false;
			navMeshAgent.updateRotation = true;
			navMeshAgent.updatePosition = true;
		}
	}
}
