using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantsList : MonoBehaviour
{
    private List<KamikazeCommand> m_servants;

    /// <summary>
    /// 下僕を追加
    /// </summary>
    /// <param name="male"></param>
    public void AddServant(KamikazeCommand male)
    {
        m_servants.Add(male);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="male"></param>
    public void SubServant(KamikazeCommand male)
    {
    }
}
