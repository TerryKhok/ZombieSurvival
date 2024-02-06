using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.U2D;

[RequireComponent(typeof(NavMeshAgent))]

public class CompanionController : MonoBehaviour
{

    // if(idle){
    //      faceOutward()
    // }
    // if(enemyInRange){
    //     Aim()
    //     Shoot()
    //     if(playerFarAway){
    //         stopShootingAndDashToPlayer()
    //     }
    //     else if(playerOutOfRange){
    //         moveWhileShooting()
    //     }
    // }


    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [Header("Follow Configs")]
    [SerializeField] private Transform _followDestination; //移動目的地
    [SerializeField] private float _walkRadius; //歩く範囲
    [SerializeField] private float _runRadius; //走る範囲

    [Header("Cache")]
    [SerializeField] private Player _player; //プレイヤーObject情報

    //Cache
    private NavMeshAgent _agent;
    private CompanionAttack _companionAttack;

    private float _distanceBetweenPlayer; //プレイヤーとの距離
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _companionAttack = GetComponentInChildren<CompanionAttack>();
    }


    private void Update()
    {
        _distanceBetweenPlayer = Vector3.Distance(_player.transform.position, transform.position); //プレイヤーとの距離を計算
        if (_distanceBetweenPlayer > _runRadius) //走る範囲入ったらプレイヤーの位置まで走る
        {
            RunToPlayer();
        }
        else if (_runRadius > _distanceBetweenPlayer && _distanceBetweenPlayer > _walkRadius) //歩く範囲入ったら銃を撃ちながら歩く
        {
            WalkAndGunToPlayer();
        }
        else //Idle状態の時プレイヤーから外に向く
        {
            IdleAndFaceOutwards();
        }
    }


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    //プレイやーの位置に走る関数
    private void RunToPlayer()
    {
        FollowPlayer(_followDestination.position, 10f);
    }


    //プレイヤーの位置に歩く関数
    private void WalkAndGunToPlayer()
    {
        FollowPlayer(_followDestination.position, 3f);
    }


    //Idle状態時プレイヤーから外に向く関数  
    private void IdleAndFaceOutwards()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance) //目的地に着いた
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) //新し目的地がない、動いてない
            {
                //プレイヤーを向いてる時の反対向き
                Quaternion outwardDir = Quaternion.LookRotation(_player.transform.position - transform.position) * Quaternion.Euler(0, 180, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, outwardDir, 0.01f);
            }
        }
    }


    //プレイヤーの位置に行く関数
    private void FollowPlayer(Vector3 followDestination, float followSpeed)
    {
        _agent.SetDestination(followDestination);
        _agent.speed = followSpeed;
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー

}
