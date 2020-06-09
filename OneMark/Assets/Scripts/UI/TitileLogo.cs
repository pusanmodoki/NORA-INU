using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitileLogo : MonoBehaviour
{
    public void ToTitle()
    {
        OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
    }
}
