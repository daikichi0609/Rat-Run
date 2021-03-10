using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravity : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity;
    private Rigidbody rBody;

    public PlayerScript Player;
    public int num;

    public GameObject BackBedWallStopper;

    // Use this for initialization
    private void Start()
    {
        rBody = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(Player.isClimbing)
        {
            rBody.useGravity = false; //rigidBodyの重力を使わなくする
            SetLocalGravity(); //重力をAddForceでかけるメソッドを呼ぶ。FixedUpdateが好ましい。
        }
        else if(!Player.isClimbing)
        {
            //壁登り解除時
            if(rBody.useGravity == false)
            {
                rBody.useGravity = true;
                switch (num)
                {
                    //キッチン壁
                    case 1:
                        transform.rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    //ベッド後ろ壁
                    case 2:
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                        BackBedWallStopper.SetActive(false);
                        break;

                }
            }
        }
    }

    private void SetLocalGravity()
    {
        rBody.AddForce(localGravity, ForceMode.Acceleration);
    }

    public void ClimbWalls()
    {
        Vector3 _Rotation = gameObject.transform.localEulerAngles;
        Debug.Log(_Rotation);
        //重力設定
        switch (num)
        {
            case 1:
                localGravity = new Vector3 (-10, 0, 0);
                //壁突入時の角度はy = 0, 180 <= y < 360
                if (_Rotation.y >= 180 && _Rotation.y <= 270)
                {

                }
                //妥協
                transform.rotation = Quaternion.Euler(-90, -90, 0);
                break;

            case 2:
                localGravity = new Vector3(0, 0, 10);
                transform.rotation = Quaternion.Euler(-90, 0, 0);
                BackBedWallStopper.SetActive(true);
                break;
        }    
    }
}