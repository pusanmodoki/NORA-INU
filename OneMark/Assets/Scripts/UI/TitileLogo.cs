﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitileLogo : MonoBehaviour
{
    public void ToOpening()
    {
        OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Opening);
    }

    public void ToTitle()
    {
        OneMarkSceneManager.instance.MoveScene(OneMarkSceneManager.SceneState.Title);
    }
}
