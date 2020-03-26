using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionS4 : AIComponent.BaseAICondition
{
	//こいつがtrueを返すとテーブルに入る
	public override bool IsCondition()
	{
		return true;
	}
}
