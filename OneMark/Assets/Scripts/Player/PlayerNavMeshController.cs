using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshController : MonoBehaviour
{
	public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }

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

	Vector3 m_velocity = Vector3.zero;
	Vector3 m_destination = Vector3.zero;
	Vector3 m_territoryNormal = Vector3.zero;
	Vector3 m_territoryHitPoint = Vector3.zero;
	
	// Update is called once per frame
	void Update()
	{
		Move();
	}

	void Move()
	{
		if (m_input.moveInput.x == 0.0f && m_input.moveInput.z == 0.0f)
		{
			navMeshAgent.isStopped = true;
			return;
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_input.moveInput), m_rotationSpeed * Time.deltaTime);

		m_velocity = m_input.moveInput * m_moveSpeed * Time.deltaTime;
		m_destination = transform.position + m_velocity;


		if (m_maualCollisionAdministrator.isTerritoryStay
			& m_maualCollisionAdministrator.isTerritorySegmentStay)
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


		//debug only
#if UNITY_EDITOR
		 m_dVelocity = m_velocity;
#endif
	}
}
