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

    public Text OnOffText;
    public Text SMLText;

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    public void PushPlayButton()
    {
        PushButtonSound.Play();
        StartCoroutine(Checking(() => {
            SceneManager.LoadScene("Main");
        }));
    }

    public void PushTutorialButton()
    {
        //PushButtonSound.Play();
        PushWrongSound.Play();
        StartCoroutine(Checking(() => {
            SceneManager.LoadScene("Tutorial");
        }));
    }

    public void PushRecordingButton()
    {
        //PushButtonSound.Play();
        PushWrongSound.Play();
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
