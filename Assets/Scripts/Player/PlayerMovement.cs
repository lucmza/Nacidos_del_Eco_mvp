using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _WalkingSpeed;
    [SerializeField] private float _CrouchingSpeed;
    private float _speed;
    private float _gravity = -9.8f;
    //private Rigidbody _playerRb;
    private PlayerInput _playerInput;
    private CapsuleCollider _playerCollider;
    private CharacterController _characterController;
    private Vector2 _input;
    private Vector3 _movementRelativeToCamera;
    private Vector3 _playerVelocity;

    private bool _onStealth = false;

    private bool _onGround;
    
    //Animation variables
    [SerializeField] private Animator _animator;
    private bool _isCrouching = false;
    private bool _IsWalking;
    private bool _movePressed;


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

    }


    private void Update()
    {
        _onGround = _characterController.isGrounded;
        _input = _playerInput.actions["Move"].ReadValue<Vector2>();
        _movementRelativeToCamera = MoveRelativeToCamera(_input);
        RotatePlayer();

        if(_onGround && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0;
        }

        _playerVelocity.y += _gravity * Time.deltaTime;

        //Player Animation
        if (!_isCrouching && _movePressed)
        {
            _animator.SetBool("IsWalking", true);
            _animator.SetBool("IsCrouching", false);

        }

        if (!_movePressed && !_isCrouching)
        {
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsCrouching", false);

        }

        if (_isCrouching && !_movePressed)
        {
            _animator.SetBool("IsCrouching", true);
            _animator.SetBool("IsCrouchWalking", false);

        }

        if (_isCrouching && _movePressed)
            _animator.SetBool("IsCrouchWalking", true);

    }

    private void FixedUpdate()
    {
        MovePlayer(_movementRelativeToCamera);
        
    }

    //Deshabilitado por el momento
    public void Jump(InputAction.CallbackContext context)
    {
        bool isJump = _animator.GetBool("IsJump");
        if (context.performed && _onGround)
        {
            _animator.SetBool("IsJump", true);
            _playerVelocity.y = Mathf.Sqrt(_jumpForce * -1f * _gravity);
        }
        
    }

    private void MovePlayer(Vector3 input)
    {
        
        _movePressed = _input.x != 0 || _input.y != 0;
        
        
        // Por que esta dos veces?

        Vector3 movement = new Vector3(input.x, _playerVelocity.y, input.z);
        _characterController.Move(movement * _speed * Time.deltaTime);
        
        //_characterController.Move(input * _speed * Time.deltaTime);
        
        //_characterController.Move(_playerVelocity * Time.deltaTime);
        //_playerRb.MovePosition(transform.position + input * _speed * Time.deltaTime);

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
                //_animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = true;
                Debug.Log(_onStealth);
            }
            else
            {
                _playerCollider.height = standingHeight;
                _playerCollider.center = new Vector3(0, 0.9f, 0);
                _speed = _WalkingSpeed;
                //_animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = false;
                Debug.Log(_onStealth);
            }
        }


    }
}