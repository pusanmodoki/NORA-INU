using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private enum AnimationState
    {
        Stand = 0,
        Run,
        GameClear,
        GameOver
    }
	static readonly int m_cStateID = Animator.StringToHash("State");
	static readonly int m_cCommandID = Animator.StringToHash("Command");
	static readonly int m_cCallID = Animator.StringToHash("Call");
	static readonly int m_cResultID = Animator.StringToHash("Result");

	public Vector3 moveInput { get { return m_moveInput; } }
	public bool isEnableInput { get; set; } = true;
	public bool isEnableActionInput { get; set; } = true;
	public bool isEnableInputAndActionInput { get { return isEnableInput & isEnableActionInput; } }

	/// <summary>This PlayerMaualCollisionAdministrator</summary>
	[SerializeField, Tooltip("This PlayerMaualCollisionAdministrator")]
	PlayerMaualCollisionAdministrator m_maualCollisionAdministrator = null;
	[SerializeField]
	SEPlayer m_sePlayer = null;
	/// <summary>
	/// アニメーションコントローラ
	/// </summary>
	[SerializeField]
    private Animator m_animator = null;
    /// <summary>
    /// プレイヤーの状態
    /// </summary>
    [SerializeField]
    private AnimationState m_state = AnimationState.Stand;

	[SerializeField]
	float m_shotDeactivateSeconds = 0.5f;

#if UNITY_EDITOR
	[Header("Debug Only"), SerializeField]
	Vector2 m_dMoveInput = Vector2.zero;
#endif

	Vector3 m_moveInput = Vector3.zero;
	List<int> m_disableEvents = new List<int>();
	List<int> m_disableActionEvents = new List<int>();
	Timer m_shotTimers = new Timer();
	int m_disableCounter = 0;
	int m_disableActionCounter = 0;

	public void GameClearAnimation()
	{
		m_animator.SetTrigger(m_cResultID);
		m_state = AnimationState.GameClear;
		m_animator.SetInteger(m_cStateID, (int)m_state);
	}
	public void GameOverAnimation()
	{
		m_animator.SetTrigger(m_cResultID);
		m_state = AnimationState.GameOver;
		m_animator.SetInteger(m_cStateID, (int)m_state);
	}

	public void StartDisableInput(out int disableID)
	{
		m_disableEvents.Add(m_disableCounter);
		disableID = m_disableCounter++;
	}
	public void EndDisableInput(int disableID)
	{
		if (m_disableEvents.Contains(disableID))
			m_disableEvents.Remove(disableID);
	}

	public void StartDisableActionInput(out int disableID)
	{
		m_disableActionEvents.Add(m_disableActionCounter);
		disableID = m_disableActionCounter++;
	}
	public void EndDisableActionInput(int disableID)
	{
		if (m_disableActionEvents.Contains(disableID))
			m_disableActionEvents.Remove(disableID);
	}

	void Start()
    {
		m_shotTimers = new Timer();
	}

    // Update is called once per frame
    void Update()
	{
		MoveInput();

		ShotInput();
	}

    /// <summary>
    ///  移動入力
    /// </summary>
    void MoveInput()
    {
        m_moveInput = Vector3.zero;

		if (!isEnableInput || m_disableEvents.Count > 0
			|| MainGameManager.instance.isPauseStay)
			return;

		m_moveInput.x = Input.GetAxis("Horizontal");
		m_moveInput.z = Input.GetAxis("Vertical");

		m_moveInput = m_moveInput.normalized;

#if UNITY_EDITOR
		m_dMoveInput = m_moveInput;
#endif

		if (m_moveInput.sqrMagnitude > 0.0f)
            m_state = AnimationState.Run;
        else
            m_state = AnimationState.Stand;

		m_animator.SetInteger(m_cStateID, (int)m_state);
    }

    void ShotInput()
    {
		if (!isEnableInputAndActionInput || m_disableEvents.Count > 0 
			|| m_disableActionEvents.Count > 0 || MainGameManager.instance.isPauseStay)
			return;

		//if (Input.GetButtonDown("Fire1")
		//	&& m_shotTimers[0].elapasedTime >= m_shotDeactivateSeconds)
		//{
		//	ShotServant(0);
		//}

		//if (Input.GetButtonDown("Fire2")
		//	&& m_shotTimers[1].elapasedTime >= m_shotDeactivateSeconds)
		//{
		//	ShotServant(1);
		//}

		if (Input.GetButtonDown("Fire3") && m_shotTimers.elapasedTime >= m_shotDeactivateSeconds)
		{
			ShotServant();
		}
	}

	void ShotServant()
	{
		if (m_maualCollisionAdministrator.hitVisibilityMarkPoint == null) return;

		int linkServantID = m_maualCollisionAdministrator.hitVisibilityMarkPoint.linkServantID;

		//Unlink
		if (linkServantID != -1)
		{
			if (ServantManager.instance.allServants[linkServantID].ComeBecauseEndOfMarking(
				m_maualCollisionAdministrator.IsHitInstructionsReturnDog(ServantManager.instance.allServants[linkServantID])))
			{
				m_animator.SetTrigger(m_cCallID);
				m_sePlayer.PlaySE(0);
				m_shotTimers.Start();
			}
		}
		//Go!
		else
		{
			int followIndex = 0;
			for (int i = 0, count = 3; i < count; ++i)
			{
				var servant = ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()][i];

				if (!servant.isAccompanyingPlayer)
					++followIndex;
				else
					break;
			}

			if (followIndex == 3) return;

			if (m_maualCollisionAdministrator.IsHitInstructionsGoingDog(
					ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()][followIndex]) &&
				ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()][followIndex]
					.GoSoStartOfMarking(m_maualCollisionAdministrator.hitVisibilityMarkPoint))
			{
				m_animator.SetTrigger(m_cCommandID);
				m_sePlayer.PlaySE(0);
				m_shotTimers.Start();
			}
		}

		//var servant = ServantManager.instance.servantByMainPlayer[id];

		//if (m_isShotFlags[id] && servant.ComeBecauseEndOfMarking(
		//		m_maualCollisionAdministrator.IsHitInstructionsReturnDog(servant)))
		//{
		//	m_isShotFlags[id] = false;
		//	m_shotTimers[id].Start();
		//}
		//else if(!m_isShotFlags[id] && m_maualCollisionAdministrator.isVisibilityStay
		//	&& m_maualCollisionAdministrator.hitVisibilityMarkPoint != null)
		//{
		//	bool isResult = m_maualCollisionAdministrator.IsHitInstructionsGoingDog(servant);

		//	if (isResult)
		//		isResult &= m_maualCollisionAdministrator.IsHitInstructionsMarkPoint(
		//			m_maualCollisionAdministrator.hitVisibilityMarkPoint);

		//	if (isResult && servant.GoSoStartOfMarking(m_maualCollisionAdministrator.hitVisibilityMarkPoint))
		//	{
		//		m_isShotFlags[id] = true;
		//		m_shotTimers[id].Start();
		//	}
		//}
	}
}
