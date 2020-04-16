using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointUI : MonoBehaviour
{
    public BaseCheckPoint checkPoint { get; set; }

    private Image flagImage;

    [SerializeField]
    private Color notLinkedColor = Color.gray;

    [SerializeField]
    private Color linkedColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        flagImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (!checkPoint.isLinked)
        {
            flagImage.color = notLinkedColor;
        }
        else
        {
            flagImage.color = linkedColor;
        }
    }
}
