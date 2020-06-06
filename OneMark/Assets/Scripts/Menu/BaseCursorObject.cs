using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCursorObject : MonoBehaviour
{

    public MenuInput menu { get; private set; } = null;

    protected int m_nowSelectIndex = 0;  

    public void SetMenu(MenuInput _menu)
    {
        menu = _menu;
    }
}
