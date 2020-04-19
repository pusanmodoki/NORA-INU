using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// OffMeshLinkの操作を行うDogOffMeshLinkController
/// 参考にしたサイト: https://www.urablog.xyz/entry/2017/10/16/145350
/// </summary>
[System.Serializable]
public class DogOffMeshLinkController
{
	/// <summary>Jumpにかかる時間</summary>
	[SerializeField, Tooltip("Jumpにかかる時間")]
	float m_jumpSeconds = 1.0f;

	/// <summary>This NavMeshAgent</summary>
	NavMeshAgent m_navMeshAgent = null;
	/// <summary>This Transform</summary>
	Transform m_transform = null;
	/// <summary>This Rigidbody</summary>
	Rigidbody m_rigidBody = null;
	/// <summary>This Rigidbody</summary>
	Timer m_timer = new Timer();
	/// <summary>ジャンプ目標座標</summary>
	Vector3 m_targetPosition = Vector3.zero;
	/// <summary>ジャンプ目標座標</summary>
	bool m_isStartOffMeshLink = false;

	/// <summary>
	/// [InitContoroller]
	/// 初期化を行う
	/// 引数1: This Agent
	/// </summary>
	public void InitContoroller(DogAIAgent thisAgent)
	{
		m_navMeshAgent = thisAgent.navMeshAgent;
		m_transform = thisAgent.transform;
		m_rigidBody = thisAgent.rigidBody;

		m_rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		m_rigidBody.isKinematic = true;
		m_rigidBody.useGravity = true;
	}
	/// <summary>[Update]</summary>
	public void Update()
	{
		//OffMeshLink上にいなければ終了
		if (!m_navMeshAgent.isOnOffMeshLink)
			return;

		//初期化
		if (!m_isStartOffMeshLink)
		{
			//RigidBodyのKinematic解除
			m_rigidBody.isKinematic = false;
			m_rigidBody.velocity = Vector3.zero;

			//NavMesh停止, 初期化フラグtrueに
			m_navMeshAgent.isStopped = true;
			m_isStartOffMeshLink = true;

			//TargetPosition設定
			OffMeshLinkData data = m_navMeshAgent.currentOffMeshLinkData;
			m_targetPosition = data.endPos;
			m_targetPosition.y += m_transform.position.y - data.startPos.y;

			//ジャンプ実行
			JumpExecution();
			//タイマースタート
			m_timer.Start();
		}

		//一定時間経過で終了
		if (m_timer.isStart && m_timer.elapasedTime >= m_jumpSeconds)
			EndOffMeshLink();
	}
	/// <summary>[FixedUpdate]</summary>
	public void FixedUpdate()
	{
		if (m_timer.isStart && m_timer.elapasedTime >= m_jumpSeconds)
			EndOffMeshLink();
	}


	/// <summary>
	/// [JumpExecution]
	/// ジャンプを実行する
	/// </summary>
	void JumpExecution()
	{
		//放物線計算に使うdetail を取得
		Vector2 detail = CalculateDetail();
		//スピードを計算
		float speed = CalculateSpeed(ref detail);
		//角度を計算
		float angle = CalculateAngle(ref detail);

		//スピードが0以下であればジャンプしない
		if (speed <= 0.0f)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! DogOffMeshLinkController->StartJump\nJump failed");
#endif
			return;
		}

		//AddForce
		m_rigidBody.AddForce(
			CalculateForce(speed, angle) * m_rigidBody.mass, ForceMode.Impulse);
	}
	/// <summary>
	/// [EndOffMeshLink]
	/// OffMeshLink処理を終了する
	/// </summary>
	void EndOffMeshLink()
	{
		//フラグ初期化, タイマーストップ
		m_isStartOffMeshLink = false;
		m_timer.Stop();
		//RigidBodyをKinematicに、Velocity = 0
		m_rigidBody.isKinematic = true;
		m_rigidBody.velocity = Vector3.zero;
		//OffMeshLink終了, Stop解除
		m_navMeshAgent.CompleteOffMeshLink();
		m_navMeshAgent.isStopped = false;
	}


	/// <summary>
	/// [CalculateDetail]
	/// 放物線計算に使用するDetailを計算する
	/// </summary>
	Vector2 CalculateDetail()
	{
		float distance = Vector2.Distance(new Vector2(m_targetPosition.x, m_targetPosition.z),
			new Vector2(m_transform.position.x, m_transform.position.z));

		return new Vector2(distance / m_jumpSeconds,
			(m_targetPosition.y - m_transform.position.y) / m_jumpSeconds + (-Physics.gravity.y * m_jumpSeconds) / 2);
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 放物線計算に使用するスピードを計算する
	/// 引数1: detail
	/// </summary>
	float CalculateSpeed(ref Vector2 detail)
	{
		//0以下であれば終了
		if (m_jumpSeconds <= 0.0f)
			return 0.0f;

		//dot計算
		float dot = detail.x * detail.x + detail.y * detail.y;
		
		//平方根計算
		if (dot > 0.0f) return Mathf.Sqrt(dot);
		else return 0.0f;
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 放物線計算に使用する角度を計算する
	/// 引数1: detail
	/// </summary>
	float CalculateAngle(ref Vector2 detail)
	{
		//Atan2
		return Mathf.Atan2(detail.y, detail.x) * Mathf.Rad2Deg;
	}
	/// <summary>
	/// [CalculateForce]
	/// RigidBodyに与える力を計算する
	/// 引数1: speed
	/// 引数1: angle
	/// </summary>
	Vector3 CalculateForce(float speed, float angle)
	{
		//This position
		Vector3 position = m_transform.position;
		//目標座標
		Vector3 targetPosition = m_targetPosition;
		//XZ平面の値がほしいのでYを0にする
		position.y = targetPosition.y = 0.0f;

		//yaw angle * x angle * x speed
		return Quaternion.FromToRotation(Vector3.right, (targetPosition - position).normalized)
			* Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(speed, 0.0f, 0.0f);
	}
}
