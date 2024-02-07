using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [SerializeField] private float _lookRadius = 10f;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange = .5f;
    [SerializeField] private float _attackSpeed = 5f;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private string _attackSFX;
    [SerializeField] private float _offsetDirection;

    public bool IsDead;

    //キャシュー
    private Transform _target;
    private NavMeshAgent _agent;
    private Animator _animator;
    private BoxCollider _col;
    private AudioManager _audioManager;
    private AudioSource _audioSrc;
    private BulletTarget _bulletTarget;

    //animation IDs
    private int _animIDWalk;
    private int _animIDAttack;
    private int _animIDDeath;

    private float _startTime;
    private float _deathTime = 10.0f;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    void Start()
    {
        _target = PlayerManager.instance.player.transform;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _col = GetComponentInChildren<BoxCollider>();
        _audioManager = FindObjectOfType<AudioManager>();
        _audioSrc = GetComponent<AudioSource>();
        _bulletTarget = GetComponent<BulletTarget>();

        AssignAnimationIDs();

        _startTime = Time.time;
    }


    void Update()
    {
        float distance = Vector3.Distance(_target.position, transform.position);

        //目標が自身の範囲に入るのを計算
        if (distance <= _agent.stoppingDistance && !IsDead) //攻撃範囲に入る
        {
            FaceTarget();
            if (Time.time - _startTime >= _attackSpeed)
            {
                _startTime = Time.time;
                _animator.SetTrigger(_animIDAttack);
            }
        }
        else if (distance <= _lookRadius && !IsDead) //探知範囲に入る
        {
            _agent.SetDestination(_target.position);
            _animator.SetBool(_animIDWalk, true);
        }
        else if (!IsDead) //範囲外
        {
            _animator.SetBool(_animIDWalk, false);
        }

        if (IsDead)
        {
            _animator.SetTrigger(_animIDDeath);
            _agent.enabled = false;
            _col.enabled = false;
            _audioSrc.Stop();
            Destroy(_bulletTarget);

            _deathTime -= Time.deltaTime;
            if (_deathTime < 5.0f)
            {
                transform.position -= new Vector3(0, .1f * Time.deltaTime, 0);
                if (_deathTime < 0.0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    private void FaceTarget()
    {
        Vector3 direction = (_target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x + _offsetDirection, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    private void AssignAnimationIDs()
    {
        _animIDWalk = Animator.StringToHash("Walk");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDDeath = Animator.StringToHash("Death");
    }
    //ーーーーーーーーーーEndPrivate関数ーーーーーーーーーー


    //ーーーーーーーーーーPublic関数ーーーーーーーーーー
    public void Attack()
    {
        Collider[] colInfo = Physics.OverlapSphere(_attackPoint.position, _attackRange, _attackMask);
        if (colInfo != null)
        {
            foreach (Collider col in colInfo)
            {
                Debug.Log("Dealt 10 DMG to Player");
                col.GetComponent<PlayerStats>().TakeDamage(10);
            }
        }
    }

    public void PlayAttackSFX(){
        _audioManager.Play(_attackSFX);
    }
    //ーーーーーーーーーーEndPublic関数ーーーーーーーーーー


    //視線半径を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }
}
