using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransButton : MonoBehaviour
{
    public void OnButtonSceneTrans()
    {
#if UNITY_EDITOR
		Debug.Log("旧バージョンです");
#endif
		//OneMarkSceneManager.instance.MoveScene(m_transitionSceneName);
		//SceneFadeTrans.SimpleFadeTrans(sceneName, fadeColor, fadeSpeed);
	}
}
