using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
	public enum FadeState
	{
		Null,
		Fadein,
		Fadeout
	}

    public float fadeSpeed { get; protected set; } = 1.0f;

	public bool isCompletedTransition { get; protected set; } = false;

    public float alpha { get; protected set; } = 0.0f;

	public FadeState fadeState { get; protected set; } = FadeState.Null;

	public bool isAutoDisabled { get; protected set; } = false;

	[SerializeField]
	Canvas m_canvas = null;
	[SerializeField]
	Image m_screenImage = null;

	// Update is called once per frame
	void Update()
    {
		if (fadeState == FadeState.Fadeout)
		{
			alpha -= Time.deltaTime / fadeSpeed;

			if (alpha <= 0.0f)
			{
				alpha = 0.0f;
				fadeState = FadeState.Null;
				isCompletedTransition = true;
				if (isAutoDisabled) m_canvas.enabled = false;
			}

			m_screenImage.color = new Color(m_screenImage.color.r, m_screenImage.color.g, m_screenImage.color.b, alpha);
		}
		else if (fadeState == FadeState.Fadein)
		{
			alpha += Time.deltaTime / fadeSpeed;

			if (alpha >= 1.0f)
			{
				alpha = 1.0f;
				fadeState = FadeState.Null;
				isCompletedTransition = true;
				if (isAutoDisabled) m_canvas.enabled = false;
			}

			m_screenImage.color = new Color(m_screenImage.color.r, m_screenImage.color.g, m_screenImage.color.b, alpha);
		}
    }

	public void SetFadeIn(Color fadeColor)
	{
		if (!m_canvas.enabled) m_canvas.enabled = true;
		m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		m_canvas.sortingOrder = 100;

		m_screenImage.rectTransform.anchoredPosition = Vector3.zero;
		m_screenImage.rectTransform.sizeDelta = new Vector2(9999, 9999);

		fadeState = FadeState.Null;
		isAutoDisabled = false;
		isCompletedTransition = false;
		alpha = 1.0f;

		m_screenImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1.0f);
	}

	public void OnFadeScreen(Color fadeColor, float fadeSpeed, FadeState fadeState, bool isAutoDisabled, float alpha = -1)
    {
		if (!m_canvas.enabled) m_canvas.enabled = true;
		m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		m_canvas.sortingOrder = 100;	

		m_screenImage.rectTransform.anchoredPosition = Vector3.zero;
		m_screenImage.rectTransform.sizeDelta = new Vector2(9999, 9999);

		if (alpha == -1)
		{
			if (fadeState == FadeState.Fadein) alpha = 0.0f;
			else if (fadeState == FadeState.Fadeout) alpha = 1.0f;
		}
		m_screenImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);

		this.fadeSpeed = fadeSpeed;
		this.fadeState = fadeState;
		this.isAutoDisabled = isAutoDisabled;
		isCompletedTransition = false;
	}
}
