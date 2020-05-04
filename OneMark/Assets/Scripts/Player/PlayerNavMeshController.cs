using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshController : MonoBehaviour
{
	static readonly float m_cBoxCastAddHeight = 100.0f;
	static readonly float m_cBoxCastDistance = 150.0f;

	public NavMeshAgent navMeshAgent { get { return m_navMeshAgent; } }
	public GameObject targetObject { get { return m_targetObject; } }

	[SerializeField]
	PlayerInput m_input = null;
	[SerializeField]
	PlayerMaualCollisionAdministrator m_maualCollisionAdministrator = null;
	[SerializeField]
	NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	GameObject m_targetObject = null;
	[SerializeField]
	BoxCastFlags m_groundFlag = null;

	[SerializeField, Space]
	LayerMaskEx m_checkGroundLayerMask = 0;
	[SerializeField]
	Vector3 m_checkGroundSize = Vector3.one;

	[SerializeField, Space, Range(1, 10)]
	int m_updateTargetIntervalFrames = 1;
	[SerializeField, Range(0.1f, 2.5f)]
	float m_updateTargetDistanceScaler = 1;

	//debug only
#if UNITY_EDITOR
	/// <summary>gizmo color -> hit frame</summary>
	static readonly Color m_cdHitColor = new Color(1.0f, 0.15f, 0.1f, 0.8f);
	/// <summary>gizmo color -> not hit frame</summary>
	static readonly Color m_cdNotHitColor = new Color(0.95f, 0.95f, 0.95f, 0.8f);
	/// <summary>gizmo color -> not hit frame</summary>
	static readonly Color m_cdChangedColor = new Color(0.15f, 0.95f, 0.15f, 0.8f);

	/// <summary>Draw Gizmos? (debug only)</summary>
	[SerializeField, Tooltip("Draw Gizmos? (debug only)"), Header("Debug Only")]
	bool m_dIsDrawGizmos = false;

	Vector3 m_dRaycastStartPosition = Vector3.zero;
	Vector3 m_dRaycastChangedStartPosition = Vector3.zero;
	bool m_dIsChanged = false;
	bool m_dIsHitBoxCast = false;
#endif

	NavMeshPath m_navMeshPath =null;
	TimerFrame m_setTargetIntervalTimer = new TimerFrame();

	RaycastHit m_raycastHit = new RaycastHit();
	Vector3 m_moveInput = Vector3.zero;
	Vector3 m_raycastPosition = Vector3.zero;
	Vector3 m_testRaycastPosition = Vector3.zero;
	Vector3 m_territoryNormal = Vector3.zero;
	Vector3 m_territoryHitPoint = Vector3.zero;

	// Start is called before the first frame update
	void Start()
	{
		m_navMeshPath = new NavMeshPath();
		m_setTargetIntervalTimer.Start();
	}

	// Update is called once per frame
	void Update()
	{
		if (m_setTargetIntervalTimer.elapasedFrame >= m_updateTargetIntervalFrames)
		{
			if (m_groundFlag.isStay)
			{
				CalculateMoveTarget();
				m_navMeshAgent.SetDestination(m_targetObject.transform.position);
			}
			else
				m_navMeshAgent.SetDestination(transform.position);

			m_setTargetIntervalTimer.Start();
		}

	}

	void CalculateMoveTarget()
	{
		m_moveInput = m_input.moveInput * m_updateTargetDistanceScaler;

		if (m_moveInput.x == 0.0f && m_moveInput.z == 0.0f)
		{
			navMeshAgent.isStopped = true;
			return;
		}

		m_raycastPosition = transform.position + m_moveInput;

		m_raycastPosition.y += m_cBoxCastAddHeight;

#if UNITY_EDITOR
		m_dIsChanged = false;

		m_dRaycastStartPosition = m_raycastPosition;
		m_dRaycastChangedStartPosition = m_raycastPosition;
		m_dIsHitBoxCast = Physics.BoxCast(m_raycastPosition, m_checkGroundSize * 0.5f, Vector3.down,
					out m_raycastHit, Quaternion.identity, m_cBoxCastDistance, m_checkGroundLayerMask);
#else
		Physics.BoxCast(m_raycastPosition, m_checkGroundSize, Vector3.down,
					out m_raycastHit, Quaternion.identity, m_cBoxCastDistance, m_checkGroundLayerMask);
#endif

		m_raycastPosition.y = m_raycastHit.point.y;

		navMeshAgent.isStopped = false;

		if (!m_navMeshAgent.CalculatePath(m_raycastPosition, m_navMeshPath))
		{
			m_testRaycastPosition = m_raycastPosition;
			m_testRaycastPosition.x -= m_moveInput.x;

			if (m_navMeshAgent.CalculatePath(m_testRaycastPosition, m_navMeshPath))
			{
#if UNITY_EDITOR
				m_dIsChanged = true;
				Debug.Log("NavMeshMove input.x == 0.0f");
#endif

				m_raycastPosition.x = m_testRaycastPosition.x;
				m_moveInput.x = 0.0f;
			}
			else
			{
				m_testRaycastPosition.x = m_raycastPosition.x;
				m_testRaycastPosition.z -= m_moveInput.z;
				if (m_navMeshAgent.CalculatePath(m_testRaycastPosition, m_navMeshPath))
				{
#if UNITY_EDITOR
					m_dIsChanged = true;
					Debug.Log("NavMeshMove input.z == 0.0f");
#endif

					m_raycastPosition.z = m_testRaycastPosition.z;
					m_moveInput.z = 0.0f;
				}
				else
				{
#if UNITY_EDITOR
					m_dIsChanged = true;
					Debug.LogWarning("Warning!! NavMeshMove not going anywhere.");
#endif
					m_raycastPosition = transform.position;
				}
			}
		}

		if (m_maualCollisionAdministrator.isTerritoryStay
			& m_maualCollisionAdministrator.isTerritorySegmentStay)
		{
			if (CollisionTerritory.HitRayTerritory(PlayerAndTerritoryManager.instance.mainPlayer.territorialArea,
				transform.position - m_moveInput, m_input.moveInput, m_moveInput.magnitude * 2,
				out m_territoryNormal, out m_territoryHitPoint))
			{
				m_raycastPosition += m_territoryNormal *
					  (new Vector3(m_raycastPosition.x, 0.0f, m_raycastPosition.z) - m_territoryHitPoint).magnitude;
			}
		}

		m_targetObject.transform.position = m_raycastPosition;
	}

	//debug only
#if UNITY_EDITOR
	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		//!Flgな場合終了
		if (!m_dIsDrawGizmos) return;

		//呼び出しコスト削減
		Transform myTransform = transform;

		//描画に反映されないため実行中以外はこちらで更新を行う
		if (UnityEditor.EditorApplication.isPlaying & UnityEditor.EditorApplication.isPaused)
			Update();
		else if (!UnityEditor.EditorApplication.isPlaying)
		{
			m_dRaycastStartPosition = myTransform.position + new Vector3(0.0f, m_cBoxCastAddHeight, m_updateTargetDistanceScaler);

			m_dIsHitBoxCast = Physics.BoxCast(m_dRaycastStartPosition, m_checkGroundSize * 0.5f, Vector3.down,
						out m_raycastHit, Quaternion.identity, m_cBoxCastDistance, m_checkGroundLayerMask);
		}

		//Hit
		if (m_dIsHitBoxCast)
		{
			//Color
			Gizmos.color = m_cdHitColor;
			//Draw Ray
			Gizmos.DrawRay(m_dRaycastStartPosition, Vector3.down * m_raycastHit.distance);
			//Matrix
			Gizmos.matrix = Matrix4x4.Translate(m_dRaycastStartPosition + Vector3.down * m_raycastHit.distance);
			Gizmos.matrix *= Matrix4x4.Rotate(myTransform.rotation);
			//Draw Cube
			Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(myTransform.lossyScale, m_checkGroundSize));
		}
		//Not Hit
		else
		{
			//Color
			Gizmos.color = m_cdNotHitColor;
			//Draw Ray
			Gizmos.DrawRay(m_dRaycastStartPosition, Vector3.down * m_cBoxCastDistance);
			//Matrix
			Gizmos.matrix = Matrix4x4.Translate(m_dRaycastStartPosition + Vector3.down * m_cBoxCastDistance);
			Gizmos.matrix *= Matrix4x4.Rotate(myTransform.rotation);
			//Draw Cube
			Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(myTransform.lossyScale, m_checkGroundSize));
		}

		if (m_dIsChanged)
		{
			//Color
			Gizmos.color = m_cdChangedColor;
			Gizmos.matrix = Matrix4x4.identity;
			//Draw Ray
			Gizmos.DrawRay(m_dRaycastChangedStartPosition, Vector3.down * m_cBoxCastDistance);
			//Matrix
			Gizmos.matrix = Matrix4x4.Translate(m_dRaycastChangedStartPosition + Vector3.down * m_cBoxCastDistance);
			Gizmos.matrix *= Matrix4x4.Rotate(myTransform.rotation);
			//Draw Cube
			Gizmos.DrawWireCube(Vector3.zero, Vector3.Scale(myTransform.lossyScale, m_checkGroundSize));
		}
	}
#endif
}
