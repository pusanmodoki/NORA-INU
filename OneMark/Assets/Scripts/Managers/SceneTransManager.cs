using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransManager : MonoBehaviour
{
    [SerializeField]
    private string g_nowSceneName = "";

    static public string nowSceneName { get; private set; }

    [SerializeField]
    private string g_nextSceneName = "";

    static public string nextSceneName { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        nowSceneName = g_nowSceneName = SceneManager.GetActiveScene().name;
        nextSceneName = g_nextSceneName;
    }
}
