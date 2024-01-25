using System.Collections;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [SerializeField] ScoreSystem _scoreSystem;
    [SerializeField] int _score = 10;
    [SerializeField] EnemySpawnManager _enemySpawnManager;

    EnemyController _enemyController;

    private void Start()
    {
        _enemyController = GetComponent<EnemyController>();
        _scoreSystem = FindObjectOfType<ScoreSystem>();
        _enemySpawnManager = FindObjectOfType<EnemySpawnManager>();
    }

    public override void Die()
    {
        GetComponent<LootManager>().InstantiateLoot(transform.position);
        base.Die();
        _enemyController.IsDead = true;
        _scoreSystem.AddScore(_score);
        _enemySpawnManager.MinusSpawnCount();
    }
}
