using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //データを代入する
    public GameData GameData;
    //制限時間
    public float TimeCount;
    public int seconds;
    public Text TimeCountText;
    //ポイント倍率
    public int iscaughtTimes;
    public int escapedTimes;
    public int clashTimes;
    //チーズ獲得数
    public int CheeseCount;
    public Text CheeseCountText;
    //ゲームオーバー
    public bool isGameOver;
    //気絶
    public bool isFaint;
    //音
    public AudioSource BGM;
    public AudioSource FueSound;

    // Start is called before the first frame update
    void Start()
    {
        //ゲーム時間
        TimeCount = 120;
        //値を初期化
        CheeseCount = 0;
        iscaughtTimes = 0;
        escapedTimes = 0;
        clashTimes = 0;
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
            BGM.Stop();
            FueSound.Play();
            StartCoroutine(Checking(() => {
                Substitute();
                isGameOver = false;
                SceneManager.LoadScene("Result");
            }));
        }
    }

    public void Substitute()
    {
        GameData.CheeseCount = CheeseCount;
        GameData.iscaughtTimes = iscaughtTimes;
        GameData.escapedTimes = escapedTimes;
        GameData.clashTimes = clashTimes;
    }

    public delegate void functionType();
    private IEnumerator Checking(functionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!FueSound.isPlaying)
            {
                callback();
                break;
            }
        }
    }
}
