using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransButton : MonoBehaviour
{
    [SerializeField]
    string sceneName = "";

    public string m_sceneName{ get { return sceneName; } set { sceneName = value; } }

    [SerializeField]
    Color fadeColor = Color.white;

    [SerializeField]
    float fadeSpeed = 1.0f;
    public void OnButtonSceneTrans()
    {
        SceneFadeTrans.SimpleFadeTrans(sceneName, fadeColor, fadeSpeed);
    }
}
