using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    //public Transform target;
    //NavMeshAgent agent;

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
    }

    // Update is called once per frame
    void Update()
    {
        //agent.SetDestination(target.position);

        if(GameManager.isGameOver)
        {
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
        //アニメーションの早さ
        animator.SetFloat("Speed", 2);

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
            navMeshAgent.SetDestination(target.transform.position);
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
        //ゲームオーバー、ステルス状態、既に発見しているとき、発見しない
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
        Debug.Log("発見ニャ！");
        exclamationPop.SetActive(true);
        audioSource.Play();
        isDetected = true; 
    }
    //プレイヤーを見失ったとき
    public void MissPlayer()
    {
        Debug.Log("見失ったニャ！");
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
            exclamationPop.SetActive(false);
            //gameEnding.IsCaught();
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            isDetected = false;
            speed = 0;
            GameManager.isGameOver = true;
            PunchSound.Play();
            TinSound.Play();
            StartCoroutine(Checking(() => {
                GameManager.isGameOver = false;
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
