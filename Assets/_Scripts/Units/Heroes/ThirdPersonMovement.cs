
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class ThirdPersonMovement : MonoBehaviour
{
    //ーーーーーーーーーー変数宣言ーーーーーーーーーー
    [Header("Camera")]
    [SerializeField] private GameObject _cameraTarget; //カメラ追跡目標の中心点
    [SerializeField] private float _topClamp = 70f; //カメラ上限
    [SerializeField] private float _bottomClamp = -30f; //カメラ下限
    [SerializeField] private float _cameraAngleOverride = 0.0f; //上下限を上書きする
    [SerializeField] private bool _lockCameraPosition = false; //カメラをロック
    [SerializeField] private float _cameraSensitivity = 1f; //カメラsensitivity

    [Header("Player")]
    [SerializeField] private float _walkSpeed = 8f; //歩く速度
    [SerializeField] private float _runSpeed = 20f; //走る速度
    [Space(10)]
    [SerializeField] private float _rotationSmoothTime = 0.1f; //回転速度
    [SerializeField] private float _speedChargeRate = 10f; //加速減速
    [Space(10)]
    [SerializeField] private float _jumpHeight = 1.2f; //ジャンプ高さ
    [SerializeField] private float _gravity = -15f; //引力
    [Space(10)]
    [SerializeField] private float _jumpCooldown = 0.05f; //ジャンプCD
    [SerializeField] private float _fallOffsetTime = 0.15f; //Fall状態入る為にいる時間

    [Header("isGroundCheck")] //地面検索
    [SerializeField] private bool _isGround = true;
    [SerializeField] private float _isGroundOffset = -0.14f; //検索誤差
    [SerializeField] private float _isGroundCheckRadius = 0.28f; //地面検索半径(Character Controller半径と同じ)
    [SerializeField] private LayerMask _groundLayers; //検索レイヤー

    [Header("SFX")]
    [SerializeField] AudioClip _landingAudioClip;
    [SerializeField] AudioClip[] _footstepAudioClip;
    [SerializeField] private float _footstepAudioVolume = 0.5f; //足音音量

    [Header("Death")]
    [SerializeField] private float _deathTime = 10.0f;
    [SerializeField] private Transform _vfxDeath;
    [SerializeField] private Image _deathScreen;

    public bool IsDead = false;

    //Cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    //Player
    private float _speed; //速度
    private float _animationBlend; //アニメーション
    private float _targetRotation = 0.0f; //回転目標
    private float _rotationSmoothVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53f; //最高速度

    //Timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    //animation IDs
    private int _animIDSpeed;
    private int _animIDGround;
    private int _animIDJump;
    private int _animIDFall;
    private int _animIDMotionSpeed;
    private int _animIDDeath;

    //Player Input
    private PlayerInput _playerInput;

    //Death
    private float _deathScreenAlpha = 0f;

    //キャシュー
    private Animator _animator;
    private CharacterController _controller;
    private StarterAssetsInputs _input;
    private GameObject _mainCamera;
    private bool _rotateOnMove = true;
    private ManageScene _manageScene;
    private SceneTransition _sceneTransition;
    [SerializeField] private AudioManager _audioManager;

    private const float _threshold = 0.00f;

    private bool _hasAnimator;
    //ーーーーーーーーーーend変数宣言ーーーーーーーーーー


    private void Awake()
    {
        if (_mainCamera == null) //カメラ情報を取る
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }


    private void Start()
    {
        //各コンポーネント情報を取る
        _cinemachineTargetYaw = _cameraTarget.transform.rotation.eulerAngles.y; //カメラ左右角度をプレイヤーと一緒にする
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
        _manageScene = GetComponent<ManageScene>();
        _sceneTransition = FindAnyObjectByType<SceneTransition>();

        _audioManager = FindObjectOfType<AudioManager>();
        AssignAnimationIDs();

        Cursor.visible = false;  //カーソル非表示
        Cursor.lockState = CursorLockMode.Locked;

        _fallTimeoutDelta = _fallOffsetTime; //誤差をリセット
        _jumpTimeoutDelta = _jumpCooldown;
    }


    private void Update()
    {
        _hasAnimator = TryGetComponent(out _animator);
        if (!IsDead)
        {
            JumpAndGravity();
            Movement();
            IsGroundCheck();
        }
        else
        {
            _animator.SetTrigger(_animIDDeath);

            _deathTime -= Time.deltaTime;
            if (_deathTime < 5.0f)
            {
                transform.position -= new Vector3(0, .1f * Time.deltaTime, 0);
                _deathScreenAlpha = _deathScreenAlpha + 0.05f * Time.deltaTime;
                _deathScreen.color = new Color(0.8f, 0.15f, 0.15f, _deathScreenAlpha);
                if (_deathTime < 0.0f)
                {
                    Cursor.visible = true;  //カーソル表示
                    Cursor.lockState = CursorLockMode.None;
                    _sceneTransition.StartSceneTransitionResult();
                }
            }
        }
    }


    private void LateUpdate()
    {
        CameraRotation();
    }


    //ーーーーーーーーーーprivate関数ーーーーーーーーーー
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGround = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDDeath = Animator.StringToHash("Death");
    }


    //地面検索関数
    private void IsGroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - _isGroundOffset, transform.position.z);
        _isGround = Physics.CheckSphere(spherePosition, _isGroundCheckRadius, //球体エリアで地面を検索する
            _groundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGround, _isGround);
        }
    }


    //カメラ操作関数
    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold && !_lockCameraPosition)
        { //入力有無をチェック＆＆カメラロックしてない
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * _cameraSensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * _cameraSensitivity;
        }

        //カメラ回転を制限
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

        //カメラ追跡目標の方向をカメラと一緒にする
        _cameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch +
            _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }


    //移動関数
    private void Movement()
    {
        float targetSpeed = _input.sprint ? _runSpeed : _walkSpeed; //歩く・走る状態で目標速度を決める

        if (_input.move == Vector2.zero) //入力ない時、速度０
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; //今の速度を取る
        float speedOffset = 0.1f; //誤差

        //目標速度まで徐々に加速減速
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * _speedChargeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f; //速度を小数点3位まで四捨五入
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChargeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0f, _input.move.y).normalized; //移動方向を決める

        //入力ある時、キャラを回転する
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                ref _rotationSmoothVelocity, _rotationSmoothTime);

            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f); //キャラを移動方向に回転する
            }
        }

        //キャラを移動する
        Vector3 targetDir = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDir.normalized * (_speed * Time.deltaTime) + //左右移動速度
            new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime); //上下移動速度

        //アニメーション
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1f);
        }
    }


    //ジャンプと引力処理関数
    private void JumpAndGravity()
    {
        if (_isGround)
        {
            _fallTimeoutDelta = _fallOffsetTime; //初期化

            //アニメーション
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            //ジャンプ処理
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity); //jumpHeightを達成する為の速度

                //アニメーション
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            //ジャンプCD処理
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else //落ちる処理
        {
            _jumpTimeoutDelta = _jumpCooldown; //初期化

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // //アニメーション
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFall, true);
                }
            }

            _input.jump = false; //地面にいないとジャンプ出来ない
        }

        //引力処理
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }


    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360) lfAngle += 360f;
        if (lfAngle > 360) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }


    //地面検索エリアを描く
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _isGroundOffset,
        transform.position.z), _isGroundCheckRadius);
    }


    //足音処理
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (_footstepAudioClip.Length > 0)
            {
                var index = Random.Range(0, _footstepAudioClip.Length);
                AudioSource.PlayClipAtPoint(_footstepAudioClip[index],
                    transform.TransformPoint(_controller.center), _footstepAudioVolume);
            }
        }
    }


    // 着地音処理
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(_landingAudioClip,
                transform.TransformPoint(_controller.center), _footstepAudioVolume);
        }
    }
    //ーーーーーーーーーーendPrivate関数ーーーーーーーーーー


    //ーーーーーーーーーーPublic関数ーーーーーーーーーー
    //カメラsensitivityを変更する関数
    public void SetSensitivity(float newSensitivity)
    {
        _cameraSensitivity = newSensitivity;
    }

    //移動する時キャラを回転するかどうか関数
    public void SetRotateOnMove(bool newRotateOnMove)
    {
        _rotateOnMove = newRotateOnMove;
    }

    public void PlayDeathVFX()
    {
        Instantiate(_vfxDeath, transform.position, Quaternion.identity);
        _audioManager.Play("Explosion");
    }
    //ーーーーーーーーーーendPublic関数ーーーーーーーーーー
}
