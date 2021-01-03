using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    //プレイヤー
    public PlayerScript Player;
    //制限時間
    public float TimeCount;
    public int seconds;
    public Text TimeCountText;
    //チーズ獲得数
    public Text CheeseCountText;

    // Start is called before the first frame update
    void Start()
    {
        TimeCount = 60;
        Player.CheeseCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount -= Time.deltaTime;
        seconds = (int)TimeCount;
        TimeCountText.text = seconds.ToString();

        CheeseCountText.text = Player.CheeseCount.ToString();

        if(TimeCount <= 0)
        {
            Debug.Log("Time Up!!");
        }
    }
}
