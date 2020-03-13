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



    private Vector3 inputVector = Vector3.zero;

    private float inputAngleRad = 0.0f;

	// Update is called once per frame
	void Update() {
	}

	private void FixedUpdate()
    {
		MoveInput();
	}


    private void InputPlayer()
    {
        inputVector = Vector3.zero;

        if (isControler)
        {
            inputVector.x = Input.GetAxis("Horizontal");
            inputVector.z = Input.GetAxis("Vertical");

            inputAngleRad = Mathf.Atan2(inputVector.z, inputVector.x);
        }
        else
        {
            Vector2 inputVector2 = Vector2.zero;

            inputVector2.x = Input.GetAxis("Horizontal");
            inputVector2.y = Input.GetAxis("Vertical");

            inputVector2.Normalize();

            inputVector.x = inputVector2.x;
            inputVector.z = inputVector2.y;

            inputAngleRad = Mathf.Atan2(inputVector.z, inputVector.x);
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void MoveInput()
    {
        if (isControler)
        {
            Vector3 moveVector = Vector3.zero;

            moveVector.x = Input.GetAxis("Horizontal");
            moveVector.z = Input.GetAxis("Vertical");
            thisRigitBody.AddForce(moveVector * moveSpeed);

            float rad = 0.0f;

            rad = Mathf.Atan2(moveVector.z, moveVector.x);


        }
        else
        {
            Vector2Int moveVector = Vector2Int.zero;

            moveVector.x = (int)Input.GetAxisRaw("Horizontal");
            moveVector.y = (int)Input.GetAxisRaw("Vertical");

            
        }
    }
}
