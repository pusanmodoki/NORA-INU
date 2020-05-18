using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTitleButton : MonoBehaviour
{
	public void OnButton()
	{
		OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
	}
}
