using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField]
    private PlayerInput clearAnime;

    [SerializeField]
    private FollowObject result;

    // Start is called before the first frame update
    void Start()
    {
        clearAnime.GameClearAnimation();
        ResultCall.GameClear();
        result.ResultFlg();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
