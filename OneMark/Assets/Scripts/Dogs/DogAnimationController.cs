using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationController : MonoBehaviour
{
	[SerializeField]
	Animator m_animator = null;

    // Start is called before the first frame update
    void Start()
    {
		m_animator.SetInteger("State", 1);  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
