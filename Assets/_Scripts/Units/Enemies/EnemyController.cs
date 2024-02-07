using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //�[�[�[�[�[�[�[�[�[�[�ϐ��錾�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private float _lookRadius = 10f;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _attackRange = .5f;
    [SerializeField] private float _attackSpeed = 5f;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private string _attackSFX;
    [SerializeField] private float _offsetDirection;

    public bool IsDead;

    //�L���V���[
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
    //�[�[�[�[�[�[�[�[�[�[end�ϐ��錾�[�[�[�[�[�[�[�[�[�[


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

        //�ڕW�����g�͈̔͂ɓ���̂��v�Z
        if (distance <= _agent.stoppingDistance && !IsDead) //�U���͈͂ɓ���
        {
            FaceTarget();
            if (Time.time - _startTime >= _attackSpeed)
            {
                _startTime = Time.time;
                _animator.SetTrigger(_animIDAttack);
            }
        }
        else if (distance <= _lookRadius && !IsDead) //�T�m�͈͂ɓ���
        {
            _agent.SetDestination(_target.position);
            _animator.SetBool(_animIDWalk, true);
        }
        else if (!IsDead) //�͈͊O
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


    //�[�[�[�[�[�[�[�[�[�[Private�֐��[�[�[�[�[�[�[�[�[�[
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
    //�[�[�[�[�[�[�[�[�[�[EndPrivate�֐��[�[�[�[�[�[�[�[�[�[


    //�[�[�[�[�[�[�[�[�[�[Public�֐��[�[�[�[�[�[�[�[�[�[
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
    //�[�[�[�[�[�[�[�[�[�[EndPublic�֐��[�[�[�[�[�[�[�[�[�[


    //�������a������
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }
}
