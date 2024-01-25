using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cameraコンポーネントが必須
[RequireComponent(typeof(Camera))]

public class moveCamera : MonoBehaviour
{
    [Header("キャラクターオブジェクトの名前")]
    public string charaName = "Player"; //キャラクターのオブジェクト名

    [Header("キャラクターとカメラの距離")]
    public float kyori = 3.0f; //キャラクターとカメラの距離

    [Header("マウス設定")]
    public float kando = 3.0f; //マウス操作の感度
    public bool rebirthFg = false; //上下方向の操作反転フラグ

    [Header("カメラの注視点設定")]
    public Vector2 adjustCamera = new Vector2(0.7f, 1.0f); //カメラの位置調整

    GameObject charaObj; //キャラクターオブジェクト
    Transform cameraTrans; //カメラの位置
    Vector3 targetPos; //注視点の位置

    float mouseInputX;
    float mouseInputY;

    void Start()
    {
        //名前検索でScene中からプレイヤーオブジェクトを見つける
        charaObj = GameObject.Find(charaName);
        if (charaObj == null) //プレイヤーオブジェクトが見つからなければエラー
        {
            Debug.LogError("プレイヤーオブジェクトが見つかりません");
            return;
        }

        //自身のtransformを取得しておく
        cameraTrans = this.transform;

        //カメラの注視点を決める（キャラクターの少し右上）
        targetPos = ExportTargetPos(charaObj); //キャラクターの座標を取得

        //カメラの位置を決める（キャラクタの後ろ方向へkyoriだけ移動させた地点）
        Vector3 k = charaObj.transform.forward; //キャラクターの正面方向のベクトルを取得
        k = k * -1; //-1を掛けてキャラクターの真後ろ方向のベクトルにする
        k = k.normalized * kyori;//ベクトルの長さをkyoriにする
        cameraTrans.position = targetPos + k; //カメラの位置を決定する

        //カメラを注視点へ向ける
        cameraTrans.LookAt(targetPos);
    }

    void Update()
    {
        //キャラクターが移動していたら
        Vector3 tpos = ExportTargetPos(charaObj);
        if (tpos != targetPos)
        {
            //移動差を取得
            Vector3 sa = targetPos - tpos;

            //カメラの位置も同じだけ動かす
            cameraTrans.position -= sa;

            //カメラの注視点を更新
            targetPos = tpos;
        }

        //マウス入力を取得
        mouseInputX = Input.GetAxis("Mouse X"); //X方向
        mouseInputY = Input.GetAxis("Mouse Y"); //Y方向

        //X方向にカメラを移動させる
        cameraTrans.RotateAround(targetPos, Vector3.up, mouseInputX * kando );

        //Y方向にカメラを移動させる
        Vector3 oldPos = cameraTrans.position;
        Quaternion oldRot = cameraTrans.rotation;
        if (rebirthFg == true) { mouseInputY *= -1; } //もしrebirthFgが立っていれば反転させる
        cameraTrans.RotateAround(targetPos, cameraTrans.right, mouseInputY * kando);
        float camAngle = Mathf.Abs(Vector3.Angle(Vector3.up, targetPos - cameraTrans.position)); //カメラの角度を求める
        if (camAngle < 45 || camAngle > 135) //カメラの角度が一定範囲外なら動かさない
        {
            cameraTrans.position = oldPos;
            cameraTrans.rotation = oldRot;
        }

        //カメラがZ軸方向に回転しないようにする
        cameraTrans.eulerAngles = new Vector3(cameraTrans.eulerAngles.x, cameraTrans.eulerAngles.y, 0.0f);
    }


    //カメラの注視点を取得する
    Vector3 ExportTargetPos(GameObject obj)
    {
        Vector3 res = obj.transform.position; //プレイヤーの位置

        res += obj.transform.right * adjustCamera.x; //注視点調整（X方向）
        res += obj.transform.up * adjustCamera.y; //注視点調整（Y方向）

        return res; //計算結果を返す
    }

}
