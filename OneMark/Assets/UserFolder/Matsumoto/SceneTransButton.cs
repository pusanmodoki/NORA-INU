using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransButton : MonoBehaviour
{
    [SerializeField]
    Vector2Int stageNum = Vector2Int.zero;
    public void OnButtonSceneTrans()
    {
#if UNITY_EDITOR
		Debug.Log("旧バージョンです");
#endif
        OneMarkSceneManager.instance.MoveStageScene(stageNum);
        //SceneFadeTrans.SimpleFadeTrans(sceneName, fadeColor, fadeSpeed);
    }
}
