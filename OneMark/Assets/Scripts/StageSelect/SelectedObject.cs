using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObject : MonoBehaviour
{
    [SerializeField]
    protected bool m_isSelected = false;
    public bool isSelected { get { return m_isSelected; } set { m_isSelected = value; } }

    public virtual void OnEnter() { }
    
    public virtual void OnCirsol() { }

    public virtual void OffCirsol() { }
}
