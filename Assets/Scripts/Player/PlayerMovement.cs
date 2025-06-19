using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _WalkingSpeed;
    [SerializeField] private float _CrouchingSpeed;
    private float _speed;
    [SerializeField] private float _gravity = -9.8f;
    //private Rigidbody _playerRb;
    private PlayerInput _playerInput;
    private CapsuleCollider _playerCollider;
    private CharacterController _characterController;
    private Vector2 _input;
    private Vector3 _movementRelativeToCamera;
    private float _horizontalSpeed;
    private Vector3 _playerVelocity;

    private bool _onStealth = false;

    private bool _onGround;

    //Animation variables
    [SerializeField] private Animator _animator;
    private bool _isCrouching = false;
    private bool _IsWalking;
    private bool _movePressed;


    //Audio
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip stepGrass;
    [SerializeField] private AudioClip stepConcrete;
    private bool _wasOnGroundLastFrame = false;
    [SerializeField] private AudioClip landSound;
    private bool _wasGroundedLastFrame = false;
    private bool _wasFallingLastFrame = false;


    [SerializeField] private float stealthStepMultiplier = 1.3f;
    private float baseStepInterval;
    [SerializeField] private float stepInterval = 0.4f;
    private float stepTimer;


    public bool OnStealth
    {
        get { return _onStealth; }
    }


    private void Awake()
    {
        //_playerRb = GetComponent<Rigidbody>();
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
        bool groundedNow = IsActuallyOnGround();

        bool wasFalling = _wasFallingLastFrame;

        _onGround = groundedNow;

        // Detectar aterrizaje solo si estaba cayendo antes
        if (groundedNow && !_wasGroundedLastFrame && wasFalling)
        {
            PlayLandSound();
        }

        // Guardar estados
        _wasGroundedLastFrame = groundedNow;
        _wasFallingLastFrame = _playerVelocity.y < -0.1f;

        _input = _playerInput.actions["Move"].ReadValue<Vector2>();
        _movementRelativeToCamera = MoveRelativeToCamera(_input);
        RotatePlayer();

        if (_onGround && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0;
        }

        _playerVelocity.y += _gravity * Time.deltaTime;

        // Animation
        _animator.SetFloat("IsWalking", _horizontalSpeed);

        // Pasos
        bool isMoving = _horizontalSpeed > 0.1f && _onGround;
        if (isMoving)
        {
            stepInterval = _onStealth ? baseStepInterval * stealthStepMultiplier : baseStepInterval;

            stepTimer += Time.deltaTime;
            if (stepTimer == 0f) PlayFootstep();
            if (stepTimer >= stepInterval)
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
        MovePlayer(_movementRelativeToCamera);

    }

    //Deshabilitado por el momento
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && _onGround)
        {
            _animator.SetTrigger("Jump");
            _playerVelocity.y = Mathf.Sqrt(_jumpForce * -1f * _gravity);
        }

    }

    private void MovePlayer(Vector3 input)
    {
        _movePressed = _input.x != 0 || _input.y != 0;

        Vector3 horizontalInput = new Vector3(input.x, 0, input.z);
        _horizontalSpeed = horizontalInput.magnitude;

        Vector3 movement = new Vector3(input.x, _playerVelocity.y, input.z);
        _characterController.Move(movement * _speed * Time.deltaTime);
    }



    private void RotatePlayer()
    {
        float rotationSpeed = 5;
        Vector3 posToLookAt;
        posToLookAt.x = _movementRelativeToCamera.x;
        posToLookAt.y = 0;
        posToLookAt.z = _movementRelativeToCamera.z;
        posToLookAt = posToLookAt.normalized;


        Quaternion targetRotation = posToLookAt == Vector3.zero ? transform.rotation : Quaternion.LookRotation(posToLookAt);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private Vector3 MoveRelativeToCamera(Vector2 input)
    {
        Vector3 camF = Camera.main.transform.forward;
        Vector3 camR = Camera.main.transform.right;
        camF.y = camR.y = 0;
        camF.Normalize(); camR.Normalize();
        return input.y * camF + input.x * camR;
    }

    public void Crouch(InputAction.CallbackContext callbackContext)
    {
        float standingHeight = 1.8f;
        float crouchingHeight = 0.9f;

        if (callbackContext.performed)
        {

            _isCrouching = !_isCrouching;
            if (_isCrouching)
            {
                _playerCollider.height = crouchingHeight;
                _playerCollider.center = new Vector3(0, 0.45f, 0);
                _speed = _CrouchingSpeed;
                _animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = true;
                Debug.Log(_onStealth);
            }
            else
            {
                _playerCollider.height = standingHeight;
                _playerCollider.center = new Vector3(0, 0.9f, 0);
                _speed = _WalkingSpeed;
                _animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = false;
                Debug.Log(_onStealth);
            }
        }
    }

    private bool IsActuallyOnGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float rayLength = 0.4f;

        return Physics.Raycast(origin, Vector3.down, rayLength);
    }

    private void PlayFootstep()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
        {
            string tag = hit.collider.tag;
            AudioClip clipToPlay = stepGrass;

            if (tag == "Concrete") clipToPlay = stepConcrete;
            else if (tag == "Grass") clipToPlay = stepGrass;

            if (footstepAudioSource && clipToPlay)
            {
                footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);

                footstepAudioSource.PlayOneShot(clipToPlay);
            }

        }
    }

    private void PlayLandSound()
    {
        if (footstepAudioSource && landSound)
        {
            footstepAudioSource.pitch = Random.Range(0.95f, 1.05f);
            footstepAudioSource.PlayOneShot(landSound);
        }
    }

}