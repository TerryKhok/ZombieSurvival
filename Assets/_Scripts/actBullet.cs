using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actBullet : MonoBehaviour
{
    float startTime; //発射時刻
    void Start()
    {
        startTime = Time.time; //発射された時刻を覚えておく
    }

    void Update()
    {
        if(startTime + 3.0f < Time.time) //発射から3秒経ったら
        {
            Destroy(this.gameObject); //自身を削除
        }
    }

    //何かにぶつかったら
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag == "Enemy"){
            Destroy(col.gameObject);
        }
        Destroy(this.gameObject); //自身を削除
    }
}
