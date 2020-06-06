using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultCursorObject : BaseCursorObject
{
    [SerializeField]
    Animator m_cursorAnimation = null;

    public RectTransform rectTransform { get; private set; } = null;

    public enum MoveType
    {
        Warp = 0,
        EqualApeed,
        Lerp
    }

    [SerializeField]
    MoveType type = MoveType.Warp;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.position = menu.selectedObjects[menu.nowSelectIndex].GetComponent<RectTransform>().position;
    }
}
