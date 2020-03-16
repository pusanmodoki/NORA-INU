//--------------------------------------------------------------
// CameraSwitching.cs
// カメラ切り替え
// 作成者：綾野紗世
//--------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitching : MonoBehaviour
{
    [SerializeField]
    [Tooltip("カメラの通常の位置")]
    private float cameraNormalPosition;

    [SerializeField]
    [Tooltip("ズームイン最小値")]
    private float minZoom;

    [SerializeField]
    [Tooltip("ズームアウト最大値")]
    private float maxZoom;

    private int m_zoomMode;     // カメラの状態 (0：通常、1：ズームアウト、2：ズームイン)

    // Start is called before the first frame update
    void Start()
    {
        m_zoomMode = 0;

        // ズームイン・ズームアウトの軸
        this.transform.LookAt(this.transform.parent);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2")) // 右alt
        {
            if (m_zoomMode == 0)
            {
                this.transform.localPosition = -this.transform.forward * maxZoom; // ズームアウトする
                m_zoomMode = 1; // ズームアウトモードになる
            }
            else if (m_zoomMode == 1)
            {
                this.transform.localPosition = -this.transform.forward * minZoom;  // ズームインする
                m_zoomMode = 2; // ズームインモードになる
            }
            else if (m_zoomMode == 2)
            {
                this.transform.localPosition = -this.transform.forward * cameraNormalPosition; // 通常位置に戻る
                m_zoomMode = 0; // 通常モード
            }
        }
        else if (Input.GetButtonDown("Fire3")) // 右shift
        {
            if (m_zoomMode == 0)
            {
                this.transform.localPosition = -this.transform.forward * minZoom;  // ズームインする
                m_zoomMode = 2; // ズームインモードになる
            }
            else if (m_zoomMode == 1)
            {
                this.transform.localPosition = -this.transform.forward * cameraNormalPosition; // 通常位置に戻る
                m_zoomMode = 0;  // 通常モード
            }
            else if (m_zoomMode == 2)
            {
                this.transform.localPosition = -this.transform.forward * maxZoom;  // ズームアウトする
                m_zoomMode = 1; // ズームアウトモードになる            
            }
        }

    }
}
