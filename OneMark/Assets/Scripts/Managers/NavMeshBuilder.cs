using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(1)]
public class NavMeshBuilder : MonoBehaviour
{
	public static NavMeshBuilder instance { get; private set; } = null;

	[SerializeField]
	NavMeshSurface[] m_notVolumeNavMeshSurfaces = null;
	[SerializeField]
	GameObject[] m_navMeshSurfaceObjects = null;

	[SerializeField, Space]
	float m_bakeInterval = 0.1f;

	NavMeshSurface[] m_navMeshSurfaces = null;

	Timer m_bakeIntervalTimer = new Timer();

	void Awake()
	{
		instance = this;
		m_navMeshSurfaces = new NavMeshSurface[m_navMeshSurfaceObjects.Length];

		for(int i = 0, length = m_navMeshSurfaceObjects.Length; i < length; ++i)
			m_navMeshSurfaces[i] = m_navMeshSurfaceObjects[i].GetComponent<NavMeshSurface>();

		{
			Vector3 setPosition = Vector3.zero, playerPosition = Vector3.zero;
			int i = 0;
			foreach (var e in PlayerAndTerritoryManager.instance.allPlayers)
			{
				setPosition = e.Value.gameObject.transform.position;

				m_navMeshSurfaceObjects[i].transform.position = setPosition;
				m_navMeshSurfaces[i].BuildNavMesh();
				e.Value.navMeshAgent.enabled = true;
				++i;
			}
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		m_bakeIntervalTimer.Start();
	}

    // Update is called once per frame
    void LateUpdate()
	{
		if (m_bakeIntervalTimer.elapasedTime > m_bakeInterval)
		{
			for (int index = 0, length = m_notVolumeNavMeshSurfaces.Length; index < length; ++index)
				m_notVolumeNavMeshSurfaces[index].BuildNavMesh();

			Vector3 playerPosition = Vector3.zero;
			int i = 0;
			foreach(var e in PlayerAndTerritoryManager.instance.allPlayers)
			{
				playerPosition = e.Value.gameObject.transform.position;
				if (e.Value.groundFlag.isStay)
					playerPosition.y = e.Value.groundFlag.boxCastResult.position.y;
				else
				{
					RaycastHit hit = default;
					if (Physics.Raycast(e.Value.groundFlag.centerPosition, 
						e.Value.groundFlag.direction, out hit, 100.0f, e.Value.groundFlag.layerMask))
					{
						playerPosition.y = hit.point.y;
					}
				}

				m_navMeshSurfaceObjects[i].transform.position = playerPosition;
				m_navMeshSurfaces[i].BuildNavMesh();
				++i;
			}
			m_bakeIntervalTimer.Start();
		}
	}
}
