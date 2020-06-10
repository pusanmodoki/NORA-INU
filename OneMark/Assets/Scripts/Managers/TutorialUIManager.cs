using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

//[DefaultExecutionOrder(-10)]
public class TutorialUIManager : MonoBehaviour
{
	[System.Serializable]
	public class TextPreset
	{
		[System.Serializable]
		struct Condition
		{
			[SerializeField]
			TutorialConditions.Conditions[] m_andConditions;

			public TutorialConditions.Conditions this[int index] { get { return m_andConditions[index]; } }
			public int length { get { return m_andConditions.Length; } }

			public Condition(TutorialConditions.Conditions[] andConditions)
			{
				m_andConditions = andConditions;
			}
		}

		public string text { get { return m_text; } }
		public bool isComplete { get; private set; }

		[SerializeField, Multiline(2)]
		string m_text = "";
		[SerializeField]
		Condition[] m_orConditions = null;

		public bool IsCondition(TutorialConditions conditions)
		{
			if (isComplete) return false;

			bool isResult = false;

			for (int i = 0; i < m_orConditions.Length; ++i)
			{
				bool isAndResult = true;
				for (int k = 0; k < m_orConditions[i].length; ++k)
					isAndResult &= conditions.IsCondition(m_orConditions[i][k]);

				isResult |= isAndResult;
			}

			return isResult;
		}

		public void OnUI() { isComplete = true; }
	}

	/// <summary>Static instance</summary>
	public static TutorialUIManager instance { get; private set; } = null;
	static bool m_isCreateInstance = false;

	public Animator dogAnimator { get; private set; } = null;
	public int onWaitTutorials { get { return m_onWaitTutorials.Count; } }
	public bool isOnTutorial { get { return m_tutorialUI.isOnTutorial || m_onWaitTutorials.Count > 0; } }

	[SerializeField]
	TutorialUI m_tutorialUI = null;
	[SerializeField]
	GameObject m_tutorialModelAndCamera = null;
	[SerializeField]
	TutorialConditions m_tutorialConditions = null;
	[SerializeField]
	Behaviour[] m_enableImages = null;
	[SerializeField]
	TextPreset[] m_textPresets = null; 

	List<int> m_onWaitTutorials = new List<int>();

	public void GameStart()
	{
		m_tutorialConditions.isGameStart = true;
	}

	/// <summary>[Start]</summary>
	void Start()
	{
		if (!m_isCreateInstance)
		{
			m_isCreateInstance = true;
			instance = this;
			DontDestroyOnLoad(gameObject);

			for (int i = 0, length = m_enableImages.Length; i < length; ++i)
				m_enableImages[i].enabled = false;

			var obj = Instantiate(m_tutorialModelAndCamera);
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, OneMarkSceneManager.instance.nowScene);
			dogAnimator = obj.GetComponentInChildren<Animator>();

			m_tutorialConditions.StartCondition();
		}
		else
		{
			instance.m_tutorialConditions.StartCondition();

			var obj = Instantiate(m_tutorialModelAndCamera);
			UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, OneMarkSceneManager.instance.nowScene);
			instance.dogAnimator = obj.GetComponentInChildren<Animator>();

			Destroy(gameObject);
		}
	}

    // Update is called once per frame
    void Update()
    {
		for (int i = 0, length = m_textPresets.Length; i < length; ++i)
		{
			if (!m_onWaitTutorials.Contains(i) && m_textPresets[i].IsCondition(m_tutorialConditions))
				m_onWaitTutorials.Add(i);
		}

		if (m_tutorialUI.isOnTutorial && !m_tutorialUI.OnTutorialUpdate())
		{
			if (m_onWaitTutorials.Count > 0)
			{
				m_tutorialUI.EnableTutorial(m_textPresets[m_onWaitTutorials[0]], true);
				m_onWaitTutorials.RemoveAt(0);
			}
			else
			{
				m_tutorialUI.DisableTutorial();
				for (int i = 0, length = m_enableImages.Length; i < length; ++i)
					m_enableImages[i].enabled = false;
			}
		}
		else if (!m_tutorialUI.isOnTutorial && m_onWaitTutorials.Count > 0)
		{
			m_tutorialUI.EnableTutorial(m_textPresets[m_onWaitTutorials[0]], false);
			m_onWaitTutorials.RemoveAt(0);

			for (int i = 0, length = m_enableImages.Length; i < length; ++i)
				m_enableImages[i].enabled = true;
		}
	}
}
