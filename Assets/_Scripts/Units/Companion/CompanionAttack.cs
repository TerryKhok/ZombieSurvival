using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [SerializeField] Transform _spawnBulletPosition; //弾を生成する位置
    [SerializeField] ThirdPersonShooterController _shooterController;

    [SerializeField] private List<BulletTarget> _attackableObjects = new List<BulletTarget>(); //範囲内敵のリスト

    private Coroutine AttackCoroutine;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _shooterController = GetComponent<ThirdPersonShooterController>();
    }


    //敵が範囲内に入る時の処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BulletTarget>(out BulletTarget attackable))
        {
            _attackableObjects.Add(attackable); //範囲内敵リストに追加する
            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
            }
            AttackCoroutine = StartCoroutine(Attack()); //攻撃処理Coroutine
        }
    }


    //敵が範囲内から出る時の処理
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BulletTarget>(out BulletTarget attackable))
        {
            _attackableObjects.Remove(attackable); //範囲内敵リストから削除する
            if (_attackableObjects.Count == 0) //リストに敵ない時攻撃停止
            {
                StopCoroutine(AttackCoroutine);
                _shooterController.SetIsShooting(false);
            }
        }
    }


    //ーーーーーーーーーーCoroutine関数ーーーーーーーーーー
    //攻撃Coroutine
    private IEnumerator Attack()
    {
        while (_attackableObjects.Count > 0)
        {
            BulletTarget closestAttackable = FindClosestAttackable(); //一番近い敵をGetする

            _shooterController.SetBulletSpawnPos(_spawnBulletPosition); //弾生成位置をSet
            _shooterController.SetIsShooting(true); //isShooting状態をSet
            _shooterController.SetTargetPos(closestAttackable.transform.position + new Vector3(0f, _spawnBulletPosition.position.y, 0f)); //Target位置をSet
            _shooterController.ShootInput(); //攻撃Inputをシミュレーションする

            yield return null;
        }
    }
    //ーーーーーーーーーーendCoroutine関数ーーーーーーーーーー


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    //一番近い敵を計算する関数
    private BulletTarget FindClosestAttackable()
    {
        float closestDistance = float.MaxValue;
        int clostestIndex = 0;
        for (int i = 0; i < _attackableObjects.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, _attackableObjects[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                clostestIndex = i;
            }
        }
        return _attackableObjects[clostestIndex];
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー
}
