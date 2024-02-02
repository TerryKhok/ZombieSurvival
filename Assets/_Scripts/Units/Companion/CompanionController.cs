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
    [Header("Idle Configs")]
    [SerializeField][Range(0f, 10f)] private float _rotationSpeed = 2f;

    [Header("Follow Configs")]
    [SerializeField] private Transform _followDestination;
    [SerializeField] private float _walkRadius = 0f;
    [SerializeField] private float _runRadius = 2f;
    

    [Header("Cache")]
    [SerializeField] private Player _player;

    private NavMeshAgent _agent;
    private CompanionAttack _companionAttack;

    [SerializeField]private float _distanceBetweenPlayer;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _companionAttack = GetComponentInChildren<CompanionAttack>();
    }


    private void Update()
    {
        _distanceBetweenPlayer = Vector3.Distance(_player.transform.position, transform.position);
        if (_distanceBetweenPlayer > _runRadius)
        {
            RunToPlayer();
        }
        else if (_runRadius > _distanceBetweenPlayer && _distanceBetweenPlayer > _walkRadius)
        {
            WalkAndGunToPlayer();
        }
        else
        {
            IdleAndFaceOutwards();
        }
    }


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    private void RunToPlayer()
    {
        FollowPlayer(_followDestination.position, 10f);
    }


    private void WalkAndGunToPlayer()
    {
        FollowPlayer(_followDestination.position, 3f);
    }


    private void IdleAndFaceOutwards()
    {
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                Quaternion outwardDir = Quaternion.LookRotation(_player.transform.position - transform.position) * Quaternion.Euler(0, 180, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, outwardDir, 0.01f);

            }
        }
    }


    private void FollowPlayer(Vector3 followDestination, float followSpeed)
    {
        _agent.SetDestination(followDestination);
        _agent.speed = followSpeed;
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー

}
