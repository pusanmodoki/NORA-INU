using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
	[SerializeField]
	MenuInput m_menuInput = null;
	[SerializeField]
	DeleteDataButton m_deleteDataButton = null;
	[SerializeField]
	AudioSource m_enterSource = null;

	bool m_isClose = false;

	void OnEnable()
	{
		m_menuInput.ForceSelect(0);

		if (OneMarkSceneManager.instance.isNowStageScene)
			m_deleteDataButton.gameObject.SetActive(false);
		else
			m_deleteDataButton.gameObject.SetActive(true);
	}

	// Update is called once per frame
	void Update()
    {
		if (m_isClose)
		{
			m_isClose = false;
			AudioManager.instance.FreePlaySE(m_enterSource);
			m_deleteDataButton.CheckEndPushAudio();
			OneMarkSceneManager.instance.SetActiveOptionScene(false);
		}

		if (Input.GetButtonDown("StageSelectToTitle"))
			m_isClose = true;
	}
}
