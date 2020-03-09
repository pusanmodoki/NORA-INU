using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoving : MonoBehaviour
{
	[SerializeField]
	UnityEngine.AI.NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	GameObject m_gameObject = null;
	Timer timer = new Timer();

	// Start is called before the first frame update
	void Start()
    {
		m_navMeshAgent.destination = m_gameObject.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			for (int i = 0; i < 100000; ++i)
			{
				Physics.OverlapSphere(Vector3.zero, 100);
			}
			sw.Stop();
			Debug.Log("1: " + sw.ElapsedMilliseconds);

			sw.Start();
			for (int i = 0; i < 100000; ++i)
			{
				Physics.OverlapBox(Vector3.zero, Vector3.one * 100);
			}
			sw.Stop();
			Debug.Log("2: " + sw.ElapsedMilliseconds);
		}

	}
}
