using System.Collections;
using System.Data.Common;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.U2D;

[RequireComponent(typeof(NavMeshAgent))]

public class CompanionController : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [Header("Follow Configs")]
    [SerializeField] private Transform _followDestination; //移動目的地
    [SerializeField] private float _walkRadius; //歩く範囲
    [SerializeField] private float _runRadius; //走る範囲
    [SerializeField] private float _walkSpeed; //歩く速度
    [SerializeField] private float _runSpeed; //走る速度

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _dustEffect;
    [SerializeField] private bool _isBGMPlayer;

    [Header("Cache")]
    [SerializeField] private Player _player; //プレイヤーObject情報

    //Cache
    private NavMeshAgent _agent;
    private CompanionAttack _companionAttack;
    private Rigidbody _rigidbody;
    private AudioManager _audioManager;

    private float _distanceBetweenPlayer; //プレイヤーとの距離
    private bool _isGrounded;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _companionAttack = GetComponentInChildren<CompanionAttack>();
        _rigidbody = GetComponent<Rigidbody>();
        _audioManager = FindObjectOfType<AudioManager>();
    }


    private void Start()
    {
        _agent.enabled = false;
        _isGrounded = false;
    }


    private void Update()
    {
        if (!_isGrounded)
        {
            Collider[] colInfo = Physics.OverlapSphere(transform.position, .05f, _groundLayer);
            foreach (Collider col in colInfo)
            {
                if (col.CompareTag("Road"))
                    Debug.Log(col);
                Instantiate(_dustEffect, transform.position, _dustEffect.transform.rotation);
                _agent.enabled = true;
                _isGrounded = true;
                if (_isBGMPlayer)
                {
                    _audioManager.Play("BGM");
                }
                Destroy(_rigidbody);
                return;
            }
        }

        _distanceBetweenPlayer = Vector3.Distance(_player.transform.position, transform.position); //プレイヤーとの距離を計算
        if (_distanceBetweenPlayer > _runRadius) //走る範囲入ったらプレイヤーの位置まで走る
        {
            RunToPlayer();
            _companionAttack.SetIsRunning(true);
        }
        else if (_runRadius > _distanceBetweenPlayer && _distanceBetweenPlayer > _walkRadius) //歩く範囲入ったら銃を撃ちながら歩く
        {
            WalkAndGunToPlayer();
            _companionAttack.SetIsRunning(false);
        }
        else if (!_companionAttack.IsShooting!)//Idle状態の時プレイヤーから外に向く
        {
            IdleAndFaceOutwards();
        }
    }


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    //プレイやーの位置に走る関数
    private void RunToPlayer()
    {
        FollowPlayer(_followDestination.position, _runSpeed);
    }


    //プレイヤーの位置に歩く関数
    private void WalkAndGunToPlayer()
    {
        FollowPlayer(_followDestination.position, _walkSpeed);
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
                transform.rotation = Quaternion.Slerp(transform.rotation, outwardDir, 0.005f);
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
