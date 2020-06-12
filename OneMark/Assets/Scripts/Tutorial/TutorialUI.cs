using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class TutorialUI : MonoBehaviour
{
	enum State
	{
		Start,
		TextScaling,
		TextDrawing,
		Update,
		End
	}

	int m_cStateID = Animator.StringToHash("State");
	int m_cStateWait = 0;
	int m_cStateAction = 1;

	public TutorialUIManager.TextPreset textPreset { get; private set; } = null;
	public float drawSpeedPerSeconds { get; private set; } = 0.0f;
	public int uiInstanceID { get; private set; } = -1;
	public bool isOnTutorial { get; private set; } = false;

	[SerializeField]
	RectTransform m_textBox = null;
	[SerializeField]
	RectTransform m_aButton = null;
	[SerializeField]
	RectTransform m_dogTransform = null;
	[SerializeField]
	AudioSource m_se = null;
	[SerializeField]
	UnityEngine.UI.Text m_text = null;
	[SerializeField]
	Vector3 m_startPoint = Vector3.zero;

	[SerializeField, Space]
	float m_firstMoveSeconds = 1.0f;
	[SerializeField]
	float m_firstMoveWaitSeconds = 0.1f;
	[SerializeField, Space]
	float m_jumpTargetHeight = 20.0f;
	[SerializeField]
	float m_jumpMoveSeconds = 1.0f;
	[SerializeField]
	int m_numJump = 4;

	[SerializeField, Space]
	float m_scalingTextBoxSeconds = 1.0f;
	[SerializeField]
	int m_drawSpeedPerSeconds = 3;
	[SerializeField]
	float m_pushButtonWaitSeconds = 0.2f;
	[SerializeField]
	float m_aButtonJumpSeconds = 0.5f;
	[SerializeField]
	float m_aButtonJumpHeight = 30.0f;

	[SerializeField, Space]
	float m_removeSeconds = 0.2f;
	[SerializeField]
	float m_removeJumpSeconds = 0.1f;
	[SerializeField]
	float m_removeJumpHeight = 20.0f;

	Vector3 m_firstPosition = Vector3.zero;
	Vector3 m_aButtonFirstPosition = Vector3.zero;
	Timer m_timer0 = new Timer();
	Timer m_timer1 = new Timer();
	Timer m_timer2 = new Timer();
	State m_state = State.Start;
	int m_disableInputID = -1;
	int m_subState = 0;
	int m_textLength = 0;
	int m_textIndex = 0;

	public void EnableTutorial(TutorialUIManager.TextPreset textPreset, bool isJumpStart)
	{
		TutorialUIManager.instance.dogAnimator.SetInteger(m_cStateID, m_cStateWait);

		this.textPreset = textPreset;
		textPreset.OnUI();
		m_textLength = textPreset.text.Length;

		m_subState = 0;
		m_textIndex = 0;
		isOnTutorial = true;
		m_text.text = "";
		m_text.enabled = true;
		m_textBox.localScale = Vector3.zero;
		m_aButton.localScale = Vector3.zero;
		m_timer0.Start();

		if (isJumpStart)
		{
			m_state = State.TextScaling;
		}
		else
		{
			m_state = State.Start;
			m_dogTransform.position = m_startPoint;
		}

		if (m_disableInputID == -1)
			PlayerAndTerritoryManager.instance.mainPlayer.input.StartDisableInput(out m_disableInputID);
	}
	public void DisableTutorial()
	{
		if (!isOnTutorial) return;

		isOnTutorial = false;
		m_text.enabled = false;
		PlayerAndTerritoryManager.instance.mainPlayer.input.EndDisableInput(m_disableInputID);
		m_disableInputID = -1;
	}

	public bool OnTutorialUpdate()
	{
		if (TutorialUIManager.instance.dogAnimator == null) return true;

		switch(m_state)
		{
			case State.Start:
				{
					Vector3 setPosition = Vector3.Lerp(m_startPoint, m_firstPosition, m_timer0.elapasedTime / m_firstMoveSeconds);

					setPosition.y = m_startPoint.y + Mathf.Sin(
						Mathf.Lerp(0, Mathf.PI * 0.5f, m_timer0.elapasedTime / m_firstMoveSeconds)) * (m_firstPosition.y - m_startPoint.y);

					m_dogTransform.position = setPosition;

					if (m_timer0.elapasedTime >= m_firstMoveSeconds)
					{
						m_dogTransform.position = m_firstPosition;
						m_state = State.TextScaling;
						m_timer0.Start();
					}

					break;
				}
			case State.TextScaling:
				{
					if (m_subState == 0)
					{
						m_subState = 1;
						m_timer1.Start();
					}

					m_textBox.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
						Mathf.Clamp01(m_timer1.elapasedTime / m_scalingTextBoxSeconds));

					if (m_timer0.elapasedTime >= m_firstMoveWaitSeconds
						&&  m_timer1.elapasedTime >= m_scalingTextBoxSeconds)
					{
						TutorialUIManager.instance.dogAnimator.SetInteger(m_cStateID, m_cStateAction);

						m_state = State.TextDrawing;
						m_subState = 1;
						m_timer0.Start();
						m_timer1.Stop();
					}
					break;
				}
			case State.TextDrawing:
				{
					if (m_subState < m_numJump && m_timer0.elapasedTime < m_jumpMoveSeconds)
					{
						m_dogTransform.position = m_dogTransform.position.ToYManual(m_firstPosition.y + Mathf.Sin(
						  Mathf.Lerp(0, Mathf.PI, m_timer0.elapasedTime / m_jumpMoveSeconds)) * m_jumpTargetHeight);
					}
					else
					{
						m_dogTransform.position = m_firstPosition;
						m_timer0.Start();
						++m_subState;
					}

					if (m_timer1.elapasedTime > 1.0f / m_drawSpeedPerSeconds)
					{
						m_text.text += textPreset.text[m_textIndex++];
						m_timer1.Start();
					}

					if (m_text.text.Length == m_textLength)
					{
						m_state = State.Update;
						m_timer1.Start();
						m_timer2.Start();
					}
					break;
				}
			case State.Update:
				{
					if (m_subState < m_numJump && m_timer0.elapasedTime < m_jumpMoveSeconds)
					{
						m_dogTransform.position = m_dogTransform.position.ToYManual(m_firstPosition.y + Mathf.Sin(
						  Mathf.Lerp(0, Mathf.PI, m_timer0.elapasedTime / m_jumpMoveSeconds)) * m_jumpTargetHeight);
					}
					else
					{
						m_dogTransform.position = m_firstPosition;
						m_timer0.Start();
						++m_subState;
					}

					{
						Vector3 setPosition = m_aButton.position;
						setPosition.y = m_aButtonFirstPosition.y + Mathf.Sin(Mathf.PI * 0.5f *
							Mathf.PingPong(m_timer1.elapasedTime / m_aButtonJumpSeconds, 1.0f)) * m_aButtonJumpHeight;

						m_aButton.position = setPosition;
					}

					m_aButton.localScale = Vector3.Lerp(Vector3.zero, Vector3.one,
						Mathf.Clamp01(m_timer2.elapasedTime / (m_pushButtonWaitSeconds * 0.5f)));

					if (m_timer2.elapasedTime > m_pushButtonWaitSeconds 
						&& !MainGameManager.instance.isPauseStay && Input.GetButtonDown("Fire3"))
					{
						m_dogTransform.position = m_firstPosition;
						m_state = State.End;
						m_subState = 0;
						m_se.PlayOneShot(m_se.clip);
						TutorialUIManager.instance.dogAnimator.SetInteger(m_cStateID, m_cStateWait);

						m_timer0.Start();
						m_timer1.Start();
					}
					break;
				}
			case State.End:
				{
					if (m_subState == 0)
					{
						m_textBox.localScale = Vector3.Lerp(Vector3.one, Vector3.zero,
							Mathf.Clamp01(m_timer0.elapasedTime / m_scalingTextBoxSeconds));

						if (m_timer0.elapasedTime >= m_scalingTextBoxSeconds)
						{
							if (TutorialUIManager.instance.onWaitTutorials == 0)
							{
								m_subState = 1;
								m_timer0.Start();
							}
							else
								m_subState = 3;
						}
					}
					else if (m_subState == 1)
					{
						if (m_timer0.elapasedTime < m_removeJumpSeconds)
						{
							m_dogTransform.position = m_dogTransform.position.ToYManual(m_firstPosition.y + 
								Mathf.Sin(Mathf.Lerp(0, Mathf.PI, m_timer0.elapasedTime / m_removeJumpSeconds)) * m_removeJumpHeight);
						}
						else
						{
							m_dogTransform.position = m_firstPosition;
							m_timer0.Start();
							m_subState = 2;
						}
					}
					else if (m_subState == 2)
					{
						Vector3 setPosition = Vector3.Lerp(m_firstPosition, m_startPoint, m_timer0.elapasedTime / m_removeSeconds);

						setPosition.y = m_firstPosition.y + Mathf.Sin(Mathf.Lerp(0.0f, Mathf.PI * 0.5f,
							Mathf.Clamp01(m_timer0.elapasedTime / m_removeSeconds))) * (m_startPoint.y - m_firstPosition.y);

						m_dogTransform.position = setPosition;

						if (m_timer0.elapasedTime >= m_removeSeconds)
							m_subState = 3;
					}
					else if (m_subState == 3)
						return false;

					break;
				}
		}

		return true;
	}

	void Start()
	{
		m_text.enabled = false;

		m_firstPosition = m_dogTransform.position;
		m_aButtonFirstPosition = m_aButton.position;

		m_startPoint += m_firstPosition;

		m_dogTransform.position = m_startPoint;
		m_textBox.localScale = Vector3.zero;
	}
}
