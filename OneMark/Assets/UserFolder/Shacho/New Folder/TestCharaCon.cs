using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharaCon : MonoBehaviour
{
	CharacterController characterController = null;

    // Start is called before the first frame update
    void Start()
    {
		characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
		characterController.Move(transform.forward * Time.deltaTime);
		UnityEngine.AI.NavMeshHit h;
		//Debug.Log(UnityEngine.AI.NavMesh.SamplePosition(transform.position, out h, 2.0f, UnityEngine.AI.NavMesh.AllAreas) + "+"
		//	+ Mathf.Abs(transform.position.x -  h.position.x) + "+" + Mathf.Abs(transform.position.z - h.position.z));
    }
}
