using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public GameData GameData;

    public AudioSource PushButtonSound;
    public AudioSource PushWrongSound;

    public GameObject[] InitialTitleObjects;
    public GameObject[] OptionTitleObjects;

    public Slider ModeChangeSlider;
    public Slider SensitivitySlider;

    public int ModeNum;
    public int SensitivityNum;
    public int OldModeNum;
    public int OldSensitivityNum;

    public Text OnOffText;
    public Text SMLText;

    public AudioSource SwitchSound;

    // Start is called before the first frame update
    void Start()
    {
        if(GameData.iPhone)
        {
            ModeChangeSlider.value = 1.0f;
        }
        else if(!GameData.iPhone)
        {
            ModeChangeSlider.value = 0f;
        }

        switch (GameData.rotateSpeed)
        {
            case 1.0f:
                SensitivitySlider.value = 0f;
                SMLText.text = "S";
                break;
            case 2.0f:
                SensitivitySlider.value = 1.0f;
                SMLText.text = "M";
                break;
            case 3.0f:
                SensitivitySlider.value = 2.0f;
                SMLText.text = "L";
                break;
        }

        ModeNum = (int)ModeChangeSlider.value;
        SensitivityNum = (int)SensitivitySlider.value;
        OldModeNum = ModeNum;
        OldSensitivityNum = SensitivityNum;
    }

    // Update is called once per frame
    void Update()
    {
        ModeNum = (int)ModeChangeSlider.value;
        SensitivityNum = (int)SensitivitySlider.value;

        if(ModeNum == 0)
        {
            GameData.iPhone = false;
            OnOffText.text = "OFF";
            OnOffText.color = Color.red;
        }
        else if(ModeNum == 1)
        {
            GameData.iPhone = true;
            OnOffText.text = "ON";
            OnOffText.color = Color.blue;
        }

        switch(SensitivityNum)
        {
            case 0:
                GameData.rotateSpeed = 1.0f;
                SMLText.text = "S";
                break;
            case 1:
                GameData.rotateSpeed = 2.0f;
                SMLText.text = "M";
                break;
            case 2:
                GameData.rotateSpeed = 3.0f;
                SMLText.text = "L";
                break;
        }

        if (OldModeNum != ModeNum)
        {
            OldModeNum = ModeNum;
            SwitchSound.Play();
        }
        if (OldSensitivityNum != SensitivityNum)
        {
            OldSensitivityNum = SensitivityNum;
            SwitchSound.Play();
        }
    }

    public void PushPlayButton()
    {
        PushButtonSound.Play();
        GameData.Tutorial = false;
        StartCoroutine(Checking(() => {
            SceneManager.LoadScene("Main");
        }));
    }

    public void PushTutorialButton()
    {
        PushButtonSound.Play();
        GameData.Tutorial = true;
        StartCoroutine(Checking(() => {
            SceneManager.LoadScene("Main");
        }));
    }

    public void PushRecordingButton()
    {
        PushButtonSound.Play();
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(0);
    }

    public delegate void functionType();
    private IEnumerator Checking(functionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!PushButtonSound.isPlaying)
            {
                callback();
                break;
            }
        }
    }

    public void ClickOption()
    {
        PushButtonSound.Play();
        for (int i = 0; i <= 4; i++)
        {
            InitialTitleObjects[i].SetActive(false);
        }
        for (int i = 0; i <= 7; i++)
        {
            OptionTitleObjects[i].SetActive(true);
        }
    }

    public void ClickReverse()
    {
        PushButtonSound.Play();
        for (int i = 0; i <= 4; i++)
        {
            InitialTitleObjects[i].SetActive(true);
        }
        for (int i = 0; i <= 7; i++)
        {
            OptionTitleObjects[i].SetActive(false);
        }
    }
}
