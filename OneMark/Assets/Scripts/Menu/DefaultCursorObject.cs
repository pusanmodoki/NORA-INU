using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultCursorObject : BaseCursorObject
{
    public RectTransform rectTransform { get; private set; } = null;

    public enum MoveType
    {
        Warp = 0,
        EqualApeed,
        Lerp
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

	public override void UpSelect()
	{
		rectTransform.position = menu.selectedObjects[menu.nowSelectIndex].GetComponent<RectTransform>().position;
	}

	public override void DownSelect()
	{
		rectTransform.position = menu.selectedObjects[menu.nowSelectIndex].GetComponent<RectTransform>().position;
	}

	public override void Enter()
	{
		throw new System.NotImplementedException();
	}
}
