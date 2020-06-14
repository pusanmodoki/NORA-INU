using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
	public bool isEnableMarker { get; private set; } = false;

    [SerializeField]
    ParticleSystem m_lineEffect = null;
	[SerializeField]
	Canvas m_crossCanvas = null;

    List<MeshRenderer> m_renderers = new List<MeshRenderer>();
	Vector3 m_targetPosition = Vector3.zero;
	float m_crossDistance = 0.0f;
	bool m_isCrossToTarget = false;


	public void SetReallyVisibilityDistance(float set)
	{
		m_crossDistance = set;
	}

	// Start is called before the first frame update
	void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_renderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }

		m_crossCanvas.worldCamera = Camera.main;
		m_crossCanvas.enabled = false;
	}

	// Update is called once per frame
	void LateUpdate()
    {
        ParticleSystem.EmissionModule emission = m_lineEffect.emission;
        ParticleSystem.MainModule main = m_lineEffect.main;

		if (isEnableMarker)
		{
			float distance = Vector3.Distance(transform.position, m_targetPosition);
			// emission.rateOverTime = Mathf.Clamp(distance / 3, 1.0f, 100.0f);
			main.startSpeed = distance;

			m_lineEffect.transform.LookAt(m_targetPosition);
			main.startRotation = m_lineEffect.transform.eulerAngles.y * Mathf.Deg2Rad;

			if (m_crossCanvas.enabled & m_isCrossToTarget)
			{
				m_crossCanvas.transform.position = transform.position.ToYZero() +
					((m_targetPosition - transform.position).normalized * m_crossDistance).ToYManual(m_targetPosition.y);
			}
			else if (m_crossCanvas.enabled)
			{
				m_crossCanvas.transform.position = m_targetPosition;
			}
		}

        //TurningEffect();
    }

    //   private void TurningEffect()
    //   {
    //	if (target != null)
    //		transform.position = target.transform.position;

    //	rotation = Quaternion.Slerp(rotation, 
    //		rotation * Quaternion.AngleAxis(rotateSpeed, Vector3.up), Time.deltaTime);
    //	transform.rotation = rotation;

    //	//transform.Rotate(0.0f, rotateSpeed * Time.deltaTime, 0.0f);
    //}

	public void ClearMarker()
	{
		m_lineEffect.Clear();
	}

	public void EnableCross(bool isCrossToTarget)
	{
		if (!m_crossCanvas.enabled) m_crossCanvas.enabled = true;
		m_isCrossToTarget = isCrossToTarget;
	}
	public void DisableCross()
	{
		if (m_crossCanvas.enabled) m_crossCanvas.enabled = false;
	}

	public void EnableMarker(Vector3 target)
	{
		if (!isEnableMarker) m_lineEffect.Play();

		m_targetPosition = target;
		isEnableMarker = true;
	}
	public void DisableMarker()
	{
		if (isEnableMarker) m_lineEffect.Stop();

		m_lineEffect.Clear();

		isEnableMarker = false;
	}

}
