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

        Debug.Assert(!m_female || !m_thisRigitBody);
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
	}

	private void FixedUpdate()
    {
        Move();
	}

    private void Move()
    {
        m_thisRigitBody.AddForce(m_inputVector * moveSpeed);
    }

    private void Shot()
    {

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

    private void ShotInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
        }
    }
}
