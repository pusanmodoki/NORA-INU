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
		public EditAnimation(DogAnimationController thisController)
		{
			m_thisController = thisController;
			m_animator = thisController.animator;
		}

		public AnimationState state
		{
			get { return (AnimationState)m_animator.GetInteger(m_stateIntegerID); }
			set { m_animator.SetInteger(m_stateIntegerID, (int)value); }
		}
		public bool isWakeUp
		{
			get { return m_animator.GetBool(m_isWakeUpBoolID); }
			set { m_animator.SetBool(m_isWakeUpBoolID, value); }
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
		public void TriggerWakeUpNext() { m_animator.SetTrigger(m_wakeUpNextTriggerID); }
		public void TriggerReturnWakeUp()
		{
			m_animator.SetTrigger(m_returnWakeUpTriggerID);
			m_thisController.SetTrueReturnWakeUp();
		}
	
		static int m_stateIntegerID = Animator.StringToHash("State");
		static int m_isWakeUpBoolID = Animator.StringToHash("IsWakeUp");
		static int m_isSearchBoolID = Animator.StringToHash("IsSearch");
		static int m_markingTriggerID = Animator.StringToHash("Marking");
		static int m_markingEndTriggerID = Animator.StringToHash("MarkingEnd");
		static int m_waitRunStartTriggerID = Animator.StringToHash("WaitRunStart");
		static int m_forceChangeStandID = Animator.StringToHash("ForceChangeStand");
		static int m_returnWakeUpTriggerID = Animator.StringToHash("ReturnWakeUp");
		static int m_wakeUpNextTriggerID = Animator.StringToHash("WakeUpNext");

		DogAnimationController m_thisController = null;
		Animator m_animator = null;
	}

	public EditAnimation editAnimation { get; private set; } = null;
	public Animator animator { get { return m_animator; } }
	public DogAIAgent aiAgent { get { return m_aiAgent; } }
	public void SetTrueReturnWakeUp() { m_isReturnWakeUp = true; }

	[SerializeField]
	Animator m_animator = null;
	[SerializeField]
	DogAIAgent m_aiAgent = null;
	[SerializeField]
	DogRushingAndMarking m_rushingAndMarking = null;

	bool m_isReturnWakeUp = false;

	// Start is called before the first frame update
	void Awake()
    {
		editAnimation = new EditAnimation(this);
    }

	public void AnimationMarkingEndCallback()
	{
		if (m_rushingAndMarking != null)
			m_rushingAndMarking.MoveStateFromMarkingEndToFunctionEnd();
	}
	public void AnimationWakeUpCallback()
	{
		if (m_isReturnWakeUp)
		{
			m_isReturnWakeUp = false;
			editAnimation.TriggerWakeUpNext();

			//ステイ終了
			if (!editAnimation.isWakeUpNextSearch)
			{
				m_aiAgent.SetWaitAndRun(false, m_aiAgent.linkMarkPoint);
				m_aiAgent.navMeshAgent.isStopped = false;
			}
		}
	}
}
