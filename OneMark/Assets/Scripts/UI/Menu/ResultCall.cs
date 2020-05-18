using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ResultCall
{
    static public void GameClear()
    {
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameClear", true);
    }
    static public void GameOver()
    {
		OneMarkSceneManager.instance.SetActiveAccessoryScene("GameOver", true);
    }
}
