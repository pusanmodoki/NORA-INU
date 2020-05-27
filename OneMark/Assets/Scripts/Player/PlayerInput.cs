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

	public Vector3 moveInput { get { return m_moveInput; } }
	public bool isEnableInput { get; set; } = true;

	/// <summary>This PlayerMaualCollisionAdministrator</summary>
	[SerializeField, Tooltip("This PlayerMaualCollisionAdministrator")]
	PlayerMaualCollisionAdministrator m_maualCollisionAdministrator = null;
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

	Timer m_shotTimers = new Timer();

	public void GameClearAnimation()
	{
		m_animator.SetTrigger("Result");
		m_state = AnimationState.GameClear;
		m_animator.SetInteger("State", (int)m_state);
		isEnableInput = false;
	}
	public void GameOverAnimation()
	{
		m_animator.SetTrigger("Result");
		m_state = AnimationState.GameOver;
		m_animator.SetInteger("State", (int)m_state);
		isEnableInput = false;
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

		if (!isEnableInput)
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

		m_animator.SetInteger("State", (int)m_state);
    }

    void ShotInput()
    {
		if (!isEnableInput)
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
			ServantManager.instance.allServants[linkServantID].ComeBecauseEndOfMarking(
				m_maualCollisionAdministrator.IsHitInstructionsReturnDog(ServantManager.instance.allServants[linkServantID]));
			m_shotTimers.Start();

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

			bool isResult = m_maualCollisionAdministrator.IsHitInstructionsGoingDog(
				ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()][followIndex]);

			if (isResult)
				isResult &= m_maualCollisionAdministrator.IsHitInstructionsMarkPoint(
					m_maualCollisionAdministrator.hitVisibilityMarkPoint);


			if (isResult && ServantManager.instance.servantByPlayers[gameObject.GetInstanceID()][followIndex]
				.GoSoStartOfMarking(m_maualCollisionAdministrator.hitVisibilityMarkPoint))
			{
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
