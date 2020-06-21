using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventDetection : MonoBehaviour
{
	public BaseEvent nearbyManualEvent { get; private set; } = null;

	[SerializeField]
	PlayerManagerIntermediary m_managerIntermediary = null;
	[SerializeField]
	FukidashiController m_fukidashiController = null;
	[SerializeField]
	string m_inputAxisAsTrigger = "";
	[SerializeField]
	float m_detectionIntervalSeconds = 0.1f;

	[Space, SerializeField]
	BoxCastInfos m_autoTriggerDetection = new BoxCastInfos();
	[SerializeField]
	bool m_isEnabledAutoTriggerDetection = false;
	[SerializeField]
	BoxCastInfos m_manualTriggerDetection = new BoxCastInfos();
	[SerializeField]
	bool m_isEnabledManualTriggerDetection = false;

    //debug only
#if UNITY_EDITOR
	/// <summary>gizmo color -> auto</summary>
	static readonly Color m_cdAutoColor = new Color(0.95f, 0.95f, 0.1f, 0.8f);
	/// <summary>gizmo color -> manual</summary>
	static readonly Color m_cdManualColor = new Color(0.1f, 0.95f, 0.95f, 0.8f);

	/// <summary>Draw Gizmos? (debug only)</summary>
	[SerializeField, Tooltip("Draw Gizmos? (debug only)"), Header("Debug Only")]
	bool m_dIsDrawGizmos = false;
#endif

	List<BaseEvent> m_hitEvents = new List<BaseEvent>();
	List<BaseEvent> m_oldHitAutoEvents = new List<BaseEvent>();
	List<BaseEvent> m_oldHitManualEvents = new List<BaseEvent>();
	Timer m_intervalTimer = new Timer();

	// Start is called before the first frame update
	void Start()
    {
		m_intervalTimer.Start();  
    }

    // Update is called once per frame
    void Update()
    {
		if (m_managerIntermediary.thisInfo.isLinkEvent)
			return;

		if (m_intervalTimer.elapasedTime < m_detectionIntervalSeconds)
		{
			if (PlayerAndTerritoryManager.instance.mainPlayer.input.isEnableInputAndActionInput
				&& Input.GetButtonDown(m_inputAxisAsTrigger) && nearbyManualEvent != null)
			{
				nearbyManualEvent.TriggerEvent(gameObject);
				m_fukidashiController.DisableEffect();
			}
			else if (nearbyManualEvent != null)
				nearbyManualEvent.CallNearbyIfManualTrigger();

			return;
		}

		Transform thisTransform = transform;
		nearbyManualEvent = null;

		if (m_isEnabledAutoTriggerDetection)
		{
			BaseEvent baseEvent = Detection(thisTransform, ref m_autoTriggerDetection, true);
			if (baseEvent != null)
				baseEvent.TriggerEvent(gameObject);
		}
		if (m_isEnabledManualTriggerDetection & !m_managerIntermediary.thisInfo.isLinkEvent)
		{
			nearbyManualEvent = Detection(thisTransform, ref m_manualTriggerDetection, false);
			if (Input.GetButtonDown(m_inputAxisAsTrigger) && nearbyManualEvent != null)
			{
				nearbyManualEvent.TriggerEvent(gameObject);
				m_fukidashiController.DisableEffect();
			}
			else if (nearbyManualEvent != null)
			{
				m_fukidashiController.EnableEffect();
				nearbyManualEvent.CallNearbyIfManualTrigger();
			}
			else if (nearbyManualEvent == null)
				m_fukidashiController.DisableEffect();
		}

		m_intervalTimer.Start();
	}

	BaseEvent Detection(Transform thisTransform, ref BoxCastInfos boxCastInfos, bool isAutoTrigger)
	{
		BaseEvent result = null;
		Vector3 overlapPosition = boxCastInfos.WorldCenter(thisTransform);

		var collisions = Physics.OverlapBox(overlapPosition, 
			boxCastInfos.overlapSize, thisTransform.rotation, boxCastInfos.layerMask);

		m_hitEvents.Clear();
		for (int i = 0, length = collisions.Length; i < length; ++i)
		{
			var baseEvent = collisions[i].gameObject.GetComponentInParent<BaseEvent>();

			if (baseEvent != null && !m_hitEvents.Contains(baseEvent) && baseEvent.linkPlayerInfo == null
				&& (!isAutoTrigger || (baseEvent.isAutoTrigger & isAutoTrigger)))
				m_hitEvents.Add(baseEvent);
		}

		if (m_hitEvents.Count > 0)
		{
			float minDistance = 10000.0f;
			for (int i = 0, count = m_hitEvents.Count; i < count; ++i)
			{
				float sqrMagnitude = (m_hitEvents[i].transform.position - overlapPosition).sqrMagnitude;
				if (minDistance > sqrMagnitude && !(isAutoTrigger && m_oldHitAutoEvents.Contains(m_hitEvents[i])))
				{
					var offMeshLinkEvent = m_hitEvents[i] as UniqueOffMeshEvent;

					if (offMeshLinkEvent != null && !offMeshLinkEvent.IsHitTerritoryEndPoint(
						m_managerIntermediary.thisInfo.territorialArea, transform.position))
						continue;

					result = m_hitEvents[i];
					minDistance = sqrMagnitude;
				}
			}
		}

		if (isAutoTrigger)
			m_oldHitAutoEvents = new List<BaseEvent>(m_hitEvents);
		else
			m_oldHitManualEvents = new List<BaseEvent>(m_hitEvents);
		return result;
	}


	//debug only
#if UNITY_EDITOR
	/// <summary>[OnDrawGizmos]</summary>
	void OnDrawGizmos()
	{
		//!Flgな場合終了
		if (!m_dIsDrawGizmos) return;

		//呼び出しコスト削減
		Transform thisTransform = transform;

		if (m_isEnabledAutoTriggerDetection)
			m_autoTriggerDetection.DCubeOnDrawGizmos(thisTransform, m_cdAutoColor);

		if (m_isEnabledManualTriggerDetection)
			m_autoTriggerDetection.DCubeOnDrawGizmos(thisTransform, m_cdManualColor);
	}
#endif
}
