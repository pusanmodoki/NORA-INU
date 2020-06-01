using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSelectedObject : MonoBehaviour
{
	[SerializeField]
	protected bool m_isSelected = false;
	public bool isSelected { get { return m_isSelected; } set { m_isSelected = value; } }

	public MenuInput menu { get; private set; }

	public void SetMenu(MenuInput _menu)
	{
		menu = _menu;
	}

	public abstract void OnEnter();

	public abstract void AwakeCursor();
	public abstract void OnCursor();

	public abstract void OffCursor();
}