using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageServants : MonoBehaviour
{
	//public List<GrahamScan.CustomFormat> 

	[SerializeField]
	ManageTerritory m_manageTerritory = null;
	[SerializeField]
	string m_firstServantName = "";

	Dictionary<int, GameObject> m_servants = new Dictionary<int, GameObject>();
	List<GrahamScan.CustomFormat> m_useTerritoryServants = new List<GrahamScan.CustomFormat>();

	void Start()
	{
		GameObject.Find(m_firstServantName).GetComponent<ManageCommander>().RegisterCommander(gameObject, this);

		CommanderManager.instance.RegisterCommander(gameObject);
	}
	void OnDestroy()
	{
		CommanderManager.instance.UnregisterCommander(gameObject);
	}

	public void RegisterServantCallback(GameObject servant)
	{
		int instanceID = servant.GetInstanceID();

		m_servants.Add(instanceID, servant);
		m_useTerritoryServants.Add(new GrahamScan.CustomFormat(servant));

		m_manageTerritory.CalculateTerritory();
	}

	public void UnregisterServantCallback(GameObject servant)
	{
		int instanceID = servant.GetInstanceID();

		m_servants.Remove(instanceID);
		for (int i = 0, count = m_useTerritoryServants.Count; i < count; ++i)
		{
			if (m_useTerritoryServants[i].gameObject.GetInstanceID() == instanceID)
			{
				m_useTerritoryServants.RemoveAt(i);
				break;
			}
		}

		m_manageTerritory.CalculateTerritory();
	}

	public void LinkMarkingCallback(LinkMarking thisComponent, LinkMarking linkedComponent, int linked)
	{
		if (linked == LinkMarking.cSettingLink)
			m_manageTerritory.RegisterMarkPoint(linkedComponent.gameObject);
		else if (linked == LinkMarking.cReleaseLink)
			m_manageTerritory.UnregisterMarkPoint(linkedComponent.gameObject);
		else
			m_manageTerritory.CalculateTerritory();
	}
}
