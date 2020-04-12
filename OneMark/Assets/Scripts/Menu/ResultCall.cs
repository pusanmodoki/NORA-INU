using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultCall : MonoBehaviour
{
    static public void GameClear()
    {
        SceneManager.LoadScene("GameClear", LoadSceneMode.Additive);
    }
    static public void GameOver()
    {
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }
}
