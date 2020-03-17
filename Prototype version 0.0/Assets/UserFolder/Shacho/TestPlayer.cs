using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
	[SerializeField]
	Rigidbody m_rigidBody = null;
	ManageTerritory m_territory = null;

    // Start is called before the first frame update
    void Start()
    {
		m_territory = GetComponent<ManageTerritory>();
	}

    // Update is called once per frame
    void Update()
    {
		Debug.Log(CollisionTerritory.HitCircleTerritory(m_territory.territoryPoints, transform.position, 1));
    }

	void FixedUpdate()
	{
		//Vector3 input = Vector3.zero;

		//input.z = Input.GetAxis("Vertical");
		//input.x = Input.GetAxis("Horizontal");

		//m_rigidBody.AddForce(input);
	}
}
