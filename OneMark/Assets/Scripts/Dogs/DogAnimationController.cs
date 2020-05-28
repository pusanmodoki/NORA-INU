using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
	public enum AnimationState : int
	{
		Stand = 0,
		Run = 1,
		Rolling = 2
	}

	public class EditAnimation
	{
		public EditAnimation(Animator animator) { m_animator = animator; }

		public AnimationState state
		{
			get { return (AnimationState)m_animator.GetInteger(m_stateIntegerID); }
			set { m_animator.SetInteger(m_stateIntegerID, (int)value); }
		}
		public bool isWakeUpNextSearch
		{
			get { return m_animator.GetBool(m_isSearchBoolID); }
			set { m_animator.SetBool(m_isSearchBoolID, value); }
		}

		public void TriggerMarking() { m_animator.SetTrigger(m_markingTriggerID); }
		public void TriggerMarkingEnd() { m_animator.SetTrigger(m_markingEndTriggerID); }
		public void TriggerWaitRunStart() { m_animator.SetTrigger(m_waitRunStartTriggerID); }
		public void TriggerForceChangeStand() { m_animator.SetTrigger(m_forceChangeStandID); }
		public void TriggerWakeUp() { m_animator.SetTrigger(m_wakeUpTriggerID); }
		public void TriggerWakeUpNext() { m_animator.SetTrigger(m_wakeUpNextTriggerID); }

		static int m_stateIntegerID = Animator.StringToHash("State");
		static int m_isSearchBoolID = Animator.StringToHash("IsSearch");
		static int m_markingTriggerID = Animator.StringToHash("Marking");
		static int m_markingEndTriggerID = Animator.StringToHash("MarkingEnd");
		static int m_waitRunStartTriggerID = Animator.StringToHash("WaitRunStart");
		static int m_forceChangeStandID = Animator.StringToHash("ForceChangeStand");
		static int m_wakeUpTriggerID = Animator.StringToHash("WakeUp");
		static int m_wakeUpNextTriggerID = Animator.StringToHash("WakeUpNext");

		Animator m_animator = null;
	}

	public EditAnimation editAnimation { get; private set; } = null;
	public DogAIAgent aiAgent { get { return m_aiAgent; } }

	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	DogAIAgent m_aiAgent = null;
	[SerializeField]
	DogRushingAndMarking m_rushingAndMarking = null;

	// Start is called before the first frame update
	void Awake()
    {
		editAnimation = new EditAnimation(m_animator);
    }

	public void AnimationMarkingEndCallback()
	{
		if (m_rushingAndMarking != null)
			m_rushingAndMarking.MoveStateFromMarkingEndToFunctionEnd();
	}
	public void AnimationWakeUpCallback()
	{
		editAnimation.TriggerWakeUpNext();

		//ステイ終了
		if (!editAnimation.isWakeUpNextSearch)
		{
			m_aiAgent.SetWaitAndRun(false, m_aiAgent.linkMarkPoint);
			m_aiAgent.navMeshAgent.isStopped = false;
		}
	}
}
