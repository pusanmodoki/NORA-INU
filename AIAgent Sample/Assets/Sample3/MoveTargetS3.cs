using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTargetS3 : AIComponent.BaseAIFunction
{
	[SerializeField, Tooltip("移動ターゲット")]
	Transform m_target = null;

	//Startみたいなもん
	public override void AIBegin(AIComponent.BaseAIFunction beforeFunction, bool isParallel)
	{
		//navMeshAgent.destination が目標地点
		navMeshAgent.destination = m_target.position;
	}

	//OnDisableみたいなもん
	public override void AIEnd(AIComponent.BaseAIFunction nextFunction, bool isParallel)
	{
	}


	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
	}
}
