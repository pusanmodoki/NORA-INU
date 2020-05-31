using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSelectedObject : MonoBehaviour
{
    [SerializeField]
    protected bool m_isSelected = false;
    public bool isSelected { get { return m_isSelected; } set { m_isSelected = value; } }

    public MenuInput menu { get; private set; }

    public void SetMenu(MenuInput _menu)
    {
        menu = _menu;
    }

    public virtual void OnEnter() { }
    
    public virtual void OnCursor() { }

    public virtual void OffCursor() { }
}