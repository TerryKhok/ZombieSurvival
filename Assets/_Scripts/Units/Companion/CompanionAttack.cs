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


    private Coroutine AttackCoroutine;

    private List<BulletTarget> _attackableObjects = new List<BulletTarget>();

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
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(_attackDelay);
        while (_attackableObjects.Count > 0)
        {
            yield return wait;

            BulletTarget closestAttackable = FindClosestAttackable();

            Transform bullet = Instantiate(_prefab, transform.position + new Vector3(0, 1f, 0), Quaternion.LookRotation(transform.forward, Vector3.up));

            StartCoroutine(MoveAttack(bullet, closestAttackable));
        }
    }

    private IEnumerator MoveAttack(Transform bullet, BulletTarget attackable)
    {
        Vector3 startPosition = bullet.transform.position;

        float distance = Vector3.Distance(bullet.transform.position, attackable.transform.position);
        float startingDistance = distance;

        while (distance > 0)
        {
            bullet.transform.position = Vector3.Lerp(startPosition, attackable.transform.position, 1);
            distance -= Time.deltaTime * _attackMoveSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(1f);

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
