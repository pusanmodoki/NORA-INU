using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrahamTest : MonoBehaviour
{
	[SerializeField]
	GameObject m_instantiate = null;

	List<Vector3> m_points = new List<Vector3>();
		
    // Start is called before the first frame update
    void Start()
    {
		var objects = GameObject.FindGameObjectsWithTag("User0");
	
		foreach (var e in objects)
			m_points.Add(e.transform.position);

		int result =  GrahamScan.Run(m_points);

		for (int i = 0; i < result; ++i)
		{
			GameObject obj = GameObject.Instantiate(m_instantiate);
			obj.transform.position = m_points[i];
		}
	}
}
