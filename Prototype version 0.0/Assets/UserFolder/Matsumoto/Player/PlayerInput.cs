using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// player rigidbody
    /// </summary>
    [SerializeField]
    private Rigidbody thisRigitBody;

    /// <summary>
    /// speed
    /// </summary>
    [SerializeField]
    private float moveSpeed = 1.0f;

    /// <summary>
    /// コントローラを使ってるかどうか
    /// </summary>
    [SerializeField]
    private bool isControler = false;

    private Vector3 m_inputVector = Vector3.zero;
    

	// Update is called once per frame
	void Update()
    {
	}

	private void FixedUpdate()
    {

	}



    private void InputPlayer()
    {
        m_inputVector = Vector3.zero;

        if (isControler)
        {
            m_inputVector.x = Input.GetAxis("Horizontal");
            m_inputVector.z = Input.GetAxis("Vertical");
        }
        else
        {
            Vector2 inputVector2 = Vector2.zero;

            inputVector2.x = Input.GetAxis("Horizontal");
            inputVector2.y = Input.GetAxis("Vertical");

            inputVector2.Normalize();

            m_inputVector.x = inputVector2.x;
            m_inputVector.z = inputVector2.y;
        }
    }
}
