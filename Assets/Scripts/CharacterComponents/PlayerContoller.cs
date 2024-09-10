using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerContoller : MonoBehaviour, ICharacter2D
{

    private Rigidbody2D _rb;
    private Animator _animator;
    private Collider2D[] _collider;

    [SerializeField] Transform _scrollPoint;
    [SerializeField] GameObject _dieUI;
    [SerializeField] GameObject _EndScreenUI;

    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _normalJumpHeight = 5;
    [SerializeField] private float _superJumpHeight = 5;
    [SerializeField] private float _sprintFactor = 2;


    private Vector2 _movment;
    private float _horizontalInput;
    private float _sprintSpeed;
    private float _horizontalVelocity;
    private float _currentSpeed;
    private float _jumpHeight;

    private bool _isRunning;
    private bool _jump;
    public bool _isGrounded;
    public bool _isDead;

    private Vector3 _moveRightFlipSprite;
    private Vector3 _moveLeftFlipSprite;
    private Vector3 _currentScale;




   //public EventManager eventManager;
    protected void Awake()
    {
       // _rb = transform.GetComponent<Rigidbody2D>();
      //  _animator = transform.root.GetComponent<Animator>();
       // _collider = transform.GetComponentsInChildren<Collider2D>();

      //  _currentScale = transform.localScale;
    }


    void Start()
    {
        _sprintSpeed = _moveSpeed * _sprintFactor;
        _jumpHeight = _normalJumpHeight;

        _moveRightFlipSprite = new Vector3(1 * _currentScale.x, 1 * _currentScale.y, 1 * _currentScale.z);
        _moveLeftFlipSprite = new Vector3(-1 * _currentScale.x, 1 * _currentScale.y, 1 * _currentScale.z);

      
    }

    void Update()
    {
        if (!_isDead)
        {

            //flip sprite when direction is changed
            if (_horizontalInput > 0.1)
            {
                transform.localScale = _moveRightFlipSprite;
            }
            else if (_horizontalInput < -0.1f)
            {
                transform.localScale = _moveLeftFlipSprite;
            }

            //change speed when sprinting
            if (_isRunning && _horizontalInput != 0)
            {
                _currentSpeed = _sprintSpeed;
                _jumpHeight = _superJumpHeight;

            }
            else
            {
                _currentSpeed = _moveSpeed;
                _jumpHeight = _normalJumpHeight;
            }

            //jumping
            if (_jump && _isGrounded)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpHeight);
                _isGrounded = false;
            }

            //set andimator conditions
          //  _animator.SetBool("isGrounded", _isGrounded);
          //  _animator.SetFloat("VelocityY", _rb.velocity.y);
          //  _animator.SetFloat("VelocityX", Mathf.Abs(_horizontalInput * _currentSpeed));
         //   _animator.SetBool("isRunning", _isRunning);
        }
   
    }
    

    private void FixedUpdate()
    {
        transform.Translate(new Vector2(_horizontalVelocity * _currentSpeed * Time.deltaTime, 0));
    }

    private void OnEnable()
    {
        //event when player is hit by enemy
     //   player.playerHitEventChannel.OnPlayerHit += HitByEnemy;
    //    player.playerEventManager.OnPlayerDie += Die;
    }

    private void OnDisable()
    {
     //   player.playerHitEventChannel.OnPlayerHit += HitByEnemy;
      //  player.playerEventManager.OnPlayerDie -= Die;

    }
    
    //switches movment to free movment and scrolling movment
    public void Move(Vector2 moveInput)
    {
        _movment = moveInput;
        _horizontalInput = _movment.x;
      //  if(this.transform.position.x > _scrollPoint.transform.position.x && _horizontalInput > 0.5) 
      //  {

        
          //  player.playerEventManager.OnScrollPointHit?.Invoke(_horizontalInput * _currentSpeed);
          //  _scrollEventChannel.OnBackgroundScroll?.Invoke(_currentSpeed);

            _horizontalVelocity = 0;
      //  }
       // else
     //   {
            _horizontalVelocity = _horizontalInput;
          //  player.playerEventManager.OnScrollPointOff?.Invoke();
          //  _scrollEventChannel?.OnBackgroundScrollOff?.Invoke();
      //  }

    }


    public void JumpButtonDown()
    {
       _jump = true;
        _animator.SetTrigger("Jump");
    }

    public void JumpButtonUp() 
    { 
        _jump = false;
    }


    public void SprintOn()
    {
        _isRunning = true;
        _animator.SetFloat("AnimationSpeed", _sprintFactor);
    }

    public void SprintOff() 
    { 
        _isRunning = false;
        _animator.SetFloat("AnimationSpeed", .75f);

    }

    public void HitByEnemy(int damage)
    {
        if (!_isDead)
        {
        //    player.playerEventManager.OnCharacterHit?.Invoke(damage);
         //   _scoreEventChannel.NotHitTimerBonus?.Invoke();
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpHeight);
            _animator.SetTrigger("KnockBack");
        }

    }

    private void Die()
    {

        if (!_isDead)
        {
            _isDead = true;

            _animator.SetBool("isDead", _isDead);

           // _rb.gravityScale = 0f;
            _currentSpeed = 0f;

            _rb.velocity = new Vector2(_horizontalVelocity * _currentSpeed, -5);

            _dieUI.SetActive(true);

        }
        
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            _isGrounded = true;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            _isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isGrounded = false;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            _isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            _isGrounded = true;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            _isGrounded = true;
            JumpButtonDown();

        }
        else
        {
            JumpButtonUp();
        }

        if (collision.gameObject.tag == "PitTrigger")
        {
            Die();
        }

        if(collision.gameObject.tag == "Flag")
        {
    
            _EndScreenUI.SetActive(true);
         //   _scoreEventChannel.OnLevelEnd?.Invoke();
            _rb.isKinematic = true;
            _rb.velocity = Vector3.zero;
            _rb.gravityScale = 0;
        }

    }


}
