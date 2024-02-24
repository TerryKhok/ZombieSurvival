
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]

public class ThirdPersonMovement : MonoBehaviour
{
    //�[�[�[�[�[�[�[�[�[�[�ϐ��錾�[�[�[�[�[�[�[�[�[�[
    [Header("Camera")]
    [SerializeField] private GameObject _cameraTarget; //�J�����ǐՖڕW�̒��S�_
    [SerializeField] private float _topClamp = 70f; //�J�������
    [SerializeField] private float _bottomClamp = -30f; //�J��������
    [SerializeField] private float _cameraAngleOverride = 0.0f; //�㉺�����㏑������
    [SerializeField] private bool _lockCameraPosition = false; //�J���������b�N
    [SerializeField] private float _cameraSensitivity = 1f; //�J����sensitivity

    [Header("Player")]
    [SerializeField] private float _walkSpeed = 8f; //�������x
    [SerializeField] private float _runSpeed = 20f; //���鑬�x
    [Space(10)]
    [SerializeField] private float _rotationSmoothTime = 0.1f; //��]���x
    [SerializeField] private float _speedChargeRate = 10f; //��������
    [Space(10)]
    [SerializeField] private float _jumpHeight = 1.2f; //�W�����v����
    [SerializeField] private float _gravity = -15f; //����
    [Space(10)]
    [SerializeField] private float _jumpCooldown = 0.05f; //�W�����vCD
    [SerializeField] private float _fallOffsetTime = 0.15f; //Fall��ԓ���ׂɂ��鎞��

    [Header("isGroundCheck")] //�n�ʌ���
    [SerializeField] private bool _isGround = true;
    [SerializeField] private float _isGroundOffset = -0.14f; //�����덷
    [SerializeField] private float _isGroundCheckRadius = 0.28f; //�n�ʌ������a(Character Controller���a�Ɠ���)
    [SerializeField] private LayerMask _groundLayers; //�������C���[

    [Header("SFX")]
    [SerializeField] AudioClip _landingAudioClip;
    [SerializeField] AudioClip[] _footstepAudioClip;
    [SerializeField] private float _footstepAudioVolume = 0.5f; //��������

    [Header("Death")]
    [SerializeField] private float _deathTime = 10.0f;
    [SerializeField] private Transform _vfxDeath;
    [SerializeField] private Image _deathScreen;

    public bool IsDead = false;

    //Cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    //Player
    private float _speed; //���x
    private float _animationBlend; //�A�j���[�V����
    private float _targetRotation = 0.0f; //��]�ڕW
    private float _rotationSmoothVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53f; //�ō����x

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

    //�L���V���[
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
    //�[�[�[�[�[�[�[�[�[�[end�ϐ��錾�[�[�[�[�[�[�[�[�[�[


    private void Awake()
    {
        if (_mainCamera == null) //�J�����������
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }


    private void Start()
    {
        //�e�R���|�[�l���g�������
        _cinemachineTargetYaw = _cameraTarget.transform.rotation.eulerAngles.y; //�J�������E�p�x���v���C���[�ƈꏏ�ɂ���
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
        _manageScene = GetComponent<ManageScene>();
        _sceneTransition = FindAnyObjectByType<SceneTransition>();

        _audioManager = FindObjectOfType<AudioManager>();
        AssignAnimationIDs();

        Cursor.visible = false;  //�J�[�\����\��
        Cursor.lockState = CursorLockMode.Locked;

        _fallTimeoutDelta = _fallOffsetTime; //�덷�����Z�b�g
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
                    Cursor.visible = true;  //�J�[�\���\��
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


    //�[�[�[�[�[�[�[�[�[�[private�֐��[�[�[�[�[�[�[�[�[�[
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGround = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDDeath = Animator.StringToHash("Death");
    }


    //�n�ʌ����֐�
    private void IsGroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - _isGroundOffset, transform.position.z);
        _isGround = Physics.CheckSphere(spherePosition, _isGroundCheckRadius, //���̃G���A�Œn�ʂ���������
            _groundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGround, _isGround);
        }
    }


    //�J��������֐�
    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold && !_lockCameraPosition)
        { //���͗L�����`�F�b�N�����J�������b�N���ĂȂ�
            float deltaTimeMultiplier = 1.0f;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * _cameraSensitivity;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * _cameraSensitivity;
        }

        //�J������]�𐧌�
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

        //�J�����ǐՖڕW�̕������J�����ƈꏏ�ɂ���
        _cameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch +
            _cameraAngleOverride, _cinemachineTargetYaw, 0.0f);
    }


    //�ړ��֐�
    private void Movement()
    {
        float targetSpeed = _input.sprint ? _runSpeed : _walkSpeed; //�����E�����ԂŖڕW���x�����߂�

        if (_input.move == Vector2.zero) //���͂Ȃ����A���x�O
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; //���̑��x�����
        float speedOffset = 0.1f; //�덷

        //�ڕW���x�܂ŏ��X�ɉ�������
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * _speedChargeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f; //���x�������_3�ʂ܂Ŏl�̌ܓ�
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChargeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0f, _input.move.y).normalized; //�ړ����������߂�

        //���͂��鎞�A�L��������]����
        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                ref _rotationSmoothVelocity, _rotationSmoothTime);

            if (_rotateOnMove)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f); //�L�������ړ������ɉ�]����
            }
        }

        //�L�������ړ�����
        Vector3 targetDir = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDir.normalized * (_speed * Time.deltaTime) + //���E�ړ����x
            new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime); //�㉺�ړ����x

        //�A�j���[�V����
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1f);
        }
    }


    //�W�����v�ƈ��͏����֐�
    private void JumpAndGravity()
    {
        if (_isGround)
        {
            _fallTimeoutDelta = _fallOffsetTime; //������

            //�A�j���[�V����
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            //�W�����v����
            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity); //jumpHeight��B������ׂ̑��x

                //�A�j���[�V����
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            //�W�����vCD����
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else //�����鏈��
        {
            _jumpTimeoutDelta = _jumpCooldown; //������

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // //�A�j���[�V����
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFall, true);
                }
            }

            _input.jump = false; //�n�ʂɂ��Ȃ��ƃW�����v�o���Ȃ�
        }

        //���͏���
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


    //�n�ʌ����G���A��`��
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_isGround) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _isGroundOffset,
        transform.position.z), _isGroundCheckRadius);
    }


    //��������
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


    // ���n������
    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(_landingAudioClip,
                transform.TransformPoint(_controller.center), _footstepAudioVolume);
        }
    }
    //�[�[�[�[�[�[�[�[�[�[endPrivate�֐��[�[�[�[�[�[�[�[�[�[


    //�[�[�[�[�[�[�[�[�[�[Public�֐��[�[�[�[�[�[�[�[�[�[
    //�J����sensitivity��ύX����֐�
    public void SetSensitivity(float newSensitivity)
    {
        _cameraSensitivity = newSensitivity;
    }

    //�ړ����鎞�L��������]���邩�ǂ����֐�
    public void SetRotateOnMove(bool newRotateOnMove)
    {
        _rotateOnMove = newRotateOnMove;
    }

    public void PlayDeathVFX()
    {
        Instantiate(_vfxDeath, transform.position, Quaternion.identity);
        _audioManager.Play("Explosion");
    }
    //�[�[�[�[�[�[�[�[�[�[endPublic�֐��[�[�[�[�[�[�[�[�[�[
}
