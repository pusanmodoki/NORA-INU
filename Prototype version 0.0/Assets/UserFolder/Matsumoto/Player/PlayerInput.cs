using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    /// <summary>
    /// speed
    /// </summary>
    [SerializeField]
    private float moveSpeed = 1.0f;

    [SerializeField]
    private float maxSpeed = 5.0f;

    /// <summary>
    /// コントローラを使ってるかどうか
    /// </summary>
    [SerializeField]
    private bool isControler = false;


    private Vector3 m_inputVector = Vector3.zero;

    /// <summary>
    /// player rigidbody
    /// </summary>
    private Rigidbody m_thisRigitBody;

    private Female m_female;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        m_thisRigitBody = GetComponent<Rigidbody>();
        m_female = GetComponent<Female>();
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
	private void FixedUpdate()
    {
        MoveInput();

       // ShotInput();
    }

    

    /// <summary>
    ///  移動入力
    /// </summary>
    private void MoveInput()
    {

        Vector2 inputVector2 = Vector2.zero;

        m_inputVector = Vector3.zero;

        if (isControler)
        {
            m_inputVector.x = Input.GetAxis("Horizontal");
            m_inputVector.z = Input.GetAxis("Vertical");
        }
        else
        {

            inputVector2.x = Input.GetAxisRaw("Horizontal");
            inputVector2.y = Input.GetAxisRaw("Vertical");

            inputVector2.Normalize();

            m_inputVector.x = inputVector2.x;
            m_inputVector.z = inputVector2.y;
        }

        m_thisRigitBody.AddForce(m_inputVector * moveSpeed);

        inputVector2 = new Vector2(m_thisRigitBody.velocity.x, m_thisRigitBody.velocity.z);
        if (inputVector2.magnitude > maxSpeed)
        {
            inputVector2 = inputVector2.normalized * maxSpeed;
            m_thisRigitBody.velocity = new Vector3(inputVector2.x, m_thisRigitBody.velocity.y, inputVector2.y);
        }

    }   // end MoveInput

    /// <summary>
    /// 飛ばします処理
    /// </summary>
    private void ShotInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            m_female.ShotServant(transform.forward);
        }
    }   // end ShotInput
}
