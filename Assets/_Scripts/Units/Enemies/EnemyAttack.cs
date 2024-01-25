using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    EnemyController _enemyController;
    void Start()
    {
        _enemyController = GetComponentInParent<EnemyController>();
    }

    public void Attack()
    {
        _enemyController.Attack();
    }

    public void PlaySFX()
    {
        _enemyController.PlayAttackSFX();
    }
}
