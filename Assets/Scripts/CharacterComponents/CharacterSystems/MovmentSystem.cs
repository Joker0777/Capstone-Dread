using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovmentSystem : CharacterSystems, IMovable
{
    [SerializeField] protected float _speed = 1f;
    [SerializeField] protected float _runSpeedMultiplier = 2f;
    [SerializeField] protected float _crouchSpeedMultiplier = 0.9f;
    [SerializeField] protected float _walkBackwardsMultiplier = 0.7f;

    [SerializeField] protected float _jumpForce = 5f;
    [SerializeField] protected float _jumpAnticipationTime;

    [SerializeField] protected Animator _animator;
    [SerializeField] protected GroundCheck groundCheck;
    [SerializeField] protected CharcterAim charcterAim;

    private bool _isRunning;
    private bool _isCrouching;
    private bool _isGrounded;
    private bool _isJumping;
    private bool _isFacingRight;

    private float _horizontal;
    private Rigidbody2D _rigidbody2D;

    private float _lastGroundSpeed;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    protected override void Start()
    {
        base.Start();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        charcterAim = GetComponent<CharcterAim>();

        if (groundCheck != null)
        {
            _isGrounded = groundCheck.IsGrounded;
        }
    }

    private void OnEnable()
    {
        _eventManager.IsFacingRight += SetDirection;
    }

    private void OnDisable()
    {
        _eventManager.IsFacingRight -= SetDirection;
    }

    private void SetDirection(bool isFacingRight)
    {
        _isFacingRight = isFacingRight;
    }

    private void Update()
    {
        GroundCheck();
        UpdateAnimation();
    }

    protected virtual void FixedUpdate()
    {
        float adjustedSpeed;

        if (_isGrounded)
        {
            adjustedSpeed = GetCharacterSpeed();
            _lastGroundSpeed = adjustedSpeed; // Save ground speed before jumping
        }
        else
        {
            adjustedSpeed = _lastGroundSpeed; // Preserve last known ground speed in the air
        }

        // Only apply horizontal velocity when grounded or when there's player input while in the air
        if (_isGrounded || Mathf.Abs(_horizontal) > 0.01f)
        {
            _rigidbody2D.velocity = new Vector2(_horizontal * adjustedSpeed, _rigidbody2D.velocity.y);
        }
    }

    private void GroundCheck()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = groundCheck.IsGrounded;


        if (!wasGrounded && !_isGrounded && !_isJumping)
        {
            StartFalling();
        }

        if (_isGrounded)
        {
            _animator.SetBool("isFalling", false);
        }
    }
    private void StartFalling()
    {
        _animator.SetBool("isFalling", true);
        _isJumping = false; 
    }

    private void UpdateAnimation()
    {
        _animator.SetBool("isWalking", IsCharacterWalking());
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetFloat("VelocityY", _rigidbody2D.velocity.y);
        _animator.SetFloat("VelocityX", Mathf.Abs(_horizontal * GetCharacterSpeed()));
        _animator.SetBool("isCrouching", _isCrouching && !IsWalkingBackWards());
        _animator.SetBool("isRunning", IsCharacterRunning());
        _animator.SetBool("isGrounded", _isGrounded);
        _animator.SetBool("isWalkingBackwards", IsWalkingBackWards());
        _animator.SetFloat("PlayerInput", Mathf.Abs(_horizontal));
      //  _animator.SetBool("isFalling", !_isGrounded && !_isJumping);


    }

    private bool IsCharacterRunning()
    {
        if (_isRunning && _isGrounded)
        {
            if ((_isFacingRight && _horizontal > 0) || (!charcterAim.IsFacingRight && _horizontal < 0))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWalkingBackWards()
    {
        if ((_isFacingRight && _horizontal < 0) || (!charcterAim.IsFacingRight && _horizontal > 0))
        {
            return true;
        }
        return false;
    }

    private bool IsCharacterWalking()
    {
        if (_isGrounded && !_isRunning && _isCrouching)
        {
            if ((_isFacingRight && _horizontal > 0) || (!charcterAim.IsFacingRight && _horizontal < 0))
            {
                return true;
            }
        }
        return false;
    }

    private float GetCharacterSpeed()
    {
        if (_isRunning && _isGrounded)
        {
            if ((_isFacingRight && _horizontal > 0) || (!charcterAim.IsFacingRight && _horizontal < 0))
            {
                return _speed * _runSpeedMultiplier;
            }
        }
        else if (_isCrouching)
        {
            return _speed * _crouchSpeedMultiplier;
        }
        else if (IsWalkingBackWards())
        {
            return _speed * _walkBackwardsMultiplier;
        }
        else if (IsWalkingBackWards() && _isCrouching)
        {
            return 0f;
        }else if(_isRunning && !_isGrounded)
        {
            return _speed * _runSpeedMultiplier;
        }

        return _speed;
    }

    public virtual void Move(Vector2 input)
    {
        _horizontal = input.x;
    }

    public void SetRunning(bool isRunning)
    {
        _isRunning = isRunning;
    }

    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching && _isGrounded && !_isRunning;
    }

    public void Jump()
    {
        if (_isGrounded && !_isJumping && !_isCrouching)
        {
            _lastGroundSpeed = _horizontal * _speed; // Store horizontal velocity when jumping
            StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        _isJumping = true;

        yield return new WaitForSeconds(_jumpAnticipationTime);

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        _isGrounded = false;
        _isJumping = false;
    }
}
