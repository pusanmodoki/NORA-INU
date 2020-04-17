using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSoundPlayer : MonoBehaviour
{
    [SerializeField]
    AudioSource select = null;

    [SerializeField]
    AudioSource enter = null;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonDown("Vertical"))
        {
            select.Play();
        }
        if (Input.GetButtonDown("Submit"))
        {
            enter.Play();
        }
        
    }
}
