using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeTrans
{
    public static void SimpleFadeTrans(string nextSceneName, Color fadeScreenColor, float fadeSpeed)
    {
        GameObject screenObject = new GameObject("ScreenCanvas");
        FadeScreen screen = screenObject.AddComponent<FadeScreen>();
        screen.OnFadeScreen(nextSceneName, fadeScreenColor, fadeSpeed);
    }
}
