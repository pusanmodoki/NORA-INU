using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Female : MonoBehaviour
{
    /// <summary>
    /// オスのリスト
    /// </summary>
    private ServantsList m_servants;
    private MarkingList m_markings;

    private int m_nextShotServant = 0;

    private float timeOutSecond = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        m_servants = GetComponent<ServantsList>();
        m_markings = GetComponent<MarkingList>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShotServant(Vector3 direction)
    {
       // m_servants.GetKamikaze(m_nextShotServant).InvokeCommand(direction, timeOutSecond);
    }

}
