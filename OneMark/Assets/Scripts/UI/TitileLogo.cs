using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitileLogo : MonoBehaviour
{
    private bool Title_Flg = false;

    public void ToOpening()
    {
        OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Opening);
    }

    public void ToTitle()
    {
        if(Title_Flg==false)
        {
            OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
            Title_Flg = true;
        }
    }
}
