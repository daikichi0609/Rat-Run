﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameManagerScript GameManager;
    //プレイヤー
    public GameObject Player;
    //データを代入する
    public GameData GameData;
    //HorizontalとVertical
    public float h;
    public float v; 
    // 前進速度
    float forwardSpeed = 5.5f;
    //後退速度
    float backSpeed = 2.0f;
    //旋回速度
    float rotateSpeed = 3.0f;
    //アニメーター
    Animator animator;
    //カメラ
    public GameObject MainCamera;
    public GameObject SubCamera;
    public GameObject StealthCamera;
    bool MainOn;
    //ステルス状態
    public bool Stealth;
    public bool StealthOn;
    //音
    public GameObject RunningSound;
    public GameObject WalkingSound;
    public AudioSource CheeseGetSound;
    public AudioSource CrashSound;
    public AudioSource FaintSound;
    //タイマー
    float countTime = 0;
    //吹き飛ばし
    Rigidbody rigidBody;
    float impulse = 1;
    Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        MainOn = true;
        Stealth = true;
        StealthOn = true;
        //アニメーションの早さ
        //animator.SetFloat("Speed", 2);

        rigidBody = Player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        playerVelocity = rigidBody.velocity;
        Debug.Log(playerVelocity);
        //気絶中
        if (GameManager.isFaint)
        {
            countTime += Time.deltaTime; //スタートしてからの秒数を格納
            if (countTime >= 3f)
            {
                countTime = 0;
                GameManager.isFaint = false;
                FaintSound.Stop();
            }
        }
        //ゲームオーバーと気絶のアニメーション
        if (GameManager.isGameOver || GameManager.isFaint)
        {
            animator.SetBool("IsGameOver", true);
            animator.SetBool("IsWalking", false);
            return;
        }
        else if (!GameManager.isGameOver)
        {
            animator.SetBool("IsGameOver", false);
        }

        h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
        v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義

        // 以下、キャラクターの移動処理
        if (v >= 0)
        {
            Vector3 velocity = gameObject.transform.rotation * new Vector3(0, 0, v * forwardSpeed);
            //gameObject.transform.position += velocity * Time.deltaTime;
            rigidBody.velocity = velocity;
        }
        else if (v < 0)
        {
            Vector3 velocity = gameObject.transform.rotation * new Vector3(0, 0, v * backSpeed);
            //gameObject.transform.position += velocity * Time.deltaTime;
            rigidBody.velocity = velocity;
        }
        //移動のアニメーション
        if (v != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else if (v == 0)
        {
            animator.SetBool("IsWalking", false);
            RunningSound.SetActive(false);
            WalkingSound.SetActive(false);
        }
        //アニメーションの早さ
        if (v > 0)
        {
            animator.SetFloat("Speed", forwardSpeed);
            RunningSound.SetActive(true);
            WalkingSound.SetActive(false);
        }
        else if (v < 0)
        {
            animator.SetFloat("Speed", -backSpeed);
            WalkingSound.SetActive(true);
            RunningSound.SetActive(false);
        }

        // 左右のキー入力でキャラクタをY軸で旋回させる
        Vector3 playerPos = transform.position;
        transform.RotateAround(playerPos, Vector3.up, h * rotateSpeed);

        //if(Input.GetKey(KeyCode.W))
        //{
        //    Debug.Log("W");
        //    transform.position += new Vector3(0, 0, speed * Time.deltaTime);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    Debug.Log("A");
        //    transform.Rotate (new Vector3(0, -rotateSpeed * Time.deltaTime, 0));
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    Debug.Log("D");
        //    transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        //}

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{

        //}
        //if(Input.GetKeyDown(KeyCode.LeftArrow))
        //{

        //}
        //if(Input.GetKeyDown(KeyCode.UpArrow))
        //{

        //}
        //if(Input.GetKeyDown(KeyCode.DownArrow))
        //{

        //}

        //カメラ管理
        if(Stealth)
        {
            //ステルス時は専用のカメラを使う
            StealthOn = true;
            StealthCamera.SetActive(true);
            MainOn = true;
            MainCamera.SetActive(false);
            SubCamera.SetActive(false);
        }
        else if(!Stealth)
        {
            //ステルス解除時にメインカメラに切替
            if(StealthOn)
            {
                StealthOn = false;
                StealthCamera.SetActive(false);
                MainCamera.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Stealth)
            {
                return;
            }
            //メインカメラとサブカメラの切替
            else if(MainOn)
            {
                MainOn = false;
                MainCamera.SetActive(false);
                SubCamera.SetActive(true);
                return;
            }
            else if(!MainOn)
            {
                MainOn = true;
                MainCamera.SetActive(true);
                SubCamera.SetActive(false);
                return;
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        //敵に触れたとき
         if(collision.gameObject.tag == "Enemy")
        {
            Debug.Log("GameOver");
            GameManager.iscaughtTimes++;
            GameManager.isGameOver = true;
        }
         //壁に衝突したとき
         else
        {
            if(!Stealth && v == 1)
            {
                GameManager.clashTimes++;
                GameManager.isFaint = true;
                CrashSound.Play();
                FaintSound.Play();
                rigidBody.AddForce(playerVelocity * -impulse, ForceMode.Impulse);
            }  
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //物陰に隠れたとき
        if(other.gameObject.tag == "Stealth")
        {
            Stealth = true;
        }
        //チーズをゲットしたとき
        if (other.gameObject.tag == "Cheese")
        {
            Destroy(other.gameObject);
            GameManager.CheeseCount++;
            CheeseGetSound.Play();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //物陰から出たとき
        if(other.gameObject.tag == "Stealth")
        {
            Stealth = false;
        }
    }
}
