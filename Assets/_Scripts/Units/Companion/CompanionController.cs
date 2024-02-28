using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]

public class CompanionController : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [Header("Follow Configs")]
    [SerializeField] private Transform _followDestination; //移動目的地
    [SerializeField] private float _walkRadius; //歩く範囲
    [SerializeField] private float _runRadius; //走る範囲
    [SerializeField] private float _dashRadius; //ダッシュ範囲
    [SerializeField] private float _walkSpeed; //歩く速度
    [SerializeField] private float _runSpeed; //走る速度
    [SerializeField] private float _dashSpeed; //ダッシュ速度

    [Header("Ground Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _dustEffect;
    [SerializeField] private bool _isBGMPlayer;

    [Header("VoiceControl")]
    [SerializeField] private float _idleVoiceCooldown = 15f;

    [Header("Cache")]
    [SerializeField] private Player _player; //プレイヤーObject情報

    //Cache
    private NavMeshAgent _agent;
    private CompanionAttack _companionAttack;
    private Rigidbody _rigidbody;
    private AudioManager _audioManager;

    private float _distanceBetweenPlayer; //プレイヤーとの距離
    private bool _isGrounded;
    private static bool _idleVoicePlayable = false;
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
                    Invoke("PlayIntroVoice", 1f);
                    Invoke("DelayVoice", 10f);
                }
                Destroy(_rigidbody);
                return;
            }
        }

        _distanceBetweenPlayer = Vector3.Distance(_player.transform.position, transform.position); //プレイヤーとの距離を計算
        if (_distanceBetweenPlayer > _dashRadius) //ダッシュ範囲入ったらプレイヤーの位置までダッシュ
        {
            DashToPlayer();
            _companionAttack.SetIsRunning(true);
        }
        else if (_distanceBetweenPlayer > _runRadius) //走る範囲入ったらプレイヤーの位置まで走る
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
    //プレイやーの位置にダッシュ関数
    private void DashToPlayer()
    {
        FollowPlayer(_followDestination.position, _dashSpeed);
    }


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
                PlayIdleVoice();
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


    private void PlayIntroVoice()
    {
        _audioManager.Play("Colt_Intro");
    }

    private void PlayIdleVoice()
    {
        if (_idleVoicePlayable)
        {
            _idleVoicePlayable = false;
            int character = Random.Range(0, 2);
            switch (character)
            {
                case 0:
                    _audioManager.Play("Catalina_Idle");
                    break;
                case 1:
                    _audioManager.Play("Stone_Idle");
                    break;
                case 2:
                    _audioManager.Play("Viktor_Idle");
                    break;
            }
            StartCoroutine(ResetCooldown());
        }
    }


    private void DelayVoice()
    {
        _idleVoicePlayable = true;
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー


    //ーーーーーーーーーーCoroutine関数ーーーーーーーーーー
    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(_idleVoiceCooldown);
        _idleVoicePlayable = true;
    }
    //ーーーーーーーーーーendCoroutine関数ーーーーーーーーーー

}
