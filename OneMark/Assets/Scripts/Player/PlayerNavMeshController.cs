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

	BaseUniqueOffMeshLink m_pointUniqueOffMeshLink = null;
	BaseUniqueOffMeshLink m_copyUniqueOffMeshLink = null;
	Vector3 m_position = Vector3.zero;
	Vector3 m_velocity = Vector3.zero;
	Vector3 m_moveVelocity = Vector3.zero;
	Vector3 m_destination = Vector3.zero;
	Vector3 m_territoryNormal = Vector3.zero;
	Vector3 m_territoryHitPoint = Vector3.zero;

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
	}


	void Move()
	{
		if (m_input.moveInput.x == 0.0f && m_input.moveInput.z == 0.0f)
		{
			navMeshAgent.isStopped = true;
			return;
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_input.moveInput), m_rotationSpeed * Time.deltaTime);

		m_position = transform.position;
		m_velocity = m_input.moveInput * m_moveSpeed * Time.deltaTime;
		m_destination = transform.position + m_velocity;

		if (m_maualCollisionAdministrator.isTerritoryStay & m_maualCollisionAdministrator.isTerritorySegmentStay
			& !PlayerAndTerritoryManager.instance.allPlayers[gameObject.GetInstanceID()].isGameOverGracePeiod)
		{
			if (CollisionTerritory.HitRayTerritory(PlayerAndTerritoryManager.instance.mainPlayer.territorialArea,
				transform.position - m_velocity, m_input.moveInput, m_velocity.magnitude * 2,
				out m_territoryNormal, out m_territoryHitPoint))
			{
				m_velocity += m_territoryNormal *
					  (new Vector3(m_destination.x, 0.0f, m_destination.z) - m_territoryHitPoint).magnitude;
			}
		}

		m_navMeshAgent.Move(m_velocity);
		m_moveVelocity = transform.position - m_position;

		if (m_moveVelocity != m_velocity && !PlayerAndTerritoryManager.instance.allPlayers[gameObject.GetInstanceID()].isGameOverGracePeiod)
		{
			if (CollisionTerritory.HitRayTerritory(PlayerAndTerritoryManager.instance.mainPlayer.territorialArea,
				m_position - m_moveVelocity, m_moveVelocity.normalized, m_moveVelocity.magnitude * 2.0f,
				out m_territoryNormal, out m_territoryHitPoint))
			{
				m_destination = transform.position + m_moveVelocity;

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
			m_navMeshAgent.updatePosition = true;
		}
	}
}
