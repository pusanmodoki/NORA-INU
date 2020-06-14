using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectToTutorial : MonoBehaviour
{
	static readonly int m_cToSelectTriggerID = Animator.StringToHash("ToSelect");
	static readonly int m_cToNonSelectTriggerID = Animator.StringToHash("ToNonSelect");

	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	Image m_headerImage = null;
	[SerializeField]
	Image m_headerFontImage = null;
	[SerializeField]
	AudioSource m_selectSE = null;
	[SerializeField]
	AudioSource m_enterSE = null;
	[SerializeField]
	string m_inpuitAxis = "";
	[SerializeField]
	float m_moveInputSeconds = 1.0f;
	[SerializeField, Range(0.0f, 1.0f)]
	float m_headerFontFillStart = 0.0f;
	[SerializeField, Range(0.0f, 1.0f)]
	float m_headerFontFillEnd = 1.0f;
	[SerializeField, Space]
	MenuInput[] m_thisSceneInputs = null;

	int[] m_thisSceneInputDisableIDs = null;
	Timer m_timer = new Timer();
	bool m_isMove = false;
	
	void Start()
	{
		m_headerImage.fillAmount = 0.0f;
		m_headerFontImage.fillAmount = 0.0f;

		m_thisSceneInputDisableIDs = new int[m_thisSceneInputs.Length];
	}

	// Update is called once per frame
	void Update()
	{
		if (m_isMove) return;

		if (m_timer.isStart)
		{
			m_headerImage.fillAmount = m_timer.elapasedTime / m_moveInputSeconds;
			m_headerFontImage.fillAmount = 
				Mathf.Clamp01(m_timer.elapasedTime / m_moveInputSeconds - m_headerFontFillStart) 
				/ (m_headerFontFillEnd - m_headerFontFillStart);

			if (!Input.GetButton(m_inpuitAxis))
			{
				m_timer.Stop();
				m_animator.SetTrigger(m_cToNonSelectTriggerID);
				m_headerImage.fillAmount = 0.0f;
				m_headerFontImage.fillAmount = 0.0f;

				for (int i = 0; i < m_thisSceneInputs.Length; ++i)
					m_thisSceneInputs[i].EndDisableEvent(m_thisSceneInputDisableIDs[i]);
			}
			else if (m_timer.elapasedTime >= m_moveInputSeconds)
			{
				if (TutorialUIManager.instance != null)
					TutorialUIManager.instance.ResetTutorial();

				AudioManager.instance.FreePlaySE(m_enterSE);
				OneMarkSceneManager.instance.MoveStageScene(0);
				m_isMove = true;
			}
		}
		else if (Input.GetButton(m_inpuitAxis))
		{
			m_timer.Start();
			m_animator.SetTrigger(m_cToSelectTriggerID);
			AudioManager.instance.FreePlaySE(m_selectSE);

			for (int i = 0; i < m_thisSceneInputs.Length; ++i)
			{
				int outID;
				m_thisSceneInputs[i].StartDisableEvent(out outID);
				m_thisSceneInputDisableIDs[i] = outID;
			}
		}
	}
}
