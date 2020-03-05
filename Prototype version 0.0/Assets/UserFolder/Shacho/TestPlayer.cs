using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
	[SerializeField]
	Rigidbody m_rigidBody = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

	void FixedUpdate()
	{
		Vector3 input = Vector3.zero;

		input.z = Input.GetAxis("Vertical");
		input.x = Input.GetAxis("Horizontal");

		m_rigidBody.AddForce(input);
	}
}
