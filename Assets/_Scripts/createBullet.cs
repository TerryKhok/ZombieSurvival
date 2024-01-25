using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createBullet : MonoBehaviour
{
    [Header("カメラオブジェクト名")]
    public string cameraName = "Main Camera"; //カメラオブジェクト名
    GameObject cameraObj;

    [Header("発射する弾の情報")]
    public KeyCode shotKey = KeyCode.Mouse0; //発射キー
    public GameObject prefabObj; //弾のプレハブ
    public float shotpower = 15.0f; //弾の発射速度

    void Start()
    {
        //名前検索でScene中からカメラオブジェクトを見つける
        cameraObj = GameObject.Find(cameraName);
        if (cameraObj == null) //カメラオブジェクトが見つからなければエラー
        {
            Debug.LogError("カメラオブジェクトが見つかりません");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(shotKey)) //ショットキーが押されたら
        {
            //弾を出現させる地点
            Vector3 spawnVec = this.transform.position;
            spawnVec += this.transform.forward * 1.0f; //キャラクターの位置より少し前
            spawnVec += this.transform.up * 0.5f; //上方向に少しずらす

            //弾のオブジェクトを生成
            GameObject obj = Instantiate(prefabObj, spawnVec, Quaternion.identity); //vecの位置にprefabObjを出現させる

            //AddForceする向きを計算する
            Ray ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
            RaycastHit hit; //当たった結果を代入する変数

            //レイを飛ばしてオブジェクトに当たれば
            Vector3 v;
            if (Physics.Raycast(ray, out hit, 100.0f)) {
                v = (hit.point - spawnVec).normalized;
            } else {
                v = cameraObj.transform.forward; //カメラの前方へむかってAddForceする
            }
            
            obj.GetComponent<Rigidbody>().AddForce(v * shotpower, ForceMode.Impulse);
        }
    }
}
