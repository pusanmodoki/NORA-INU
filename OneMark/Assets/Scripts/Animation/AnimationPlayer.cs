using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("操作したいアニメーションコントローラ")]
    private Animator m_animator = null;


    public void SetTrigger(string _parameterName)
    {
        m_animator.SetTrigger(_parameterName);
    }

    public void SetState(int _integer, string _parameterName)
    {
        m_animator.SetInteger(_parameterName, _integer);
    }

    public void SetFloat(float _float, string _parameterName)
    {
        m_animator.SetFloat(_parameterName, _float);
    }

    public void SetBool(bool _bool, string _parameterName)
    {
        m_animator.SetBool(_parameterName, _bool);
    }
}
