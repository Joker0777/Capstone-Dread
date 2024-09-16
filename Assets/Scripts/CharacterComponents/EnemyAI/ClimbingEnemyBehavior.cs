using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingEnemyBehavior : CharacterSystems
{
   
        private Rigidbody2D rb;
        private Collider2D collider;
        public GameObject target;
        private Vector2 moveDirection;

        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float climbSpeed = 3f;
        [SerializeField] private float jumpForce = 5f;  // Jump force for the enemy
        [SerializeField] private float detectRange = 10f;
        [SerializeField] private float attackRange = 7f;
        [SerializeField] private float stopRange = 5f;
        [SerializeField] private float backoffRange = 2f;

        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform frontCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private float frontCheckDistance = 1f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private WeaponSystem weapon;
        [SerializeField] private Animator[] animators;
        [SerializeField] private string _detectSoundEffect;


        private bool isGrounded;
        private bool _isClimbing;

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
            CheckObstacleAhead();
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
        private void CheckObstacleAhead()
        {
            _isClimbing = Physics2D.Raycast(frontCheck.position, Vector2.right, frontCheckDistance, groundLayer);
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
            this.enabled = false;
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

             Destroy(gameObject,20);
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

        private void AttackTarget()//added blend tree stuff
        {
            if (target != null)
            {
                float distance = Vector2.Distance(transform.position, target.transform.position);

                // Map the distance to a value between 0 (close) and 1 (far) for the blend tree
                float blendValue = Mathf.InverseLerp(backoffRange, attackRange, distance);

                // Update the blend tree parameter in the animator
                foreach (var animator in animators)
                {
                    animator.SetFloat("attackRange", blendValue);
                }

                // Backoff if player is too close
                if (distance <= backoffRange)
                {
                    currentState = EnemyState.Backoff;  // Transition to backoff state
                }
                else if (distance <= stopRange)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y); // Stop movement
 
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
                float distanceToTarget = target.transform.position.x - transform.position.x;

                // Move away from the target without flipping direction
                moveDirection = new Vector2(-Mathf.Sign(distanceToTarget), 0);  // Move in the opposite direction
                rb.velocity = new Vector2(moveDirection.x * moveSpeed, rb.velocity.y);

                // Do not flip the sprite in backoff

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
                float distanceToTarget = target.transform.position.x - transform.position.x;
                moveDirection = new Vector2(Mathf.Sign(distanceToTarget), 0);

                // Use climbing speed if climbing, otherwise use normal move speed
                float speed = _isClimbing ? climbSpeed : moveSpeed;

                rb.velocity = new Vector2(moveDirection.x * speed, rb.velocity.y);

                // Flip enemy sprite based on direction (only when moving towards target)
                if (moveDirection.x > 0)
                    transform.localScale = new Vector3(-1, 1, 1); // Facing right
                else if (moveDirection.x < 0)
                    transform.localScale = new Vector3(1, 1, 1); // Facing left
            }
        }

        private void UpdateAnimations()
        {
            foreach (var animator in animators)
            {
                float movementType = _isClimbing ? 1 : 0;
                animator.SetFloat("MovementType", movementType);


                if (currentState == EnemyState.Patrol)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalkingBackwards", false);
                }
                else if (currentState == EnemyState.DetectTarget)
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalkingBackwards", false);
                }
                else if (currentState == EnemyState.AttackTarget)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isAttacking", true);
                    animator.SetBool("isWalkingBackwards", false);
                }
                else if (currentState == EnemyState.Backoff)
                {
                    animator.SetBool("isWalkingBackwards", true);
                    animator.SetBool("isAttacking", false);
                    animator.SetBool("isWalking", false);
                }
            }
        }

        public void FireWeapon()
        {
            weapon?.UsePrimaryWeapon();
        }
    
}
