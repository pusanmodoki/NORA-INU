using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepFlags : MonoBehaviour
{
	public enum AutoExecutionFrame
	{
		Update,
		FixedUpdate,
		ManualOnly
	}
	public struct HitFlags
	{
		public HitFlags(bool isSet)
		{
			isStay = isEnter = isExit = isSet;
		}

		public bool isStay;
		public bool isEnter;
		public bool isExit;

		public void EditFlag(bool isNewValue)
		{
			isEnter = (isNewValue ^ isStay) & isNewValue;
			isExit = (isNewValue ^ isStay) & isStay;
			isStay = isNewValue;
		}
	}
	static readonly Quaternion m_cXAxisRotation = Quaternion.AngleAxis(90, Vector3.up);

	public AutoExecutionFrame autoExecutionFrame { get { return m_autoExecutionFrame; } }

	public RaycastHit raycastHitRight { get { return m_raycastHitRight; } } 
	public bool isRightStay { get { return m_isHitFlagsRight.isStay; } }
	public bool isRightEnter { get { return m_isHitFlagsRight.isEnter; } }
	public bool isRightExit { get { return m_isHitFlagsRight.isExit; } }

	public RaycastHit raycastHitLeft { get { return m_raycastHitLeft; } }
	public bool isLeftStay { get { return m_isHitFlagsLeft.isStay; } }
	public bool isLeftEnter { get { return m_isHitFlagsLeft.isStay; } }
	public bool isLeftExit { get { return m_isHitFlagsLeft.isStay; } }

	public RaycastHit raycastHitForward { get { return m_raycastHitForward; } }
	public bool isForwardStay { get { return m_isHitFlagsForward.isStay; } }
	public bool isForwardEnter { get { return m_isHitFlagsForward.isStay; } }
	public bool isForwardExit { get { return m_isHitFlagsForward.isStay; } }

	public RaycastHit raycastHitBack { get { return m_raycastHitBack; } }
	public bool isBackStay { get { return m_isHitFlagsBack.isStay; } }
	public bool isBackEnter { get { return m_isHitFlagsBack.isStay; } }
	public bool isBackExit { get { return m_isHitFlagsBack.isStay; } }

	[SerializeField]
	AutoExecutionFrame m_autoExecutionFrame = AutoExecutionFrame.FixedUpdate;
	[Space, SerializeField]
	Vector2 m_layooutScaler = new Vector2(1.5f, 0.0f);
	[Space, SerializeField]
	LayerMaskEx m_boxCastLayerMask = 0;
	[SerializeField]
	Vector3 m_boxCastSize = Vector3.one;
	[SerializeField]
	float m_boxCastDistance = 1.0f;

	RaycastHit m_raycastHitRight = new RaycastHit();
	RaycastHit m_raycastHitLeft = new RaycastHit();
	RaycastHit m_raycastHitForward = new RaycastHit();
	RaycastHit m_raycastHitBack = new RaycastHit();

	HitFlags m_isHitFlagsRight = new HitFlags(false);
	HitFlags m_isHitFlagsLeft = new HitFlags(false);
	HitFlags m_isHitFlagsForward = new HitFlags(false);
	HitFlags m_isHitFlagsBack = new HitFlags(false);

	//debug only
#if UNITY_EDITOR
	/// <summary>gizmo color -> hit frame</summary>
	static readonly Color m_cdHitColor = new Color(1.0f, 0.15f, 0.1f, 0.8f);
	/// <summary>gizmo color -> not hit frame</summary>
	static readonly Color m_cdNotHitColor = new Color(0.95f, 0.95f, 0.95f, 0.8f);

	[SerializeField]
	bool m_dIsDrawGizmos = true;
#endif
	
    // Update is called once per frame
    void Update()
    {
		if (m_autoExecutionFrame == AutoExecutionFrame.Update)
			StepDetection();
	}
	void FixedUpdate()
	{
		if (m_autoExecutionFrame == AutoExecutionFrame.FixedUpdate)
			StepDetection();
	}

	public void ManualStepDetection()
	{
		StepDetection();
	}

	void StepDetection()
	{
		//呼び出しコスト削減
		Transform myTransform = transform;
		Quaternion rotation = myTransform.rotation, usedRotation;
		Vector3 position = myTransform.position, right = Vector3.right, forward = Vector3.forward,
			startPoint = Vector3.zero, size = m_boxCastSize * 0.5f, downDirection = Vector3.down * m_boxCastDistance;
		//リザルトフラグ
		bool isResult = false;

		position.y += m_layooutScaler.y;

		//Right
		{
			startPoint = position + right * m_layooutScaler.x;
			usedRotation = rotation * m_cXAxisRotation;

			isResult = Physics.BoxCast(startPoint, size, Vector3.down,
				out m_raycastHitRight, usedRotation, m_boxCastDistance, m_boxCastLayerMask);
			if (!isResult)
				isResult = Physics.CheckBox(startPoint + downDirection, size, usedRotation, m_boxCastLayerMask);

			m_isHitFlagsRight.EditFlag(isResult);
		}
		//Left
		{
			startPoint = position + -right * m_layooutScaler.x;
			usedRotation = rotation * m_cXAxisRotation;

			isResult = Physics.BoxCast(startPoint, size, Vector3.down,
				out m_raycastHitLeft, usedRotation, m_boxCastDistance, m_boxCastLayerMask);
			if (!isResult)
				isResult = Physics.CheckBox(startPoint + downDirection, size, usedRotation, m_boxCastLayerMask);

			m_isHitFlagsLeft.EditFlag(isResult);
		}
		//Forward
		{
			startPoint = position + forward * m_layooutScaler.x;

			isResult = Physics.BoxCast(startPoint, size, Vector3.down,
				out m_raycastHitForward, rotation, m_boxCastDistance, m_boxCastLayerMask);
			if (!isResult)
				isResult = Physics.CheckBox(startPoint + downDirection, size, rotation, m_boxCastLayerMask);

			m_isHitFlagsForward.EditFlag(isResult);
		}
		//Back
		{
			startPoint = position + -forward * m_layooutScaler.x;

			isResult = Physics.BoxCast(startPoint, size, Vector3.down,
				out m_raycastHitBack, rotation, m_boxCastDistance, m_boxCastLayerMask);
			if (!isResult)
				isResult = Physics.CheckBox(startPoint + downDirection, size, rotation, m_boxCastLayerMask);

			m_isHitFlagsBack.EditFlag(isResult);
		}
	}

	//debug only
#if UNITY_EDITOR
	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		//!Flgな場合終了
		if (!m_dIsDrawGizmos) return;

		//描画に反映されないため実行中以外はこちらで更新を行う
		if (!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused)
			ManualStepDetection();

		//呼び出しコスト削減
		Transform myTransform = transform;
		Quaternion rotation = myTransform.rotation;
		Vector3 position = myTransform.position;

		position.y += m_layooutScaler.y;

		DDrawCast(m_cXAxisRotation, position + Vector3.right * m_layooutScaler.x, m_raycastHitRight.distance, isRightStay);
		DDrawCast(m_cXAxisRotation, position + -Vector3.right * m_layooutScaler.x, m_raycastHitLeft.distance, isLeftStay);
		DDrawCast(Quaternion.identity, position + Vector3.forward * m_layooutScaler.x, m_raycastHitForward.distance, isForwardStay);
		DDrawCast(Quaternion.identity, position + -Vector3.forward * m_layooutScaler.x, m_raycastHitBack.distance, isBackStay);
	}

	void DDrawCast(Quaternion rotation, Vector3 startPoint, float ifHitDistance, bool isHit)
	{
		float distance = isHit && ifHitDistance > 0.0f ? ifHitDistance : m_boxCastDistance;

		//Init
		Gizmos.color = isHit ? m_cdHitColor : m_cdNotHitColor;
		Gizmos.matrix = Matrix4x4.identity;

		//Draw Ray
		Gizmos.DrawRay(startPoint, Vector3.down * distance);

		//Matrix
		Gizmos.matrix = Matrix4x4.Translate(startPoint + Vector3.down * distance);
		Gizmos.matrix *= Matrix4x4.Rotate(rotation);
		//Draw Cube
		Gizmos.DrawWireCube(Vector3.zero, m_boxCastSize);
	}
#endif
}
