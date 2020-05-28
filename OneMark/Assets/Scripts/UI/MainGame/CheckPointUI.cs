using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointUI : MonoBehaviour
{
    public BaseCheckPoint checkPoint { get; set; }

    [SerializeField]
    private Image m_boneImage = null;
    [SerializeField]
    private Image m_notBoneImage = null;

    [SerializeField]
    private Animator m_animator = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (checkPoint.isLinked)
        {
            OnUIEffect();
        }
        else
        {
            LostEffect();
        }
    }

    private void OnUIEffect()
    {
        if (m_boneImage.gameObject.activeSelf) { return; }
        m_boneImage.gameObject.SetActive(true);
        if (!m_notBoneImage.gameObject.activeSelf) { return; }
        m_notBoneImage.gameObject.SetActive(false);
    }

    private void LostEffect()
    {
        m_animator.SetTrigger("Lost");
    }

    private void OffUIEffect()
    {
        if (!m_boneImage.gameObject.activeSelf) { return; }
        m_boneImage.gameObject.SetActive(false);
        if (m_notBoneImage.gameObject.activeSelf) { return; }
        m_notBoneImage.gameObject.SetActive(true);
    }
}
