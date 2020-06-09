using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCursorObject : MonoBehaviour
{

    public MenuInput menu { get; private set; } = null;

    protected int m_nowSelectIndex = 0;  

    public void SetMenu(MenuInput _menu)
    {
        menu = _menu;
    }

	public abstract void UpSelect();

	public abstract void DownSelect();

	public abstract void Enter();

	public virtual void SelectTopIndex() { }

	public virtual void SelectBottomIndex() { }

	public void SelectUpdate(int _direction)
	{
		if(_direction > 0)
		{
			UpSelect();
		}
		else if(_direction < 0)
		{
			DownSelect();
		}
	} 
}
