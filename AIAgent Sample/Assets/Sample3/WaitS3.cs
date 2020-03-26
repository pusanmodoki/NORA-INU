using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitS3 : AIComponent.BaseAIFunction
{
	//Startみたいなもん
	public override void AIBegin(AIComponent.BaseAIFunction beforeFunction, bool isParallel)
	{
	}

	//OnDisableみたいなもん
	public override void AIEnd(AIComponent.BaseAIFunction nextFunction, bool isParallel)
	{
	}

	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		//回転
		transform.Rotate(new Vector3(0, 100, 0) * Time.deltaTime);
	}
}
