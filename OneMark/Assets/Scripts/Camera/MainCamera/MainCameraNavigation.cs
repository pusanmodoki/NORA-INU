using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraNavigation : MonoBehaviour
{

    [SerializeField]
    Dictionary<string, NavigationInfo> m_navInfo = new Dictionary<string, NavigationInfo>();

    [SerializeField]
    NavigationInfo m_nowNavigation = null;

    [SerializeField]
    string m_nowNavigationName = "";
    
    Transform m_startPoint = null;

    float m_t = 0.0f;
    float m_distance = 0.0f;


    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    void NormalNavigation()
    {
       
    }


    void SelectNavigation(string _name)
    {
        if (m_navInfo[_name] == null) { return; }
        m_nowNavigation = m_navInfo[_name];
    }
}
