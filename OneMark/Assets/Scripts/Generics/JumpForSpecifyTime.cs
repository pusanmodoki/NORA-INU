using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JumpForSpecifyTime
{
	/// <summary>
	/// [JumpExecution]
	/// ジャンプを実行する
	/// </summary>
	public static void JumpExecution(Rigidbody rigidBody, Vector3 position, Vector3 moveTarget, float jumpSeconds)
	{
		//y0 positions
		Vector3 y0Position = position;
		Vector3 y0MoveTarget = moveTarget;
		y0Position.y = y0MoveTarget.y = 0.0f;

		//放物線計算に使うdetail を取得
		Vector2 detail = CalculateDetail(ref position, ref moveTarget, ref y0Position, ref y0MoveTarget, jumpSeconds);
		//スピードを計算
		float speed = CalculateSpeed(ref detail, jumpSeconds);
		//角度を計算
		float angle = CalculateAngle(ref detail);

		//スピードが0以下であればジャンプしない
		if (speed <= 0.0f)
		{
#if UNITY_EDITOR
			Debug.LogError("Error!! OffMeshJumpingBoard->JumpExecution\nJump failed");
#endif
			return;
		}

		//AddForce
		rigidBody.AddForce(
			CalculateForce(ref y0Position, ref y0MoveTarget, speed, angle) * rigidBody.mass, ForceMode.Impulse);
	}
	/// <summary>
	 /// [CalculateDetail]
	 /// 放物線計算に使用するDetailを計算する
	 /// 引数1: moveTarget
	 /// 引数2: agent position (y = 0)
	 /// 引数3: target position (y = 0)
	 /// </summary>
	static Vector2 CalculateDetail(ref Vector3 position, ref Vector3 moveTarget, ref Vector3 y0Position, ref Vector3 y0MoveTarget, float jumpSeconds)
	{
		float distance = Vector2.Distance(new Vector2(y0MoveTarget.x, y0MoveTarget.z),
			new Vector2(y0Position.x, y0Position.z));

		return new Vector2(distance / jumpSeconds,
			(moveTarget.y - position.y) / jumpSeconds + (-Physics.gravity.y * jumpSeconds) / 2);
	}
	/// <summary>
	/// [CalculateSpeed]
	/// 放物線計算に使用するスピードを計算する
	/// 引数1: detail
	/// </summary>
	static float CalculateSpeed(ref Vector2 detail, float jumpSeconds)
	{
		//0以下であれば終了
		if (jumpSeconds <= 0.0f) return 0.0f;

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
	static float CalculateAngle(ref Vector2 detail)
	{
		//Atan2
		return Mathf.Atan2(detail.y, detail.x) * Mathf.Rad2Deg;
	}
	/// <summary>
	/// [CalculateForce]
	/// RigidBodyに与える力を計算する
	/// 引数1: agent position (y = 0)
	/// 引数2: target position (y = 0)
	/// 引数3: speed
	/// 引数4: angle
	/// </summary>
	static Vector3 CalculateForce(ref Vector3 y0Position, ref Vector3 y0MoveTarget, float speed, float angle)
	{
		//yaw angle * x angle * x speed
		return Quaternion.FromToRotation(Vector3.right, (y0MoveTarget - y0Position).normalized)
			* Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(speed, 0.0f, 0.0f);
	}
}
