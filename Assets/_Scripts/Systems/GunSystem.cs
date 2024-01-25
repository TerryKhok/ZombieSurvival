using UnityEngine;

public class GunSystem : MonoBehaviour
{
    //銃のステータス
    [SerializeField] private int _damage;
    [SerializeField] private float _timeBetweenShooting;
    [SerializeField] private float _spread;
    [SerializeField] private float _range;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _megazineSize;
    [SerializeField] private int _bulletsPerTap;
    [SerializeField] private bool _allowButtonHold;

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private RaycastHit _rayHit;
    [SerializeField] private LayerMask _whatIsEnemy;

    private int _bulletsLeft;
    private int _bulletsShot;

    private bool _isShooting;
    private bool _readyToShoot;
    private bool _isReloading;
}
