using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSupplyCrate : MonoBehaviour
{
    [SerializeField] private int _healAmount;
    [SerializeField] private int _scoreAmount;

    private ScoreSystem _scoreSystem;
    private PlayerStats _playerStats;
    [SerializeField]private EnemySpawnManager _spawnManager;


    private void Awake()
    {
        _playerStats = GetComponentInParent<PlayerStats>();
        _scoreSystem = FindFirstObjectByType<ScoreSystem>();
        _spawnManager = GameObject.FindGameObjectWithTag("SupplyManager").GetComponent<EnemySpawnManager>();
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "SupplyCrate(Clone)")
        {
            Destroy(hit.gameObject);
            _playerStats.TakeDamage(-_healAmount);
            _scoreSystem.AddScore(_scoreAmount);
            _spawnManager.MinusSpawnCount();
        }
    }
}



