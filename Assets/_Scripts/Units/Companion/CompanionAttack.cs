using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField] private Transform _prefab;
    [SerializeField][Range(0.1f, 1f)] private float _attackDelay = 0.33f;
    [SerializeField] private float _attackMoveSpeed = 3;
    [SerializeField] private Companion _companion;
    [SerializeField] ThirdPersonShooterController _shooterController;
    [SerializeField] Transform _spawnBulletPosition;


    private Coroutine AttackCoroutine;

    [SerializeField] private List<BulletTarget> _attackableObjects = new List<BulletTarget>();

    private void Awake()
    {
        _shooterController = GetComponent<ThirdPersonShooterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BulletTarget>(out BulletTarget attackable))
        {
            _attackableObjects.Add(attackable);
            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
            }
            AttackCoroutine = StartCoroutine(Attack());
            Debug.Log("Companion tried to attack");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BulletTarget>(out BulletTarget attackable))
        {
            _attackableObjects.Remove(attackable);
            if (_attackableObjects.Count == 0)
            {
                StopCoroutine(AttackCoroutine);
            _shooterController.SetIsShooting(false);

            }
        }
    }

    private IEnumerator Attack()
    {
        while (_attackableObjects.Count > 0)
        {
            BulletTarget closestAttackable = FindClosestAttackable();

            _shooterController.SetBulletSpawnPos(_spawnBulletPosition);
            _shooterController.SetIsShooting(true);
            _shooterController.SetTargetPos(closestAttackable.transform.position + new Vector3(0f, _spawnBulletPosition.position.y, 0f));
            _shooterController.ShootInput();

            yield return null;
        }
    }

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
}
