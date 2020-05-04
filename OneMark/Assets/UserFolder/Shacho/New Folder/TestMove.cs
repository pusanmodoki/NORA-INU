using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMove : MonoBehaviour
{
	NavMeshAgent m_meshAgent = null;	
    // Start is called before the first frame update
    void Start()
    {
		m_meshAgent = GetComponent<NavMeshAgent>();

	}

    // Update is called once per frame
    void Update()
    {
		m_meshAgent.Move(Vector3.right * Time.deltaTime);
    }
}
