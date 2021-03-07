using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGravity : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity;
    private Rigidbody rBody;

    public PlayerScript Player;
    public int num;

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
            if(rBody.useGravity == false)
            {
                rBody.useGravity = true;
                switch (num)
                {
                    case 1:
                        transform.rotation = Quaternion.Euler(0, 90, 0);
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
        switch (num)
        {
            case 1:
                //壁突入時の角度はy = 0, 180 <= y < 360
                localGravity = new Vector3 (-10, 0, 0);
                if(_Rotation.y >= 180 && _Rotation.y <= 270)
                {

                }
                transform.rotation = Quaternion.Euler(-90, -90, 0);
                break;
        }    
    }
}