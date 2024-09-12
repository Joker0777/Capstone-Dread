using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingENemyBehaviourScript : CharacterSystems
{
    private Rigidbody2D rb;
    public GameObject target;
    private Vector2 moveDirection;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 7f;
    [SerializeField] private float stopRange = 5f;  // Ensure stopRange is less than attackRange
    [SerializeField] private float backoffRange = 2f;  // Range for backing off

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private WeaponSystem weapon;
    [SerializeField] private Animator[] animators;

    public enum EnemyState { Patrol, DetectTarget, AttackTarget, Backoff, IsDead, IsHurt }
    public EnemyState currentState = EnemyState.Patrol;
    public EnemyState previousState = EnemyState.Patrol;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();

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

        HandleStates();
        UpdateAnimations();
    }
    private void OnEnable()
    {
        _eventManager.OnCharacterDestroyed += SetDeathState;
        _eventManager.OnCharacterHurt += SetHurtState;
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

        rb.velocity = Vector3.zero;

        foreach (var animator in animators)
        {
            StartCoroutine(PlayDeathAnimationAndDestroy(animator, "isDead", "DeathAnimation"));
        }
        this.enabled = false;
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

         Destroy(gameObject,4);
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
            // Backoff if player is too close
            if (distance <= backoffRange)
            {
                currentState = EnemyState.Backoff;
            }
            else if (distance <= stopRange)
            {
                rb.velocity = Vector2.zero; // Stop movement

            }
            else
            {
                MoveTowardsTarget(); // Move towards target if not in stop range
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
            Vector2 directionToTarget = (target.transform.position - transform.position).normalized;

            // Move away from the target in the opposite direction
            moveDirection = -directionToTarget;  // Move in the opposite direction
            rb.velocity = moveDirection * moveSpeed;

            // Transition back to Attack or Detect state if distance increases
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
            // Calculate direction towards the target
            Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
            moveDirection = directionToTarget;
            rb.velocity = moveDirection * moveSpeed;

            // Face the enemy towards the target (only horizontally flip if needed)
            if (moveDirection.x > 0)
                transform.localScale = new Vector3(1, 1, 1); // Facing right
            else if (moveDirection.x < 0)
                transform.localScale = new Vector3(-1, 1, 1); // Facing left
        }
    }

    private void UpdateAnimations()
    {
        foreach (var animator in animators)
        {
            if (currentState == EnemyState.Patrol)
            {
                animator.SetBool("isFlying", false);
                animator.SetBool("isAttacking", false);
            }
            else if (currentState == EnemyState.DetectTarget)
            {
                animator.SetBool("isFlying", true);
                animator.SetBool("isAttacking", false);
            }
            else if (currentState == EnemyState.AttackTarget)
            {
                animator.SetBool("isFlying", false);
                animator.SetBool("isAttacking", true);
            }
            else if (currentState == EnemyState.Backoff)
            {
                animator.SetBool("isFlying", true); // Flying animation during backoff
                animator.SetBool("isAttacking", false);
            }
        }
    }

    public void FireWeapon()
    {
        if(currentState == EnemyState.AttackTarget)
        {
         weapon?.UsePrimaryWeapon();
        }
    }
}
