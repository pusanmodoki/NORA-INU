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

}
