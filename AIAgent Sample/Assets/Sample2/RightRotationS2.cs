using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class RightRotationS2 : AIComponent.BaseAIFunction
{
	//Startみたいなもん
	public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
	{
		Debug.Log("Start right rotation");
	}

	//OnDisableみたいなもん
	public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
	{
		Debug.Log("End right rotation");
	}

	//Updateみたいなもん
	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		//回転
		transform.Rotate(new Vector3(0, -100, 0) * Time.deltaTime);
	}
}
