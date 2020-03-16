using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTerritory : MonoBehaviour
{
	[SerializeField]
	ManageServants m_manageServants = null;
	[SerializeField]
	string m_firstPointName = "";

	public Dictionary<int, GrahamScan.CustomFormat> markPoints { get; private set; } = new Dictionary<int, GrahamScan.CustomFormat>();
	public List<Vector3> territoryPoints { get; private set; } = new List<Vector3>();

	GameObject m_firstPoint = null;

	// Start is called before the first frame update
	void Start()
    {
		m_firstPoint = GameObject.Find(m_firstPointName);

		RegisterMarkPoint(m_firstPoint);
    }
	
	public void RegisterMarkPoint(GameObject gameObject)
	{
		markPoints.Add(gameObject.GetInstanceID(), new GrahamScan.CustomFormat(gameObject));
		CalculateTerritory();
	}
	public void UnregisterMarkPoint(GameObject gameObject)
	{
		markPoints.Remove(gameObject.GetInstanceID());
		CalculateTerritory();
	}

	public void CalculateTerritory()
	{
		territoryPoints.Clear();

		int i = 0;
		foreach (var e in markPoints)
		{
			territoryPoints.Add(e.Value.gameObject.transform.position);
			++i;
		}

		int result = GrahamScan.Run(territoryPoints);
		if (result < territoryPoints.Count - 1)
			territoryPoints.RemoveRange(result, territoryPoints.Count - result);

		Debug.Log(markPoints.Count + "+"+ territoryPoints.Count);
	}
}
