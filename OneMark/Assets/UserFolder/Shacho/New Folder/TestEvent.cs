using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : BaseEvent
{
	protected override void EndEvent()
	{
	}

	protected override void NearbyIfManualTrigger()
	{
		Debug.Log("doki doki...");
	}

	protected override void StartEvent()
	{
		Debug.Log("Start!!");
	}

	protected override bool UpdateEvent()
	{
		return false;
	}
}
