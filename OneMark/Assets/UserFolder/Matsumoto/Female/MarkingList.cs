using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkingList : MonoBehaviour
{
    List<GameObject> m_markings;

    public void AddMarking(GameObject male)
    {
        m_markings.Add(male);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="male"></param>
    public void RemoveMarking(GameObject male)
    {
        m_markings.Remove(male);
    }

}
