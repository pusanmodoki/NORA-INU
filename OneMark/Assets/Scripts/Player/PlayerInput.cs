﻿using System.Collections;
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

    /// <summary>
    /// アニメーションコントローラ
    /// </summary>
    [SerializeField]
    private Animator animator = null;

    private PlayerTerritoryIntermediary territoryIntermediary = null;

    [SerializeField, Tooltip("入力受付状態")]
    private bool isControlInput = true;

    private enum AnimationState
    {
        Stand = 0,
        Run,
        GameClear,
        GameOver
    }

    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    [SerializeField]
    private AnimationState state = AnimationState.Stand;

    private Vector3 m_inputVector = Vector3.zero;

    /// <summary>
    /// player rigidbody
    /// </summary>
    private Rigidbody m_thisRigitBody;

    private Female m_female;


    private void Start()
    {
        territoryIntermediary = GetComponent<PlayerTerritoryIntermediary>();
        m_thisRigitBody = GetComponent<Rigidbody>();
        m_female = GetComponent<Female>();
    }

    // Update is called once per frame
    void Update()
    {
	}

	private void FixedUpdate()
    {
        if (isControlInput)
        {
            MoveInput();

            ShotInput();
        }
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
		var step = GetComponent<PlayerStepFlags>();
		Vector2 inputVector2 = Vector2.zero;

        m_inputVector = Vector3.zero;

        if (isControler)
        {
            m_inputVector.x = Input.GetAxis("Horizontal");
            m_inputVector.z = Input.GetAxis("Vertical");
        }
        else
        {

            inputVector2.x = Input.GetAxis("Horizontal");
			if (inputVector2.x < 0.0f && !step.isLeftStay)
				inputVector2.x = 0.0f;
			else if(inputVector2.x > 0.0f && !step.isRightStay)
				inputVector2.x = 0.0f;

			inputVector2.y = Input.GetAxis("Vertical");
			if (inputVector2.y < 0.0f && !step.isBackStay)
				inputVector2.y = 0.0f;
			else if (inputVector2.y > 0.0f && !step.isForwardStay)
				inputVector2.y = 0.0f;

			if (inputVector2.magnitude > 1.0f)
            {
                inputVector2.Normalize();
            }

            m_inputVector.x = inputVector2.x;
            m_inputVector.z = inputVector2.y;
        }


        if(inputVector2.magnitude > 0.0f)
        {
            state = AnimationState.Run;
        }
        else
        {
            state = AnimationState.Stand;
        }
        //m_thisRigitBody.AddForce(m_inputVector * moveSpeed);


        //inputVector2 = new Vector2(m_thisRigitBody.velocity.x, m_thisRigitBody.velocity.z);
        //if (inputVector2.magnitude > maxSpeed)
        //{
        //    inputVector2 = inputVector2.normalized * maxSpeed;
        //    m_thisRigitBody.velocity = new Vector3(inputVector2.x, m_thisRigitBody.velocity.y, inputVector2.y);
        //}

        transform.LookAt(transform.position + m_inputVector);
		var t = m_inputVector;

		m_inputVector = m_inputVector * moveSpeed;
        m_inputVector.y = m_thisRigitBody.velocity.y;
		m_thisRigitBody.velocity = m_inputVector;

		var com = GetComponent<PlayerMaualCollisionAdministrator>();
		if (com.isFixedTerritorySegmentStay)
		{
			m_thisRigitBody.velocity += (com.territoryForwardSideNormal * ((t.magnitude * moveSpeed) + 0.01f) );
		}

		animator.SetInteger("State", (int)state);

    }

    private void ShotInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            territoryIntermediary.DirectingServant(0);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            territoryIntermediary.DirectingServant(1);
        }
        if (Input.GetButtonDown("Fire3"))
        {
            territoryIntermediary.DirectingServant(2);
        }
    }

    public void GameClearAnimation()
    {
        animator.SetTrigger("Result");
        state = AnimationState.GameClear;
        animator.SetInteger("State", (int)state);
        isControlInput = false;
    }
    public void GameOverAnimation()
    {
        animator.SetTrigger("Result");
        state = AnimationState.GameOver;
        animator.SetInteger("State", (int)state);
        isControlInput = false;
    }
}
