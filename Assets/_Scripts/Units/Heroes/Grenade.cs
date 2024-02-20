using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _delay = 3f;
    [SerializeField] private float _radius = 15f;
    [SerializeField] private float _force = 700f;
    [SerializeField] private int _damage = 100;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private LayerMask _layermask;

    private float _countdown;
    private bool _hasExploded = false;

    private void Start()
    {
        _countdown = _delay;
    }

    private void Update()
    {
        _countdown -= Time.deltaTime;

        if (_countdown <= 0f && !_hasExploded)
        {
            _hasExploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(_explosionVFX, transform.position, _explosionVFX.transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius,_layermask);
        foreach (Collider col in colliders)
        {
                if(col.transform.name=="Player"){
                    col.GetComponent<PlayerStats>().TakeDamage(_damage);
                }
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.GetComponent<EnemyStats>().TakeDamage(_damage);
                rb.AddExplosionForce(_force, transform.position, _radius);
            }
        }

        Destroy(gameObject);
    }
}
