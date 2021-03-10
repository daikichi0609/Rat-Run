using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultScript : MonoBehaviour
{
    //参照するデータ
    public GameData GameData;
    //テキスト
    public GameObject CheeseCountTitle;
    public Text CheeseCountText;

    public GameObject EscapeBonusTitle;
    public Text EscapeBonusText;

    public GameObject NoClashBonusTitle;
    public Text NoClashBonusText;

    public GameObject SurviveBonusTitle;
    public Text SurviveBonusText;

    public GameObject ScoreTitle;
    public Text ScoreText;
    public int Score;

    public GameObject RetryText;
    public GameObject TitleText;

    public GameObject RetryButton;
    public GameObject TitleButton;

    //音
    public AudioSource JanSound;
    public AudioSource JajanSound;
    public AudioSource PushSound;

    //表示演出関連
    int num;
    bool Finish;

    // Start is called before the first frame update
    void Start()
    {
        Calculate();
        num = 0;
        JanSound.Play();
        Invoke("ShowResult", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && Finish)
        {
            PushSound.Play();
            StartCoroutine(CheckingSound(() => {
                SceneManager.LoadScene("Main");
            }));
        }
        if (Input.GetKeyDown(KeyCode.T) && Finish)
        {
            PushSound.Play();
            StartCoroutine(CheckingSound(() => {
                SceneManager.LoadScene("Title");
            }));
        }
    }

    void ShowResult()
    {
        switch(num)
        {
            case 0:
                num++;
                Loop();
                CheeseCountTitle.SetActive(true);
                CheeseCountText.text = GameData.CheeseCount.ToString() + "pt";
                break;

            case 1:
                num++;
                Loop();
                EscapeBonusTitle.SetActive(true);
                EscapeBonusText.text = "+" + GameData.escapedTimes.ToString() + "pt";
                break;

            case 2:
                num++;
                Loop();
                NoClashBonusTitle.SetActive(true);
                if(GameData.clashTimes == 0)
                {
                    NoClashBonusText.text = "+5pt";
                }
                else
                {
                    NoClashBonusText.text = "+0pt";
                }
                break;

            case 3:
                num++;
                Loop();
                SurviveBonusTitle.SetActive(true);
                if (GameData.iscaughtTimes == 0)
                {
                    SurviveBonusText.text = "+10pt";
                }
                else
                {
                    SurviveBonusText.text = "+0pt";
                }
                break;

            case 4:
                num = 0;
                ShowFinish();
                ScoreTitle.SetActive(true);
                ScoreText.text = Score.ToString() + "pt";
                break;
        }
    }

    void Loop()
    {
        JanSound.Stop();
        JanSound.Play();
        Invoke("ShowResult", 1.0f);
    }

    void ShowFinish()
    {
        JajanSound.Play();
        StartCoroutine(CheckingSound(() => {
            Finish = true;
            if(!GameData.iPhone)
            {
                RetryText.SetActive(true);
                TitleText.SetActive(true);
            }
            else if(GameData.iPhone)
            {
                RetryButton.SetActive(true);
                TitleButton.SetActive(true);
            }
        }));
    }

    public void Retry()
    {
        PushSound.Play();
        StartCoroutine(CheckingSound(() => {
            SceneManager.LoadScene("Main");
        }));
    }

    public void Title()
    {
        PushSound.Play();
        StartCoroutine(CheckingSound(() => {
            SceneManager.LoadScene("Title");
        }));
    }

    void Calculate()
    {
        GameData.CheeseCount = GameData.CheeseCount * 2;
        Score = GameData.CheeseCount + GameData.escapedTimes;
        if (GameData.clashTimes == 0)
        {
            Score = Score + 5;
        }
        if (GameData.iscaughtTimes == 0)
        {
            Score = Score + 10;
        }
    }

    public delegate void functionType();
    private IEnumerator CheckingSound(functionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!JanSound.isPlaying && !PushSound.isPlaying)
            {
                callback();
                break;
            }
        }
    }
}
