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
    //気づかれている状態
    public bool MasterDetected;
    //音
    public AudioSource BGM;
    public AudioSource FueSound;
    //ゲーム終了
    public bool Finish;
    //ゲーム開始
    public bool Starting;
    public AudioSource ReadySound;
    public AudioSource StartSound;
    public float StartTimer;
    public GameObject ReadyText;
    public GameObject GoText;
    public GameObject FinishText;
    public GameObject GetCaughtText;

    // Start is called before the first frame update
    void Start()
    {
        //ゲーム時間
        TimeCount = 180f;
        //値を初期化
        CheeseCount = 0;
        iscaughtTimes = 0;
        escapedTimes = 0;
        clashTimes = 0;
        //ゲーム開始
        ReadySound.Play();
        StartTimer = 2f;
        ReadyText.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //捕まったらテキストを出す
        if (iscaughtTimes != 0)
        {
            GetCaughtText.SetActive(true);
            return;
        }
        //ゲーム開始の準備
        StartTimer -= Time.deltaTime;
        if(StartTimer <= 0f)
        {
            if(!Starting)
            {
                BGM.Play();
                StartSound.Play();
                Starting = true;
                GoText.SetActive(true);
                ReadyText.SetActive(false);
            }
        }
        if(TimeCount <= 179f)
        {
            GoText.SetActive(false);
        }
        if(!Starting)
        {
            return;
        }
        if(!Finish)
        {
            TimeCount -= Time.deltaTime;
        }
        //制限時間
        seconds = (int)TimeCount;
        TimeCountText.text = "TIME\n" + seconds.ToString();

        CheeseCountText.text = CheeseCount.ToString() + "こ";

        if(TimeCount <= 0)
        {
            if(!Finish)
            {
                Finish = true;
                Debug.Log("Time Up!!");
                FueSound.Play();
                FinishText.SetActive(true);
                StartCoroutine(Checking(() => {
                    Substitute();
                    isGameOver = false;
                    SceneManager.LoadScene("Result");
                }));
            }
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
