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


    private void Start()
    {
        m_thisRigitBody = GetComponent<Rigidbody>();
        m_female = GetComponent<Female>();
    }

    // Update is called once per frame
    void Update()
    {
	}

	private void FixedUpdate()
    {
        MoveInput();

        ShotInput();
    }

    
    /// <summary>
    /// 入力処理
    /// </summary>
    private void InputPlayer()
    {
        // 移動入力
        MoveInput();
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

        //m_thisRigitBody.AddForce(m_inputVector * moveSpeed);


        //inputVector2 = new Vector2(m_thisRigitBody.velocity.x, m_thisRigitBody.velocity.z);
        //if (inputVector2.magnitude > maxSpeed)
        //{
        //    inputVector2 = inputVector2.normalized * maxSpeed;
        //    m_thisRigitBody.velocity = new Vector3(inputVector2.x, m_thisRigitBody.velocity.y, inputVector2.y);
        //}

        transform.LookAt(transform.position + m_inputVector);

        m_inputVector = m_inputVector * moveSpeed;
        m_inputVector.y = m_thisRigitBody.velocity.y;
        m_thisRigitBody.velocity = m_inputVector;
    }

    private void ShotInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            m_female.ShotServant(transform.forward);
        }
    }
}
