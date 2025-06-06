using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _WalkingSpeed;
    [SerializeField] private float _CrouchingSpeed;
    private float _speed;
    private Rigidbody _playerRb;
    private PlayerInput _playerInput;
    private CapsuleCollider _playerCollider;
    private Vector2 _input;
    private Vector3 _movementRelativeToCamera;

    private bool _onStealth = false;
    
    //Animation variables
    [SerializeField] private Animator _animator;
    private bool _isCrouching = false;
   

   public bool OnStealth 
    {
        get { return _onStealth; }
    }
   

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        _speed = _WalkingSpeed;
        
    }


    private void Update()
    {
        _input = _playerInput.actions["Move"].ReadValue<Vector2>();
        _movementRelativeToCamera = MoveRelativeToCamera(_input);
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        MovePlayer(_movementRelativeToCamera);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        bool isJump = _animator.GetBool("IsJump");
        if (context.performed)
            _animator.SetBool("IsJump", true);
        _playerRb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void MovePlayer(Vector3 input)
    {
        bool isWalking = _animator.GetBool("IsWalking");
        bool movePressed = _input.x != 0 || _input.y != 0;

        if (movePressed && !isWalking)
            _animator.SetBool("IsWalking", true);

        if ((!movePressed && isWalking))
            _animator.SetBool("IsWalking", false);
           

        
        

        _playerRb.MovePosition(transform.position + input * _speed * Time.deltaTime);
       
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

        if(callbackContext.performed)
        {
            _isCrouching = !_isCrouching;
            if(_isCrouching)
            {
                _playerCollider.height = crouchingHeight;
                _playerCollider.center = new Vector3(0, 0.45f, 0);
                _speed = _CrouchingSpeed;
                _animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = true;

            }
            else
            {
                _playerCollider.height = standingHeight;
                _playerCollider.center = new Vector3(0, 0.9f, 0);
                _speed = _WalkingSpeed;
                _animator.SetBool("IsCrouching", _isCrouching);
                _onStealth = false;
            }
        }
        
        
    }
}