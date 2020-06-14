using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshController : MonoBehaviour
{
	public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }
	public bool isOnUniqueOffMeshLink { get { return m_copyUniqueOffMeshLink != null || m_navMeshAgent.isOnOffMeshLink; } }
	public bool isOnManualUniqueOffMeshLink { get { return m_copyUniqueOffMeshLink != null; } }

	[SerializeField]
	PlayerInput m_input = null;
	[SerializeField]
	PlayerMaualCollisionAdministrator m_maualCollisionAdministrator = null;
	[SerializeField]
	NavMeshAgent m_navMeshAgent = null;
	
	[SerializeField, Space]
	float m_moveSpeed = 1;
	[SerializeField]
	float m_rotationSpeed = 1;

	//debug only
#if UNITY_EDITOR
	/// <summary>Draw Gizmos? (debug only)</summary>
	[SerializeField, Tooltip("Velocity drawing"), Header("Debug Only")]
	Vector3 m_dVelocity = Vector3.zero;
#endif
	
	PlayerAndTerritoryManager.PlayerInfo m_player = null;
	BaseUniqueOffMeshLink m_pointUniqueOffMeshLink = null;
	BaseUniqueOffMeshLink m_copyUniqueOffMeshLink = null;
	Vector3 m_position = Vector3.zero;
	Vector3 m_velocity = Vector3.zero;
	Vector3 m_moveVelocity = Vector3.zero;
	Vector3 m_destination = Vector3.zero;
	Vector3 m_territoryNormal = Vector3.zero;
	Vector3 m_territoryHitPoint = Vector3.zero;
	Vector3 m_oldFixedPosition = Vector3.zero;
	Vector3 m_fixedPosition = Vector3.zero;

	public bool LinkManualUniqueOffMeshLink(BaseUniqueOffMeshLink pointUniqueOffMeshLink ,
		BaseUniqueOffMeshLink copyUniqueOffMeshLink)
	{
		if (m_copyUniqueOffMeshLink == null)
		{
			m_pointUniqueOffMeshLink = pointUniqueOffMeshLink;
			m_copyUniqueOffMeshLink = copyUniqueOffMeshLink;
			m_navMeshAgent.updatePosition = false;
			return true;
		}
		else
			return false;
	}

	void Start()
	{
		m_player =  PlayerAndTerritoryManager.instance.allPlayers[gameObject.GetInstanceID()];
	}
	// Update is called once per frame
	void Update()
	{
		if (!isOnManualUniqueOffMeshLink) Move();
		else CheckManualUniqueOffMeshLink(true);
	}
	// Update is called once per frame
	void FixedUpdate()
	{
		if (isOnManualUniqueOffMeshLink)
			CheckManualUniqueOffMeshLink(false);

		m_oldFixedPosition = m_fixedPosition;
		m_fixedPosition = transform.position;
	}


	void Move()
	{
		if (m_player.manualCollisionAdministrator.isTerritoryExit)
		{
			float distance = (m_player.maxPointMoveDistance * 2.0f
				+ Vector3.Distance(m_oldFixedPosition, transform.position) * 2.0f
				+ m_player.manualCollisionAdministrator.collisionRadius);
			if ((m_player.shortestTerritoryBorderPoint.ToYManual(transform.position.y)
				- transform.position).sqrMagnitude < distance * distance)
			{
				navMeshAgent.Warp(m_player.shortestTerritoryBorderPoint.ToYManual(transform.position.y) + 
					Vector3.Cross((m_player.territorialArea[m_player.shortestTerritoryPointIndex1] 
					- m_player.territorialArea[m_player.shortestTerritoryPointIndex0]).normalized, Vector3.up).normalized * 0.5f);
				m_player.manualCollisionAdministrator.ResetTerritoryHitInfo(true);
			}
		}

		if (m_input.moveInput.x == 0.0f && m_input.moveInput.z == 0.0f)
		{
			navMeshAgent.isStopped = true;
			return;
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_input.moveInput), m_rotationSpeed * Time.deltaTime);

		int index0, index1;
		m_position = transform.position;
		m_velocity = m_input.moveInput * m_moveSpeed * Time.deltaTime;
		m_destination = m_position + m_velocity;

		if (m_maualCollisionAdministrator.isTerritoryStay 
			& m_maualCollisionAdministrator.isTerritorySegmentStay & !m_player.isGameOverGracePeiod)
		{
			CollisionTerritory.HitRayTerritory(m_player.territorialArea, m_position - m_velocity, m_input.moveInput,
				m_velocity.magnitude * 2, out m_territoryNormal, out m_territoryHitPoint, out index0, out index1);


			if (CollisionTerritory.HitRayTerritory(m_player.territorialArea, m_position - m_velocity, m_input.moveInput, 
				m_velocity.magnitude * 2, out m_territoryNormal, out m_territoryHitPoint, out index0, out index1))
			{
				m_velocity += m_territoryNormal *
					  (new Vector3(m_destination.x, 0.0f, m_destination.z) - m_territoryHitPoint).magnitude;
			}
		}

		m_navMeshAgent.Move(m_velocity);
		m_position = transform.position;
		m_moveVelocity = m_position - m_position;

		if (m_moveVelocity != m_velocity && !m_player.isGameOverGracePeiod)
		{
			if (CollisionTerritory.HitRayTerritory(m_player.territorialArea, m_position - m_moveVelocity, m_moveVelocity.normalized,
				m_moveVelocity.magnitude * 2.0f, out m_territoryNormal, out m_territoryHitPoint, out index0, out index1))
			{
				m_destination = m_position + m_moveVelocity;

				m_moveVelocity += m_territoryNormal *
					  (new Vector3(m_destination.x, 0.0f, m_destination.z) - m_territoryHitPoint).magnitude;
				transform.position = m_position;
				m_navMeshAgent.Move(m_moveVelocity);
			}
		}


		//debug only
#if UNITY_EDITOR
		m_dVelocity = m_velocity;
#endif
	}

	void CheckManualUniqueOffMeshLink(bool isUpdate)
	{
		if ((isUpdate && !m_copyUniqueOffMeshLink.CalledUpdateOffMeshLink())
			|| (!isUpdate && !m_copyUniqueOffMeshLink.CalledFixedUpdateOffMeshLink()))
		{
			m_pointUniqueOffMeshLink.UnacquisitionRightToUse();
			m_copyUniqueOffMeshLink.Unlink();
			Object.Destroy(m_copyUniqueOffMeshLink);
			m_copyUniqueOffMeshLink = null;
		}
	}
}
