using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
	public void OnButton()
	{
		OneMarkSceneManager.instance.MoveStageScene(0);
#if UNITY_EDITOR
		Debug.Log("仮実装です");
#endif
		//SceneFadeTrans.SimpleFadeTrans(sceneName, fadeColor, fadeSpeed);
	}
}
