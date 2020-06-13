using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTutorialTriggerEvent : TriggerEvent
{
	public override void OnTrigger(string key)
	{
		if (key == cDefaultEnable)
		{
			TutorialUIManager.instance.ForceEndTutorial();
		}
	}
}
