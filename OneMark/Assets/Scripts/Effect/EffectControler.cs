using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControler : MonoBehaviour
{
    [SerializeField, Tooltip("Effect Objects")]
    private List<GameObject> m_effects = new List<GameObject>();

    private Dictionary<string, GameObject> m_effectDictionary = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var obj in m_effects)
        {
            m_effectDictionary.Add(obj.name, obj);
        }
    }
    public void OnEffect(string _effectName)
    {
        if (enabled)
        {
            m_effectDictionary[_effectName].SetActive(false);
        }
        m_effectDictionary[_effectName].SetActive(true);
    }

    public void OnEffect(int _effectNum)
    {
        if (enabled)
        {
            m_effects[_effectNum].SetActive(false);
        }
        m_effects[_effectNum].SetActive(true);
    }

    public void OffEffect(string _effectName)
    {
        m_effectDictionary[_effectName].SetActive(false);
    }

    public void OffEffect(int _effectNum)
    {
        m_effects[_effectNum].SetActive(false);
    }
}
