using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    //public Transform target;
    //NavMeshAgent agent;

    public GameData GameData;
    public GameManagerScript GameManager;
    //プレイヤー
    public PlayerScript Player;
    public NavMeshAgent navMeshAgent;
    //プレイヤーを見失う距離
    public float loseSightDistance;
    //ステージ内を巡回するポイント
    public Transform[] waypoints;

    //public GameEnding gameEnding;

    //スネーク！！
    public GameObject exclamationPop;
    public GameObject detected;
    AudioSource audioSource;
    //プレイヤーの位置
    //Transform player;
    public GameObject target;

    //プレイヤー発見時にwaypointにタゲをなすりつけるのを防止
    public bool Waiting;
    public bool isDetected = false;
    //巡回をランダムにする
    int currentWaypointIndex;
    int oldWaypointIndex;
    
    //速さを取得
    Rigidbody rigid;
    float speed;
    //アニメーター
    Animator animator;
    //音
    public AudioSource SuccessSound;
    public AudioSource TinSound;
    public AudioSource PunchSound;
    //クールタイム
    public float CT;

    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //player = target.transform; //プレイヤーの位置・角度・回転を取得する

        currentWaypointIndex = Random.Range(0, 8);
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        oldWaypointIndex = currentWaypointIndex;
        audioSource = detected.GetComponent<AudioSource>();

        rigid = this.gameObject.GetComponent<Rigidbody>();

        animator = GetComponent<Animator>();
        //アニメーションの早さ
        animator.SetFloat("Speed", 2);
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(target.position);

        //時間停止中は実行しない
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        //CTカウント
        if (CT >= 0)
        {
            CT -= Time.deltaTime;
        }

        if (GameManager.isGameOver)
        {
            speed = 0;
            return;
        }
        //状況に応じてアニメーションを変化
        speed = rigid.velocity.magnitude;
        //Debug.Log(speed);
        if (speed <= 0)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
        else if (speed > 0 && !isDetected)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
        }
        else if (speed > 0 && isDetected)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", false);
        }

        //waypoint到着時に呼ばれる
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !isDetected)
        {
            GetNewNum();
            //currentWaypointIndex = currentWaypointIndex % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        //プレイヤーに気づいているとき
        if (isDetected)
        {
            GameManager.MasterDetected = true;
            navMeshAgent.SetDestination(target.transform.position);
            //遅延
            if(!Waiting)
            {
                Waiting = true;
                return;
            }
        }
        //プレイヤーを見失うとき
        if ((navMeshAgent.remainingDistance >= loseSightDistance) && isDetected)
        {
            MissPlayer();
        }
        else if(Player.Stealth && isDetected)
        {
            MissPlayer();
        }
    }
    //プレイヤーを発見したとき
    public void DetectPlayer()
    {
        //ゲームオーバー、ステルス状態、既に発見しているとき、クールダウン中、発見しない
        if(GameManager.isGameOver)
        {
            return;
        }
        if(Player.Stealth)
        {
            return;
        }
        if(isDetected)
        {
            return;
        }
        if(CT > 0f)
        {
            return;
        }
        Debug.Log("発見ニャ！");
        exclamationPop.SetActive(true);
        audioSource.Play();
        isDetected = true;
        CT = 3.0f;
    }
    //プレイヤーを見失ったとき
    public void MissPlayer()
    {
        GameManager.MasterDetected = false;
        Debug.Log("見失ったニャ！");
        GameManager.escapedTimes++;
        exclamationPop.SetActive(false);
        SuccessSound.Play();
        currentWaypointIndex = Random.Range(0, 8);
        navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        isDetected = false;
        Waiting = false;
    }
    //ランダムで前と違う値を入手
    public void GetNewNum()
    {
        while (currentWaypointIndex == oldWaypointIndex)
        {
            currentWaypointIndex = Random.Range(0, 8);
        }
        oldWaypointIndex = currentWaypointIndex;
    }
    //プレイヤー到着時に呼ばれる
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(GameManager.isGameOver)
            {
                return;
            }
            //表示を消す
            exclamationPop.SetActive(false);
            //gameEnding.IsCaught();
            //アニメーションをアイドル状態にする
            //animator.SetBool("IsWalking", false);
            //animator.SetBool("IsRunning", false);

            //追尾をやめる
            isDetected = false;
            //音を鳴らす 鳴り終わったらシーン遷移
            PunchSound.Play();
            TinSound.Play();
            if (GameData.Tutorial)
            {
                return;
            }
            StartCoroutine(Checking(() => {
                GameManager.isGameOver = false;
                GameManager.Substitute();
                SceneManager.LoadScene("Result");
            }));
        }
    }

    public delegate void functionType();
    private IEnumerator Checking(functionType callback)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!TinSound.isPlaying)
            {
                callback();
                break;
            }
        }
    }
}
