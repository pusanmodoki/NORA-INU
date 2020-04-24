using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField]
    private PlayerInput clear;

    // Start is called before the first frame update
    void Start()
    {
        clear.GameClearAnimation();
        ResultCall.GameClear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
