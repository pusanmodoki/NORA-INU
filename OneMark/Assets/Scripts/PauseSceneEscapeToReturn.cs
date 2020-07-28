using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSceneEscapeToReturn : TriggerEvent
{
	[SerializeField]
	AudioSource m_enterSE = null;

	bool m_isOpenOption = false;
	bool m_isStart = false;

	public override void OnTrigger(string key)
	{
		if (key == cDefaultEnable)
		{
	//		m_input.ResetNowSelectIndex();
			AudioManager.instance.FreePlaySE(m_enterSE);
			MainGameManager.instance.SetPauseStayFalse();
		}
	}

	void Start()
	{
		OneMarkSceneManager.instance.openOptionCallback += OpenOption;
		OneMarkSceneManager.instance.closeOptionCallback += CloseOption;
	}

	void OnEnable()
	{
		if (MainGameManager.instance != null && MainGameManager.instance.isPauseStay)
		{
			m_isStart = true;
			AudioManager.instance.FreePlaySE(m_enterSE);
		}
	}

	// Update is called once per frame
	void LateUpdate()
    {
		if (!MainGameManager.instance.isPauseStay | m_isOpenOption)
			return;

		if (m_isStart)
		{
			m_isStart = false;
			return;
		}

        if (Input.GetButtonDown("ActionPause"))
		{
			OnTrigger(cDefaultEnable);
			OneMarkSceneManager.instance.SetActiveAccessoryScene(
				gameObject.scene.name, false);
		}
    }

	void OpenOption() { m_isOpenOption = true; }
	void CloseOption() { m_isOpenOption = false; }
}
