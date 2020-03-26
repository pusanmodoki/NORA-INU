using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class LeftRotationS2 : AIComponent.BaseAIFunction
{
	//Startみたいなもん
	public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
	{
		Debug.Log("Start left rotation");
	}

	//OnDisableみたいなもん
	public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
	{
		Debug.Log("End left rotation");
	}

	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		//回転
		transform.Rotate(new Vector3(0, 100, 0) * Time.deltaTime);
	}
}
