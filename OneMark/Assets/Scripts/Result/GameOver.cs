using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private PlayerInput overAnime = null;

    [SerializeField]
    private FollowObject result = null;

    // Start is called before the first frame update
    void Start()
    {
        overAnime.GameOverAnimation();
        ResultCall.GameOver();
        result.ResultFlg();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
