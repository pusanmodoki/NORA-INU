using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField]
    ParticleSystem m_lineEffect = null;

    private List<MeshRenderer> m_renderers = new List<MeshRenderer>();
	Quaternion rotation = Quaternion.identity;

	Vector3 m_targetPosition = Vector3.zero;
	bool m_isEnabled = false;

	// Start is called before the first frame update
	void Start()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            m_renderers.Add(transform.GetChild(i).GetComponent<MeshRenderer>());
        }

		rotation = transform.rotation;
	}

	// Update is called once per frame
	void LateUpdate()
    {
        ParticleSystem.EmissionModule emission = m_lineEffect.emission;
        ParticleSystem.MainModule main = m_lineEffect.main;

        if (m_isEnabled)
        {
            float distance = Vector3.Distance(transform.position, m_targetPosition);
            emission.rateOverTime = distance;
            main.startSpeed = distance;
            m_lineEffect.transform.LookAt(m_targetPosition);
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

	public void EnableMarker(Vector3 target)
	{
		if (!m_isEnabled) m_lineEffect.Play();

		m_targetPosition = target;
		m_isEnabled = true;
	}
	public void DisableMarker()
	{
		if (m_isEnabled) m_lineEffect.Stop();

		m_lineEffect.Clear();

		m_isEnabled = false;
	}

}
