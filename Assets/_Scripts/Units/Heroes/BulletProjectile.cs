using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private int _bulletPierceCount = 0;
    [SerializeField] private Transform _vfxHit;

    public int BulletDamage = 20;

    private Rigidbody _bulletRb;
    private AudioManager _audioManager;

    private void Awake()
    {
        _bulletRb = GetComponent<Rigidbody>();
        _audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        _bulletRb.velocity = transform.forward * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletTarget>() != null)
        {
            other.GetComponent<EnemyStats>().TakeDamage(BulletDamage);
            _audioManager.Play("ZombieHurt");
        }

        Instantiate(_vfxHit, transform.position, Quaternion.identity);
        if (_bulletPierceCount == 0)
        {
            transform.Find("Trail").SetParent(null);
            Destroy(gameObject);
        }
        _bulletPierceCount--;
    }
}
