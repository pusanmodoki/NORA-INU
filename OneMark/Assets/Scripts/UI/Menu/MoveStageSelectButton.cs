using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStageSelectButton : MonoBehaviour
{
	public void OnButton()
	{
		OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.StageSelect);
	}
}
