using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class MoveTargetS1 : AIComponent.BaseAIFunction
{
	[SerializeField, Tooltip("移動ターゲット")]
	Transform m_target = null;
	[SerializeField, Tooltip("ゴールに着いたと判断する半径")]
	float m_goalRadius = 0.5f;
	[SerializeField, Tooltip("タイムアウトと判断する秒数")]
	float m_timeoutSeconds = 5.0f;

	//Startみたいなもん
	public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
	{
		//navMeshAgent.destination が目標地点
		navMeshAgent.destination = m_target.position;
	}

	//OnDisableみたいなもん
	public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
	{
	}

	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		//ターゲットとの距離がradius以下になったら終了
		//もしくは、タイムアウト秒数まで実行から経過したら終了
		//timer.elapsedTimeはAIBeginがよびだされてから経過した秒数です
		//EndAIFunctionが終了関数, これか再割当てがなければ終了しません
		if (timer.elapasedTime > m_timeoutSeconds || 
			(m_target.position - transform.position).sqrMagnitude <= m_goalRadius * m_goalRadius)
		{
			Debug.Log("Goal!!");
			EndAIFunction(updateIdentifier);
		}
	}
}
