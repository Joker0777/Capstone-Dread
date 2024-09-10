using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviorSystem : CharacterSystems
{
    private Rigidbody2D rb;
    private Collider2D collider;
    public GameObject target;

    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float attackRange = 7f;
 

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private RangedWeaponSystem weapon;
    [SerializeField] private Animator[] animators;

    private bool isGrounded;

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
            weapon = GetComponentInChildren<RangedWeaponSystem>();
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
        this.enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector3.zero;
        collider.enabled = false;


        foreach (var animator in animators)
        {
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

        // Destroy(gameObject,4);
    }

    protected bool InRangeOfTarget(float range, GameObject target)
    {
        return target != null && Vector2.Distance(target.transform.position, transform.position) <= range;
    }

    private void Patrol()
    {

        if (InRangeOfTarget(detectRange, target))
        {
            currentState = EnemyState.DetectTarget;
        }
    }

    private void DetectTarget()
    {
        if (target != null)
        {
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

            if (!InRangeOfTarget(attackRange, target))
            {
                currentState = EnemyState.DetectTarget;
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


    private void UpdateAnimations()
    {
        foreach (var animator in animators)
        {
            if (currentState == EnemyState.Patrol)
            {
                animator.SetBool("isDetected", false);
                animator.SetBool("isAttacking", false);
            }
            else if (currentState == EnemyState.DetectTarget)
            {
                animator.SetBool("isDetected", true);
                animator.SetBool("isDetected", false);
            }
            else if (currentState == EnemyState.AttackTarget)
            {
                animator.SetBool("isDetected", false);
                animator.SetBool("isAttacking", true);
            }
        }
    }

    public void FireWeapon()
    {
        weapon?.UsePrimaryWeapon();
    }
}
