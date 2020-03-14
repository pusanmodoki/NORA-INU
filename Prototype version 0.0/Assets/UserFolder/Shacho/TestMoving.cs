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
    }
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
			GetComponent<KamikazeCommand>().InvokeCommand(transform.forward, 10.0f);
		}

	}
}
