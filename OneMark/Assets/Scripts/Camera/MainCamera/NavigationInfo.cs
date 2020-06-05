using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationInfo : MonoBehaviour
{
    public Transform lookPoint { get; set; } = null;

    public Transform movePoint { get; set; } = null;

    public float moveSpeed { get; set; } = 0.0f;

    public enum MoveMode
    {
        Normal = 0,
        Delay
    }

    MoveMode mode { get; set; } = MoveMode.Normal;
}