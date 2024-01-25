using System.Security.Cryptography;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private LayerMask _layersEnemyCannotSpawnOn;
    [SerializeField] private float _spawnInterval;
    [SerializeField] private float _minSpawnInterval;
    [SerializeField] private GameObject[] _enemiesToSpawn;
    [SerializeField] private Collider _spawnArea;
    [SerializeField] private int _spawnLimit = 200;

    private int _spawnCount = 0;
    private float _startTime;

    private void Start()
    {
        _startTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Time.time - _startTime > _spawnInterval && _spawnCount <= _spawnLimit)
        {
            _startTime = Time.time;
            if (_spawnInterval > _minSpawnInterval)
            {
                _spawnInterval -= 0.1f;
            }
            SpawnEnemies(_spawnArea, _enemiesToSpawn);
        }
    }

    public void SpawnEnemies(Collider spawnableAreaCollider, GameObject[] enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnableAreaCollider);
            GameObject spawnedEnemy = Instantiate(enemy, spawnPosition, Quaternion.identity);
            if (spawnedEnemy)
                _spawnCount++;
        }
    }

    private Vector3 GetRandomSpawnPosition(Collider spawnableAreaCollider)
    {
        Vector3 spawnPosition = Vector2.zero;
        bool isSpawnPosValid = false;

        int attemptCount = 0;
        int maxAttempts = 200;

        while (!isSpawnPosValid && attemptCount < maxAttempts)
        {
            spawnPosition = GetRandomPointInCollider(spawnableAreaCollider);
            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 1.8f);

            bool isInvalidCollision = false;
            foreach (Collider collider in colliders)
            {
                if (((1 << collider.gameObject.layer) & _layersEnemyCannotSpawnOn) != 0) //bitwise opetarion //‚à‚µcollider‚ªlayer‚ÉŠÜ‚ñ‚¾‚çtrue‚ð–ß‚·
                { 
                    isInvalidCollision = true;
                    break;
                }

            }

            if (!isInvalidCollision)
            {
                isSpawnPosValid = true;
            }

            attemptCount++;
        }

        if (!isSpawnPosValid)
        {
            Debug.LogWarning("Could not find a valid spawn position");
        }


        return spawnPosition;
    }

    private Vector3 GetRandomPointInCollider(Collider collider, float offset = 1f)
    {
        Bounds collBounds = collider.bounds;

        Vector3 minBounds = new Vector3(collBounds.min.x + offset, collBounds.min.y, collBounds.min.z + offset);
        Vector3 maxBounds = new Vector3(collBounds.max.x - offset, collBounds.max.y, collBounds.max.z - offset);

        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        float randomZ = Random.Range(minBounds.z, maxBounds.z);

        return new Vector3(randomX, randomY, randomZ);
    }


    public void MinusSpawnCount()
    {
        if (_spawnCount! <= 0)
            _spawnCount--;
    }
}
