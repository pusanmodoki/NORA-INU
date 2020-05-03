using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private PlayerInput overAnime;

    [SerializeField]
    private FollowObject result;

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
