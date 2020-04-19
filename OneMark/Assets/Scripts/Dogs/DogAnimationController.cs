using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
	public class EditAnimation
	{
		public EditAnimation(Animator animator)
		{
			m_animator = animator;

			if (m_stateIntegerID == -1)
			{
				m_stateIntegerID = Animator.StringToHash("State");
				m_rollingTriggerID = Animator.StringToHash("Rolling");
				m_markingTriggerID = Animator.StringToHash("Marking");
				m_sleepStartTriggerID = Animator.StringToHash("SleepStart");
				m_wakeUpTriggerID = Animator.StringToHash("WakeUp");
				m_returnRunTriggerID = Animator.StringToHash("ReturnRun");
				m_nextChangeTriggerID = Animator.StringToHash("NextChange");
				m_isSearchBoolID = Animator.StringToHash("IsSearch");
			}
		}

		public void SetStateStand()
		{
			m_animator.SetInteger(m_stateIntegerID, 0);
		}
		public void SetStateRun()
		{
			m_animator.SetInteger(m_stateIntegerID, 1);
		}
		public void SetBoolIsNextSearch(bool isSet)
		{
			m_animator.SetBool(m_isSearchBoolID, isSet);
		}

		public void SetTriggerRolling()
		{
			m_animator.SetTrigger(m_rollingTriggerID);
		}
		public void SetTriggerMarking()
		{
			m_animator.SetTrigger(m_markingTriggerID);
		}
		public void SetTriggerReturnRun()
		{
			m_animator.SetTrigger(m_returnRunTriggerID);
		}

		public void SetTriggerSleepStart()
		{
			m_animator.SetTrigger(m_sleepStartTriggerID);
		}
		public void SetTriggerWakeUp()
		{
			m_animator.SetTrigger(m_wakeUpTriggerID);
		}
		public void SetTriggerNextChange()
		{
			m_animator.SetTrigger(m_nextChangeTriggerID);
		}

		Animator m_animator = null;

		static int m_stateIntegerID = -1;
		static int m_rollingTriggerID = -1;
		static int m_markingTriggerID = -1;
		static int m_sleepStartTriggerID = -1;
		static int m_wakeUpTriggerID = -1;
		static int m_returnRunTriggerID = -1;
		static int m_nextChangeTriggerID = -1;
		static int m_isSearchBoolID = -1;
	}

	public EditAnimation editAnimation { get; private set; } = null;

	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	DogAIAgent m_aiAgent = null;

	// Start is called before the first frame update
	void Awake()
    {
		editAnimation = new EditAnimation(m_animator);
    }

	public void AnimationWakeUpCallback()
	{
		editAnimation.SetTriggerNextChange();
		//ステイ終了
		m_aiAgent.SetSitAndStay(false, m_aiAgent.linkMarkPoint);
	}
}
