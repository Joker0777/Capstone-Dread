using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemyBehaviourSystem : CharacterSystems
{
    private Rigidbody2D rb;
    public GameObject target;
    private Vector2 moveDirection;
    private Collider2D collider;



    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;  // Jump force for the enemy
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float stopRange = 5f;
    [SerializeField] private float backoffRange = 2f;
    [SerializeField] private float obstacleCheckDistance = 1.0f;  // Distance to detect obstacles
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform frontCheck;  // Position to check for obstacles
    [SerializeField] private WeaponSystem weapon;
    [SerializeField] private Animator[] animators;
    [SerializeField] private string _detectSoundEffect;

    public bool isGrounded;
    private bool _isDead;

    public enum EnemyState { Patrol, DetectTarget, AttackTarget, Backoff, IsDead, IsHurt }
    public EnemyState currentState = EnemyState.Patrol;
    public EnemyState previousState = EnemyState.Patrol;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();


        if (weapon == null)
        {
            weapon = GetComponentInChildren<WeaponSystem>();
            if (weapon == null)
            {
                Debug.LogWarning("Weapon system not assigned or found in children.");
            }
        }
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
        }

        GroundCheck();
        HandleStates();
        UpdateAnimations();
    }

    private void OnEnable()
    {
        _eventManager.OnCharacterDestroyed += SetDeathState;
        _eventManager.OnCharacterHurt += SetHurtState;
    }

    private void SetHurtState(CharacterType type, Character enemyCharacter, Vector3 vector)
    {
        if (type == CharacterType.Enemy && character == enemyCharacter && currentState != EnemyState.IsDead)
        {
            previousState = currentState;
            currentState = EnemyState.IsHurt;
        }
    }

    private void SetDeathState(CharacterType type, Character enemyCharacter, Vector3 vector)
    {
        if (type == CharacterType.Enemy && character == enemyCharacter)
        {
            currentState = EnemyState.IsDead;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleStates()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.DetectTarget:
                DetectTarget();
                break;
            case EnemyState.AttackTarget:
                AttackTarget();
                break;
            case EnemyState.Backoff:
                Backoff();
                break;
            case EnemyState.IsDead:
                CharacterDeath();
                break;
            case EnemyState.IsHurt:
                CharacterHurt();
                break;
        }
    }

    private void CharacterHurt()
    {
        foreach (var animator in animators)
        {
            StartCoroutine(PlayDeathAnimationAndDestroy(animator, "isHurt", "Hurt"));
        }
        rb.velocity = Vector3.zero;
        currentState = previousState;
    }

    private void CharacterDeath()
    {


        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
        collider.enabled = false;

        foreach (var animator in animators)
        {
            animator.SetBool("deathState", true);
            StartCoroutine(PlayDeathAnimationAndDestroy(animator, "isDead", "DeathAnimation"));
        }
    }

    protected virtual IEnumerator PlayDeathAnimationAndDestroy(Animator animator, string trigger, string animation)
    {
        animator.SetTrigger(trigger);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (stateInfo.IsName(animation) && stateInfo.normalizedTime < 1.0f)
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }
       
        // Optionally destroy the enemy
         Destroy(gameObject, 2);
    }

    protected bool InRangeOfTarget(float range, GameObject target)
    {
        return target != null && Vector2.Distance(target.transform.position, transform.position) <= range;
    }

    private void Patrol()
    {
        moveDirection = Vector2.zero; // No movement during patrol

        if (InRangeOfTarget(detectRange, target))
        {
            currentState = EnemyState.DetectTarget;
            _eventManager.OnPlaySoundEffect?.Invoke(_detectSoundEffect, transform.position);
        }
    }

    private void DetectTarget()
    {
        if (target != null)
        {
            MoveTowardsTarget();

            if (InRangeOfTarget(attackRange, target))
            {
                currentState = EnemyState.AttackTarget;
            }
            else if (!InRangeOfTarget(detectRange, target))
            {
                currentState = EnemyState.Patrol;
            }
        }
    }

    private void AttackTarget()
    {
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance <= backoffRange)
            {
                currentState = EnemyState.Backoff;
            }
            else if (distance <= stopRange)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                MoveTowardsTarget();
            }

            if (!InRangeOfTarget(attackRange, target))
            {
                currentState = EnemyState.DetectTarget;
            }
        }
    }

    private void Backoff()
    {
        if (target != null)
        {
            float distanceToTarget = target.transform.position.x - transform.position.x;
            moveDirection = new Vector2(-Mathf.Sign(distanceToTarget), 0);
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

            if (Vector2.Distance(target.transform.position, transform.position) > backoffRange)
            {
                if (InRangeOfTarget(attackRange, target))
                {
                    currentState = EnemyState.AttackTarget;
                }
                else
                {
                    currentState = EnemyState.DetectTarget;
                }
            }
        }
    }

    private void FindTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, detectRange, targetLayer);
        if (colliders.Length > 0)
        {
            target = colliders[0].gameObject;
        }
    }

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            float distanceToTarget = target.transform.position.x - transform.position.x;
            moveDirection = new Vector2(Mathf.Sign(distanceToTarget), 0);
            rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

            DetectObstaclesAndJump();

            if (moveDirection.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveDirection.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void DetectObstaclesAndJump()
    {
        // Cast a ray forward to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(frontCheck.position, moveDirection, obstacleCheckDistance, groundLayer);

        if (hit.collider != null && isGrounded)
        {
            // If an obstacle is detected and the player is higher than the enemy, jump
            if (target != null && target.transform.position.y > transform.position.y + 1.0f)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void UpdateAnimations()
    {
        foreach (var animator in animators)
        {
            animator.SetBool("isGrounded", isGrounded);

            if (currentState == EnemyState.Patrol)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
            }
            else if (currentState == EnemyState.DetectTarget)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
            else if (currentState == EnemyState.AttackTarget)
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
            }
            else if (currentState == EnemyState.Backoff)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
        }
    }

    public void FireWeapon()
    {
        weapon?.UsePrimaryWeapon();
    }

}



