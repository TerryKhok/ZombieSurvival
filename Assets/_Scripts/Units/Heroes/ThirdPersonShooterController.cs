using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ThirdPersonShooterController : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [Header("Currently selected as controlled chartacter")]
    [SerializeField] private bool _isControlled = false;

    [Header("Aim")]
    [SerializeField] private GameObject _playerGameobject; //プレイヤーのgameobject
    [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera; //エイムカメラコンポーネント
    [SerializeField] private float _normalSensitivity = 1.0f; //通常感度
    [SerializeField] private float _aimSensitivity = 0.5f; //エイム感度
    [SerializeField] private LayerMask _aimColliderLayerMask = new LayerMask(); //エイム感度

    [Header("Shoot")]
    [SerializeField] private Transform _prefabBulletProjectile; //弾のprefab
    [SerializeField] private Transform _spawnBulletPosition; //弾の生成点
    [SerializeField] private CrosshairController _crosshairController; //crosshairの処理
    [SerializeField] private GameObject _muzzleFlash; //MuzzleFlash
    [SerializeField] private Transform _UIMuzzleFlashPosition; //弾の生成点
    [SerializeField] private GameObject _UIMuzzleFlash; //MuzzleFlash
    [SerializeField] private Transform _UIGunObject;
    [SerializeField] private Text _bulletAmountText; //銃の弾数

    [Header("Grenade")]
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private float _throwForce = 40f;
    [SerializeField] private float _grenadeCooldown;

    [Header("Gun Stats")]
    [SerializeField] private int _damage;
    [SerializeField] private int _magazineSize;
    [SerializeField] private float _shotsPerSecond;
    [SerializeField] private bool _allowButtonHold;
    [SerializeField] private int _bulletsPerTap;
    [SerializeField] private float _timeBetweenShotsPerTap;
    [SerializeField] private float _reloadSpeed;
    [SerializeField] private float _settleSpeed = 2f;
    [SerializeField] private float _maxCrosshairSize = 2f;
    [SerializeField] private float _gunRecoil = 0.3f;
    [SerializeField] private float _spread;

    [Header("SFX")]
    [SerializeField] private string _gunShotSFX;
    [SerializeField] private string _gunCockSFX;
    private bool _isPlayingMinigunLoop = false;

    public bool IsDead = false;

    private ThirdPersonMovement _thirdPersonMovement; //移動処理情報
    private StarterAssetsInputs _starterAssetsInputs; //入力情報
    private Animator _animator;
    private AudioManager _audioManager;

    private Vector3 _targetPosition = Vector3.zero; //カーソル位置情報
    private Vector3 _aimDirection = Vector3.zero;

    private float _shootRate;
    private int _bulletsLeft;
    private int _bulletsShot;

    private bool _isShooting;
    private bool _readyToShoot;
    private bool _isReloading;
    private bool _isShootnRun;
    private bool _readyToGrenade;

    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        _thirdPersonMovement = GetComponentInParent<ThirdPersonMovement>(); //移動処理をゲット
        _starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>(); //入力コンポーネントをゲット
        _animator = GetComponentInParent<Animator>(); //アニメーションコンポーネントをゲット
        _audioManager = FindObjectOfType<AudioManager>();
    }


    private void Start()
    {
        //crosshairのステータスを設定する
        if (_crosshairController != null)
        {
            _crosshairController.SetReloadSpeed(_reloadSpeed);
            _crosshairController.SetShrinkSpeed(_settleSpeed);
            _crosshairController.SetMaxScale(_maxCrosshairSize);
        }

        //射的速度を計算
        _shootRate = 1.0f / _shotsPerSecond;

        _bulletsLeft = _magazineSize;
        _readyToShoot = true;
        _readyToGrenade = true;

    }


    private void Update()
    {
        if (_isControlled)
        {
            if (!IsDead)
            {
                Aim();
                ShootInput();
            }

            _bulletAmountText.text = _bulletsLeft + "/" + _magazineSize;
        }
    }


    //ーーーーーーーーーーPrivate関数ーーーーーーーーーー
    //エイム関数
    private void Aim()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f); //画面中心点を計算

        //Rayを使ってcrosshairに当たったgameobjectの情報を取る
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimColliderLayerMask))
        {
            _targetPosition = raycastHit.point;
        }

        //エイム押したら、カメラとカメラ感度を切り替える
        if (_starterAssetsInputs.aim)
        {
            _aimVirtualCamera.gameObject.SetActive(true);
            _thirdPersonMovement.SetSensitivity(_aimSensitivity);
            _thirdPersonMovement.SetRotateOnMove(false);
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            //カーソル位置情報からキャラの回転方向を決める
            Vector3 worldAimTarget = _targetPosition;
            worldAimTarget.y = _playerGameobject.transform.position.y;
            _aimDirection = (worldAimTarget - _playerGameobject.transform.position).normalized;

            _playerGameobject.transform.forward = Vector3.Lerp(_playerGameobject.transform.forward, _aimDirection, Time.deltaTime * 20f); //エイムしてる時、キャラをエイム方向に回転する
        }
        else if (!_isShootnRun)
        {
            _aimVirtualCamera.gameObject.SetActive(false);
            _thirdPersonMovement.SetSensitivity(_normalSensitivity);
            _thirdPersonMovement.SetRotateOnMove(true);
            _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }


    //弾撃つ関数
    private void Shoot()
    {
        _readyToShoot = false;

        //Spread
        float xSpread = Random.Range(-1f, 1f);
        float ySpread = Random.Range(-1f, 1f);
        float zSpread = Random.Range(-1f, 1f);
        float yOffset = (_isControlled) ? 0 : 1.5f;

        //弾の方向を計算
        Vector3 aimDir = (_targetPosition + new Vector3(0, yOffset, 0) - _spawnBulletPosition.position).normalized
                        + (new Vector3(xSpread, ySpread, zSpread).normalized * _spread);

        //弾を生成
        //-----UI-----
        if (_isControlled)
        {
            RotateToShootDir();
            Instantiate(_UIMuzzleFlash, _UIMuzzleFlashPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
            _UIGunObject.GetComponent<Vibration>().StartVibration();
        }

        Instantiate(_muzzleFlash, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Transform bullet = Instantiate(_prefabBulletProjectile, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        BulletProjectile bulletProjectile = bullet.GetComponent<BulletProjectile>();
        bulletProjectile.BulletDamage = _damage;
        _bulletsLeft--;
        _bulletsShot--;
        if (_isControlled)
        {
            _crosshairController.ExpandCrosshair(_gunRecoil);
        }

        if (_gunShotSFX == "MinigunShot")
        {
            if (!_isPlayingMinigunLoop)
            {
                _audioManager.Play(_gunShotSFX);
                _isPlayingMinigunLoop = true;
            }
        }
        else
        {
            _audioManager.Play(_gunShotSFX);
        }
        if (_gunCockSFX != "" && _gunCockSFX != null)
            Invoke("PlayShotgunCock", 0.3f);

        Invoke("ResetShot", _shootRate);

        if (_bulletsShot > 0 && _bulletsLeft > 0)
        {
            Invoke("Shoot", _timeBetweenShotsPerTap);
        }
    }


    private void ResetShot()
    {
        _readyToShoot = true;
    }


    private void Reload()
    {
        _isReloading = true;

        if (_isControlled) { _crosshairController.DoReload(); }
        Invoke("ReloadFinished", _reloadSpeed);
    }


    private void PlayShotgunCock()
    {
        _audioManager.Play(_gunCockSFX);
    }


    private void ReloadFinished()
    {
        _bulletsLeft = _magazineSize;
        _isReloading = false;
    }


    private void RotateToShootDir()
    {
        StartCoroutine(RotatingToShootDir());
    }


    private void ThrowGrenade()
    {
        _readyToGrenade = false;
        GameObject grenade = Instantiate(_grenadePrefab, _spawnBulletPosition.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(_aimDirection * _throwForce, ForceMode.Impulse);
        Invoke("ResetGrenadeCooldown", _grenadeCooldown);
    }


    private void ResetGrenadeCooldown()
    {
        _readyToGrenade = true;
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー


    //ーーーーーーーーーーCoroutine関数ーーーーーーーーーー
    IEnumerator RotatingToShootDir()
    {
        _thirdPersonMovement.SetRotateOnMove(false);
        float timePassed = 0;
        _isShootnRun = true;
        _playerGameobject.transform.forward = Vector3.Lerp(_playerGameobject.transform.forward,
            (_targetPosition - _spawnBulletPosition.position).normalized, Time.deltaTime * 40f); //エイムしてる時、キャラをエイム方向に回転する
        while (timePassed < 1.5)
        {
            timePassed += Time.deltaTime;

            yield return null;
        }
        _isShootnRun = false;
        _thirdPersonMovement.SetRotateOnMove(true);

    }
    //ーーーーーーーーーーendCoroutine関数ーーーーーーーーーー


    //ーーーーーーーーーーPublic関数ーーーーーーーーーー
    //弾撃つ操作管理関数
    public void ShootInput()
    {
        if (_isControlled)
        {
            if (_allowButtonHold)
            {
                _isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else
            {
                _isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < _magazineSize && !_isReloading)
            {
                _crosshairController.SetReloadSpeed(_reloadSpeed);
                Reload();
            }
            if (Input.GetKeyDown(KeyCode.G) && _readyToGrenade)
            {
                ThrowGrenade();
            }
        }

        if (_readyToShoot && _isShooting && !_isReloading)
        {
            if (_bulletsLeft > 0)
            {
                _bulletsShot = _bulletsPerTap;
                Shoot();
            }
            else
            {
                if (_isControlled) { _crosshairController.SetReloadSpeed(_reloadSpeed); }
                Reload();
            }
        }
        else
        {
            if (_gunShotSFX == "MinigunShot")
            {
                _isPlayingMinigunLoop = false;
                _audioManager.Stop("MinigunShot");
            }
        }
    }


    public void SetBulletSpawnPos(Transform newSpawnBulletPos)
    {
        _spawnBulletPosition = newSpawnBulletPos;
    }


    public void SetIsShooting(bool newIsShooting)
    {
        _isShooting = newIsShooting;
    }


    public void SetTargetPos(Vector3 newTargetPos)
    {
        _targetPosition = newTargetPos;
    }
    //ーーーーーーーーーーEndPublic関数ーーーーーーーーーー
}
