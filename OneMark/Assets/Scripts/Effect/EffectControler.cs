using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectControler : MonoBehaviour
{
    [SerializeField, Tooltip("Effect Objects")]
    private List<GameObject> m_effects = new List<GameObject>();

    private Dictionary<string, GameObject> m_effectDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, ParticleSystem> m_particleSystems = new Dictionary<string, ParticleSystem>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(var obj in m_effects)
        {
            m_effectDictionary.Add(obj.name, obj);
            m_particleSystems.Add(obj.name, obj.GetComponent<ParticleSystem>());
        }
    }
    public void OnEffectByString(string _effectName)
    {
		if (!m_effectDictionary.ContainsKey(_effectName))
		{
#if UNITY_EDITOR
			Debug.Log("!m_effectDictionary.ContainsKey(_effectName) : " + _effectName);
#endif
			return;
		}
		if (m_effectDictionary[_effectName].activeSelf)
        {
            m_effectDictionary[_effectName].SetActive(false);
        }
        m_effectDictionary[_effectName].SetActive(true);
    }

    public void OnEffectByInteger(int _effectNum)
	{
		if (m_effects.Count < _effectNum)
		{
#if UNITY_EDITOR
			Debug.Log("m_effects.Count < _effectNum : " + _effectNum);
#endif
			return;
		}

		if (m_effects[_effectNum].activeSelf)
        {
            m_effects[_effectNum].SetActive(false);
        }
        m_effects[_effectNum].SetActive(true);
    }

    public void OffEffectByString(string _effectName)
    {
        m_effectDictionary[_effectName].SetActive(false);
    }

    public void OffEffectByInteger(int _effectNum)
    {
        m_effects[_effectNum].SetActive(false);
    }

    public bool IsPlayByString(string _effectName)
    {
        return m_effectDictionary[_effectName].activeSelf;
    }
    public bool IsPlayByInteger(int _effectNum)
    {
        return m_effects[_effectNum].activeSelf;
    }

    public ParticleSystem GetParticleSystem(string _effectName)
    {
        return m_particleSystems[_effectName];
    }
}
