using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantsList : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> m_servants;

    /// <summary>
    /// 下僕を追加
    /// </summary>
    /// <param name="male"></param>
    public void AddServant(GameObject male)
    {
        m_servants.Add(male);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="male"></param>
    public void RemoveServant(GameObject male)
    {
    }

    public KamikazeCommand GetKamikaze(int id)
    {
        if(m_servants.Count <= id)
        {
            return null;
        }
        return m_servants[id].GetComponent<KamikazeCommand>();
    }

}
