using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField]
    private GameObject target = null;

    [SerializeField]
    private float rotateSpeed = 1.0f;

    [SerializeField]
    ParticleSystem m_lineEffect = null;


    private List<MeshRenderer> m_renderers = new List<MeshRenderer>();
	Quaternion rotation = Quaternion.identity;

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
        if (target)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            emission.rateOverTime = distance;
            main.startSpeed = distance;
            m_lineEffect.transform.LookAt(target.transform);
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

    public void SetTarget(GameObject _target)
    {
        //ParticleSystem.EmissionModule emission = m_lineEffect.emission;
        //ParticleSystem.MainModule main = m_lineEffect.main;

        target = _target;
        if(target == null)
        {
            m_lineEffect.gameObject.SetActive(false);
        }
        else
        {
            if (!m_lineEffect.gameObject.activeSelf)
            {
                m_lineEffect.gameObject.SetActive(true);
            }
        }

    }

}
