using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTerritoryIntermediary : MonoBehaviour
{
	[SerializeField]
	string m_firstPointName = "";
	[SerializeField]
	LineRenderer m_lineRenderer = null;
	[SerializeField]
	string m_testName = "";

	BaseMarkPoint m_firstPoint = null;
	bool m_isPauseFirarPoint = false;

    // Start is called before the first frame update
    void Start()
    {
		PlayerAndTerritoryManager.instance.AddPlayer(gameObject);

		GameObject firstPoint = GameObject.Find(m_firstPointName);

		if (firstPoint != null)
			m_firstPoint = firstPoint.GetComponent<BaseMarkPoint>();

		if (m_firstPoint != null)
		{
			var playerInfo = PlayerAndTerritoryManager.instance.GetPlayer(gameObject.GetInstanceID());
			playerInfo.changeTerritoryCallback += ChangeTerritory;
			m_firstPoint.LinkPlayer(gameObject, null);

			m_firstPoint.isPauseTimer = true;
			m_isPauseFirarPoint = true;
		}
		else
		{
			Debug.LogError("Error!! PlayerTerritoryIntermediary->Start\n FirstPoint not found");
		}
	}

	void Update()
	{
		var playerInfo = PlayerAndTerritoryManager.instance.GetPlayer(gameObject.GetInstanceID());

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

		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameObject point = GameObject.Find(m_testName);
			var point1 = point.GetComponent<BaseMarkPoint>();


			var obj = ServantManager.instance.servantByMainPlayer[0];
			var com = obj.GetComponent<DogRushingAndMarking>();
			com.SetAdvanceInformation(point1, point.transform.position);
			//point1.LinkPlayer(gameObject, null);
			obj.ForceSpecifyFunction(com);
		}
	}

	public void ChangeTerritory()
	{
		if (m_isPauseFirarPoint)
		{
			m_firstPoint.isPauseTimer = false;
			m_isPauseFirarPoint = false;
		}
	}

    // Update is called once per frame
    void OnDestroy()
    {
		if (m_firstPoint != null)
			PlayerAndTerritoryManager.instance.GetPlayer(
				m_firstPoint.linkPlayerID).changeTerritoryCallback -= ChangeTerritory;
    }
}
