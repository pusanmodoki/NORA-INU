using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    public float fadeSpeed { get; protected set; } = 1.0f;

    public bool isFadeout { get; protected set; } = false;

    public bool isFadein { get; protected set; } = false;

    public float alpha { get; protected set; } = 0.0f;

    public Canvas canvas { get; protected set; } = null;

    public Image screen { get; protected set; } = null;

    public string nextSceneName { get; protected set; } = "";

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFadein)
        {
            alpha -= Time.deltaTime / fadeSpeed;

            if (alpha <= 0.0f)
            {
                isFadein = false;
                alpha = 0.0f;
                Destroy(gameObject);
                return;
            }
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
        }

        // フェードアウト処理
        else if (isFadeout)
        {
            alpha += Time.deltaTime / fadeSpeed;

            if (alpha >= 1.0f)
            {
                isFadeout = false;
                isFadein = true;
                alpha = 1.0f;

                SceneManager.LoadScene(nextSceneName);
                return;
                //SceneManager.LoadScene(nextScene);
            }
            screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, alpha);
        }
    }

    public void OnFadeScreen(string _nextSceneName, Color _fadeColor, float _fadeSpeed)
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject screenObject = new GameObject("Screen");
        screen = screenObject.AddComponent<Image>();

        screen.rectTransform.anchoredPosition = Vector3.zero;
        screen.rectTransform.sizeDelta = new Vector2(9999, 9999);

        screen.color = new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, 0.0f);

        screenObject.transform.SetParent(transform, false);
        isFadeout = true;
        isFadein = false;

        nextSceneName = _nextSceneName;

        DontDestroyOnLoad(gameObject);
    }
}
