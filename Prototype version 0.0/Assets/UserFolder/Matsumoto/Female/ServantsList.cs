using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantsList : MonoBehaviour
{
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
}
