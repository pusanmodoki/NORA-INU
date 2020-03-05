using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoving : MonoBehaviour
{
	[SerializeField]
	UnityEngine.AI.NavMeshAgent m_navMeshAgent = null;
	[SerializeField]
	GameObject m_gameObject = null;

    // Start is called before the first frame update
    void Start()
    {
		m_navMeshAgent.destination = m_gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
