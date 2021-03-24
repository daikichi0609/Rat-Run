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
    public Text CheeseCountText;
    public Text EscapeBonusText;
    public Text NoClashBonusText;
    public Text SurviveBonusText;
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

    public bool[] start;
    int cheesecountBonus;
    int escapeBonus;
    int noclashBonus;
    int surviveBonus;
    int score;

    // Start is called before the first frame update
    void Start()
    {
        Calculate();
        num = 0;
        JanSound.Play();
        Loop();
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

        //スコア表示
        if(start[0])
        {
            CheeseCountText.text = cheesecountBonus.ToString() + "pt";
        }
        if (start[1])
        {
            EscapeBonusText.text = "+" + escapeBonus.ToString() + "pt";
        }
        if (start[2])
        {
            NoClashBonusText.text = "+" + noclashBonus.ToString() + "pt";
        }
        if (start[3])
        {
            SurviveBonusText.text = "+" + surviveBonus.ToString() + "pt";
        }
        if (start[4])
        {
            ScoreText.text = score.ToString() + "pt";
        }
        
        //増加処理
        if (start[0] && GameData.CheeseCount != cheesecountBonus)
        {
            cheesecountBonus = cheesecountBonus + 100;
            if(GameData.CheeseCount == cheesecountBonus)
            {
                JanSound.Play();
                Loop();
            }
        }
        if (start[1] && GameData.escapedTimes != escapeBonus)
        {
            escapeBonus = escapeBonus + 100;
            if (GameData.escapedTimes == escapeBonus)
            {
                JanSound.Play();
                Loop();
            }
        }
        if (start[2] && noclashBonus != 1000)
        {
            noclashBonus = noclashBonus + 100;
            if (noclashBonus == 1000)
            {
                JanSound.Play();
                Loop();
            }
        }
        if (start[3] && surviveBonus != 2000)
        {
            surviveBonus = surviveBonus + 100;
            if (surviveBonus == 2000)
            {
                JanSound.Play();
                Loop();
            }
        }
        if (start[4] && Score != score)
        {
            score = score + 100;
            if (Score == score)
            {
                JajanSound.Play();
                Invoke("ShowFinish", 1f);
            }
        }
    }

    void ShowResult()
    {
        switch(num)
        {
            case 0:
                num++;
                start[0] = true;
                if(GameData.CheeseCount == 0)
                {
                    JanSound.Play();
                    Loop();
                }
                break;

            case 1:
                num++;
                start[1] = true;
                if (GameData.escapedTimes == 0)
                {
                    JanSound.Play();
                    Loop();
                }
                break;

            case 2:
                num++;
                if(GameData.clashTimes == 0)
                {
                    start[2] = true;
                }
                else
                {
                    start[2] = false;
                    NoClashBonusText.text = "+" + noclashBonus.ToString() + "pt";
                    JanSound.Play();
                    Loop();
                }
                break;

            case 3:
                num++;
                if (GameData.iscaughtTimes == 0)
                {
                    start[3] = true;
                }
                else
                {
                    start[3] = false;
                    SurviveBonusText.text = "+" + surviveBonus.ToString() + "pt";
                    JanSound.Play();
                    Loop();
                }
                break;

            case 4:
                num = 0;
                start[4] = true;
                if(Score == 0)
                {
                    JajanSound.Play();
                    Invoke("ShowFinish", 1f);
                }
                break;
        }
    }

    void Loop()
    {
        Invoke("ShowResult", 1f);
        Debug.Log("ループ");
    }

    void ShowFinish()
    {
        StartCoroutine(CheckingSound(() => {
            Finish = true;
            //スコア機能
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(Score);
            if (!GameData.iPhone)
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
        GameData.CheeseCount = GameData.CheeseCount * 200;
        GameData.escapedTimes = GameData.escapedTimes * 100;
        Score = GameData.CheeseCount + GameData.escapedTimes;
        if (GameData.clashTimes == 0)
        {
            Score = Score + 1000;
        }
        if (GameData.iscaughtTimes == 0)
        {
            Score = Score + 2000;
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
