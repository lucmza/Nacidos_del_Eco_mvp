using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _WalkingSpeed;
    [SerializeField] private float _CrouchingSpeed;
    [SerializeField] private float _gravity = -9.8f;

    private float _speed;
    private PlayerInput _playerInput;
    private CapsuleCollider _playerCollider;
    private CharacterController _characterController;
    private Vector2 _input;
    private Vector3 _movementRelativeToCamera;
    private float _horizontalSpeed;
    private Vector3 _playerVelocity;

    private bool _onGround;
    private bool _isCrouching;
    private bool _onStealth;

    [SerializeField] private Animator _animator;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip stepGrass;
    [SerializeField] private AudioClip stepConcrete;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private float stealthStepMultiplier = 1.3f;
    private float baseStepInterval;
    [SerializeField] private float stepInterval = 0.4f;
    private float stepTimer;
    private float _fallDuration = 0f;
    [SerializeField] private float _minFallTimeToPlayLandSound = 0.15f;


    public bool OnStealth => _onStealth;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerCollider = GetComponent<CapsuleCollider>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _speed = _WalkingSpeed;
        baseStepInterval = stepInterval;
    }

    private void Update()
    {
        bool wasGroundedLastFrame = _onGround;
        _onGround = _characterController.isGrounded;

        if (!_onGround)
        {
            if (_playerVelocity.y < -0.1f)
                _fallDuration += Time.deltaTime;
        }
        else if (!wasGroundedLastFrame && _onGround)
        {
            if (_fallDuration >= _minFallTimeToPlayLandSound)
            {
                PlayLandSound();
            }
            _fallDuration = 0f;
        }



        _input = _playerInput.actions["Move"].ReadValue<Vector2>();
        _movementRelativeToCamera = MoveRelativeToCamera(_input);
        RotatePlayer();

        if (_onGround && _playerVelocity.y < 0f)
            _playerVelocity.y = 0f;
        _playerVelocity.y += _gravity * Time.deltaTime;

        Vector3 horizontalVel = new Vector3(_movementRelativeToCamera.x, 0, _movementRelativeToCamera.z);
        _horizontalSpeed = horizontalVel.magnitude;
        _animator.SetFloat("IsWalking", _horizontalSpeed);

        bool isMoving = _horizontalSpeed > 0.1f && _onGround;
        if (isMoving)
        {
            float interval = _onStealth
                ? baseStepInterval * stealthStepMultiplier
                : baseStepInterval;
            stepTimer += Time.deltaTime;
            if (stepTimer >= interval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }


    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(
            _movementRelativeToCamera.x,
            _playerVelocity.y,
            _movementRelativeToCamera.z
        );
        _characterController.Move(movement * _speed * Time.deltaTime);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _onGround)
        {
            _animator.SetTrigger("Jump");
            _playerVelocity.y = Mathf.Sqrt(_jumpForce * -1f * _gravity);
        }
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        _isCrouching = !_isCrouching;
        float standingHeight = 1.8f;
        float crouchHeight = 0.9f;

        if (_isCrouching)
        {
            _playerCollider.height = crouchHeight;
            _playerCollider.center = new Vector3(0, crouchHeight / 2f, 0);
            _speed = _CrouchingSpeed;
            _onStealth = true;
        }
        else
        {
            _playerCollider.height = standingHeight;
            _playerCollider.center = new Vector3(0, standingHeight / 2f, 0);
            _speed = _WalkingSpeed;
            _onStealth = false;
        }

        _animator.SetBool("IsCrouching", _isCrouching);
    }

    private void RotatePlayer()
    {
        Vector3 dir = _movementRelativeToCamera;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * Time.deltaTime);
    }

    private Vector3 MoveRelativeToCamera(Vector2 input)
    {
        Vector3 camF = Camera.main.transform.forward;
        Vector3 camR = Camera.main.transform.right;
        camF.y = 0; camR.y = 0;
        Vector3 move = input.y * camF + input.x * camR;
        return move.normalized;
    }

    private void PlayFootstep()
    {
        if (!footstepAudioSource) return;
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            AudioClip clip = hit.collider.CompareTag("Concrete")
                ? stepConcrete
                : stepGrass;

            if (clip != null)
            {
                footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
                footstepAudioSource.PlayOneShot(clip);
            }
        }
    }

    private void PlayLandSound()
    {
        if (!footstepAudioSource || landSound == null) return;
        footstepAudioSource.pitch = Random.Range(0.95f, 1.05f);
        footstepAudioSource.PlayOneShot(landSound);
    }
}
