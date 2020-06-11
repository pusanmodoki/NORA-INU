using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvents : TriggerEvent
{
	int m_disableAction = -1;

	public override void OnTrigger(string key)
	{
		switch (key)
		{
			case "DisableShot":
				if (m_disableAction == -1)
					PlayerAndTerritoryManager.instance.mainPlayer.input.StartDisableActionInput(out m_disableAction);
				break;
			case "EnableShot":
				if (m_disableAction != -1)
					PlayerAndTerritoryManager.instance.mainPlayer.input.EndDisableActionInput(m_disableAction);
				break;
		}
	}
}
