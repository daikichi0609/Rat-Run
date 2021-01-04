using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    //制限時間
    public float TimeCount;
    public int seconds;
    public Text TimeCountText;
    //チーズ獲得数
    public int CheeseCount;
    public Text CheeseCountText;
    //ポイント
    public int pt;
    //ゲームオーバー
    public bool isGameOver;

    // Start is called before the first frame update
    void Start()
    {
        TimeCount = 120;
        CheeseCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        TimeCount -= Time.deltaTime;
        seconds = (int)TimeCount;
        TimeCountText.text = "TIME\n" + seconds.ToString();

        CheeseCountText.text = CheeseCount.ToString() + "こ";

        if(TimeCount <= 0)
        {
            Debug.Log("Time Up!!");
        }
    }
}
