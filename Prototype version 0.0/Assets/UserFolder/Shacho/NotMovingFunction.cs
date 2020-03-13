using System.Collections;
using System.Collections.Generic;
using AIComponent;
using UnityEngine;

public class NotMovingFunction : AIComponent.BaseAIFunction
{
	public override void AIBegin(BaseAIFunction beforeFunction, bool isParallel)
	{
		SetUpdatePosition(false);
	}

	public override void AIEnd(BaseAIFunction nextFunction, bool isParallel)
	{
	}

	public override void AIUpdate(UpdateIdentifier updateIdentifier)
	{
		EndAIFunction(updateIdentifier);
	}
	
}
