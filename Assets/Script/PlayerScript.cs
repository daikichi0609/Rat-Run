using System.Collections;
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
    float forwardSpeed = 5.0f;
    //後退速度
    float backSpeed = 2.0f;
    //旋回速度s
    float rotateSpeed = 3.0f;
    //アニメーター
    Animator animator;
    //カメラ
    public GameObject MainCamera;
    public GameObject SubCamera;
    public GameObject StealthCamera;
    public GameObject ClimbCamera;
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
    float impulse = 100;
    Vector3 playerVelocity;
    //星
    public GameObject Stars;
    //壁登り
    public ChangeGravity ChangeGravity;
    public bool isClimbing;
    public bool ReadyToClimb;
    public bool ClimbOn;
    //クールタイム
    public float CT;
    //落下
    public float FallTime;
    public float Gravity;
    //カメラ移動
    public int CameraMoveCount;
    //透明壁エフェクト
    public GameObject RippleEffect;
    public ParticleSystem particle;
    //ジョイスティック
    public FloatingJoystick HorizontalJoystick;
    public FloatingJoystick VerticalJoystick;

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
        Gravity = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //カメラ移動
        //MainCameraMove();

        //CTカウント
        if (CT >= 0)
        {
            CT -= Time.deltaTime;
        }
        //落下カウント
        /*
        if (FallTime > 0)
        {
            FallTime -= Time.deltaTime;
            Gravity = -1f;
        }
        else if (FallTime <= 0)
        {
            Gravity = 0f;
        }
        */
        playerVelocity = rigidBody.velocity;
        //Debug.Log(playerVelocity);
        //最初は動けない
        if(GameManager.StartTimer >= 0)
        {
            return;
        }
        //制限時間が経つまで動ける
        if (GameManager.TimeCount <= 0)
        {
            return;
        }
        //壁登り条件
        if (!Stealth && !GameManager.isFaint && !GameManager.isGameOver && v == 1 && GameManager.MasterDetected == false && CT <= 0f)
        {
            ReadyToClimb = true;
        }
        else
        {
            ReadyToClimb = false;
        }
        //気絶中
        if (GameManager.isFaint)
        {
            animator.SetFloat("Faint", 100.0f);
            countTime += Time.deltaTime; //スタートしてからの秒数を格納
            if (countTime >= 2f)
            {
                countTime = 0;
                GameManager.isFaint = false;
                FaintSound.Stop();
            }
        }
        //ゲームオーバーと気絶のアニメーション
        if (GameManager.isGameOver)
        {
            animator.SetBool("IsGameOver", true);
            animator.SetBool("IsWalking", false);
            RunningSound.SetActive(false);
            WalkingSound.SetActive(false);
            Stars.SetActive(true);
            return;
        }
        else if(GameManager.isFaint)
        {
            animator.SetBool("IsFainting", true);
            animator.SetBool("IsWalking", false);
            RunningSound.SetActive(false);
            WalkingSound.SetActive(false);
            Stars.SetActive(true);
            return;
        }
        else if (!GameManager.isGameOver && !GameManager.isFaint)
        {
            animator.SetBool("IsGameOver", false);
            animator.SetBool("IsFainting", false);
            Stars.SetActive(false);
        }

        //PC
        h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
        v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義

        //スマホ
        h = HorizontalJoystick.Horizontal;
        v = VerticalJoystick.Vertical;

        // 以下、キャラクターの移動処理
        if (v >= 0)
        {
            //PC
            Vector3 velocity = gameObject.transform.rotation * new Vector3(0, Gravity, v * forwardSpeed);
            //gameObject.transform.position += velocity * Time.deltaTime;
            rigidBody.velocity = velocity;
        }
        else if (v < 0)
        {
            //PC
            Vector3 velocity = gameObject.transform.rotation * new Vector3(0, Gravity, v * backSpeed);
            //gameObject.transform.position += velocity * Time.deltaTime;
            //rigidBody.velocity = velocity;
            rigidBody.velocity = velocity;
        }

        if (!isClimbing)
        {
            // 左右のキー入力でキャラクタをY軸で旋回させる
            Vector3 playerPos = transform.position;
            transform.RotateAround(playerPos, Vector3.up, h * rotateSpeed);
        }
        else if(isClimbing)
        {
            switch (ChangeGravity.num)
            {
                case 1:
                    Vector3 playerPos = transform.position;
                    transform.RotateAround(playerPos, Vector3.right, h * rotateSpeed);
                    break;
            }
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
        else if(isClimbing)
        {
            //クライム中は専用のカメラを使う
            ClimbOn = true;
            ClimbCamera.SetActive(true);
            MainOn = true;
            MainCamera.SetActive(false);
            SubCamera.SetActive(false);
        }
        else if(!Stealth && !isClimbing)
        {
            //ステルス解除時とクライム終わりにメインカメラに切替
            if(StealthOn)
            {
                StealthOn = false;
                StealthCamera.SetActive(false);
                MainCamera.SetActive(true);
            }
            else if(ClimbOn)
            {
                ClimbOn = false;
                ClimbCamera.SetActive(false);
                MainCamera.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Stealth)
            {
                return;
            }
            if(isClimbing)
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
            if(GameManager.isGameOver)
            {
                return;
            }
            Debug.Log("GameOver");
            GameManager.iscaughtTimes++;
            GameManager.isGameOver = true;
            rigidBody.AddForce(playerVelocity * impulse, ForceMode.Impulse);
        }
         //壁に衝突したとき
         else
        {
            if(!Stealth && !GameManager.isFaint && !GameManager.isGameOver && v == 1 &&!isClimbing && CT <= 0f)
            {
                if(collision.gameObject.tag == "NotWall")
                {
                    foreach (ContactPoint point in collision.contacts)
                    {
                        //衝突位置
                        RippleEffect.transform.position = (Vector3)point.point;
                        particle.Play();
                    }
                        return;
                }
                CT = 3.0f;
                GameManager.clashTimes++;
                GameManager.isFaint = true;
                CrashSound.Play();
                FaintSound.Play();
                rigidBody.AddForce(playerVelocity * -impulse, ForceMode.Impulse);
            }  
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Plane" || other.gameObject.tag == "Wall")
        {
            Gravity = 0;
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
        //壁登り
        if(other.gameObject.tag == "Wall" && ReadyToClimb)
        {
            if(isClimbing)
            {
                return;
            }
            isClimbing = true;
            ChangeGravity.num = other.gameObject.GetComponent<WallScript>().num;
            ChangeGravity.ClimbWalls();
        }
        //壁登り解除
        if (other.gameObject.tag == "Plane" && isClimbing)
        {
            CT = 3.0f;
            isClimbing = false;
            Gravity = 0f;

            //カメラ移動
            //MainCamera.transform.position = StealthCamera.transform.position;
            //MainCamera.transform.rotation = StealthCamera.transform.rotation;
            //CameraMoveCount = 75;
        }
        if(other.gameObject.tag == "Plane" && !isClimbing)
        {
            //FallTime = 1.0f;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //物陰から出たとき
        if(other.gameObject.tag == "Stealth")
        {
            Stealth = false;
        }

        //落下
        if(other.gameObject.tag == "Plane")
        {
            if(!isClimbing)
            {
                Gravity = -5.0f;
            }
        }
    }

    /*
    public void MainCameraMove()
    {
        if(CameraMoveCount > 0)
        {
            MainCamera.gameObject.transform.Translate(0, 0.01f, -0.02f);
            MainCamera.transform.Rotate(new Vector3(0.6666666f, 0, 0));
            CameraMoveCount--;
        }
    }
    */
}
