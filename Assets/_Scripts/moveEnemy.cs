using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Rigidbodyコンポーネントが必須
[RequireComponent(typeof(Rigidbody))]

public class moveEnemy : MonoBehaviour
{
    Rigidbody rb;

    [Header("プレイヤーオブジェクト名")]
    public string playerObjName = "Akaza_sum"; //プレイヤーオブジェクト名
    GameObject playerObj;
    Transform playerTransform;

    [Header("移動速度")]
    public float speed = 200.0f;

    [Header("視野")]
    public float visionLength = 10.0f;
    public float visionAngle = 90.0f;

    void Start()
    {
        //Rigidbodyコンポーネントを取得する
        rb = this.GetComponent<Rigidbody>();
        if (rb == null) //rididbodyが見つからなければエラー
        {
            Debug.LogError("Rigidbodyが見つかりません");
            return;
        }

        //プレイヤーオブジェクトを取得する
        playerObj = GameObject.Find(playerObjName);
        if (playerObj == null) //プレイヤーオブジェクトが見つからなければエラー
        {
            Debug.LogError("プレイヤーキャラクターが見つかりません");
            return;
        }
        playerTransform = playerObj.transform; //プレイヤーのtransformを取得
    }

    void Update()
    {
        Vector3 rayVec = playerTransform.position - this.transform.position; //敵からプレイヤーへの方向

        float sa = Mathf.Abs(Vector3.Angle(rayVec, transform.forward));

        if (sa < visionAngle / 2)
        {
            Ray ray = new Ray(transform.position + new Vector3(0.0f, 0.7f, 0.0f), rayVec);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionLength))
            {
                if (hit.transform.gameObject.name == playerObjName)
                {


                    //プレイヤーキャラクターへ向かって移動する
                    Vector3 moveForward = rayVec * speed * Time.deltaTime;
                    rb.velocity = new Vector3(moveForward.x, rb.velocity.y, moveForward.z); //XとZ方向に移動度を適用させる

                    //プレイヤーキャラクターの方向を向く
                    this.transform.LookAt(playerTransform.position);
                    this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, this.transform.eulerAngles.z);

                    Debug.DrawRay(ray.origin, ray.direction * visionLength, Color.green, 0.1f, false);
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * visionLength, Color.blue, 0.1f, false);
                }

            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<Player>().TakeDamage(20);
        }
    }
}
