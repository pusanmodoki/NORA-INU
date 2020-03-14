using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ズームイン最小値")]
    private float minZoom;

    [SerializeField]
    [Tooltip("ズームアウト最大値")]
    private float maxZoom;

    private int m_zoomMode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            this.transform.localPosition = new Vector3(0.0f, maxZoom, -10.0f);
        }

    }
}
