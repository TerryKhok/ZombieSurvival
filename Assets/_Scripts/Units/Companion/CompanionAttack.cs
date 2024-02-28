using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [SerializeField] Transform _companionGameobject;
    [SerializeField] Transform _spawnBulletPosition; //弾を生成する位置
    [SerializeField] ThirdPersonShooterController _shooterController;

    [SerializeField] private List<BulletTarget> _attackableObjects = new List<BulletTarget>(); //範囲内敵のリスト
    [SerializeField] private float _enemiesVoiceCooldown = 15f;

    private static bool _enemiesVoicePlayable = false;

    public bool IsShooting = false;

    private Coroutine AttackCoroutine;
    private bool _isRunning = false;
    private Vector3 _shootDir;

    private AudioManager _audioManager;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _companionGameobject = GetComponentInParent<Companion>().transform;
        _shooterController = GetComponent<ThirdPersonShooterController>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        Invoke("DelayVoice", 10f);
    }

    private void FixedUpdate()
    {
        if (_attackableObjects.Count == 0)
        {
            IsShooting = false;
        }
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
            PlayEnemiesVoice();
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


    private void PlayEnemiesVoice()
    {
        if (_enemiesVoicePlayable)
        {
            _enemiesVoicePlayable = false;
            int character = Random.Range(0, 2);
            switch (character)
            {
                case 0:
                    _audioManager.Play("Catalina_Enemies");
                    break;
                case 1:
                    _audioManager.Play("Stone_Enemies");
                    break;
                case 2:
                    _audioManager.Play("Viktor_Enemies");
                    break;
            }
            StartCoroutine(ResetCooldown());
        }
    }
    //ーーーーーーーーーーCoroutine関数ーーーーーーーーーー
    //攻撃Coroutine
    private IEnumerator Attack()
    {
        while (_attackableObjects.Count > 0)
        {
            for (int i = 0; i < _attackableObjects.Count; i++)
            {
                if (_attackableObjects[i] == null)
                {
                    _attackableObjects.Remove(_attackableObjects[i]);
                }
            }
            if (!_isRunning)
            {
                IsShooting = true;
                BulletTarget closestAttackable = FindClosestAttackable(); //一番近い敵をGetする

                _shootDir = (closestAttackable.transform.position - _companionGameobject.position).normalized;
                _companionGameobject.forward = Vector3.Lerp(_companionGameobject.forward, _shootDir, Time.deltaTime * 20f);

                _shooterController.SetBulletSpawnPos(_spawnBulletPosition); //弾生成位置をSet
                _shooterController.SetIsShooting(true); //isShooting状態をSet
                _shooterController.SetTargetPos(closestAttackable.transform.position + new Vector3(0f, _spawnBulletPosition.position.y, 0f)); //Target位置をSet
                _shooterController.ShootInput(); //攻撃Inputをシミュレーションする
            }

            yield return null;
        }
    }


    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(_enemiesVoiceCooldown);
        _enemiesVoicePlayable = true;
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


    private void DelayVoice()
    {
        _enemiesVoicePlayable = true;
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー


    //ーーーーーーーーーーPublic関数ーーーーーーーーーー
    public void SetIsRunning(bool newState)
    {
        _isRunning = newState;
    }
    //ーーーーーーーーーーendPublic関数ーーーーーーーーーー
}
