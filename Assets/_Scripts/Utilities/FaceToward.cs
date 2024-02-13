using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class FaceToward : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _turnSpeed;

    void Update()
    {
        if (_target != null)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_target.transform.position - transform.position), _turnSpeed);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
