using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransButton : MonoBehaviour
{
    [SerializeField]
    string sceneName = "";
    public void OnButtonSceneTrans()
    {
        SceneManager.LoadScene(sceneName);
    }
}
