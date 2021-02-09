using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    public AudioSource PushButtonSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        PushButtonSound.Play();
        StartCoroutine(Checking(() => {
            SceneManager.LoadScene("Tutorial");
        }));
    }

    public void PushRecordingButton()
    {
        PushButtonSound.Play();
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
}
