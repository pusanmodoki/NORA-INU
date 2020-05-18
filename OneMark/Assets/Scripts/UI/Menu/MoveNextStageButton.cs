using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNextStageButton : MonoBehaviour
{
	public void OnButton()
	{
		OneMarkSceneManager.instance.MoveNextStage();
	}
}
