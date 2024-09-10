using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private AIState currentState;
    

    void Start()
    {
        ChangeState(new PatrolState(this));
    }

 
    void Update()
    {
        if (currentState != null)
        { 
            currentState.OnStateRun();
        }     
    }

    public void ChangeState(AIState state)
    {
        if (currentState != null)
        {
            currentState.OnStateExit();
        }

        currentState = state;

        currentState.OnStateEnter();
    }

    public bool InRange(float range, GameObject target)
    {
        return target != null && Vector2.Distance(target.transform.position, transform.position) < range;
    }

    public bool InLineWtihTarget(GameObject target, float inLineThreshold)
    {
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        return Vector2.Dot(transform.up, targetDirection) >= inLineThreshold;
    }

    public void FollowTarget(GameObject target)
    {
        if (target == null) return;

        List<GameObject> obsticles = GetObsticlesInRange();
        Vector2 targetDirection = (target.transform.position - transform.position).normalized;
        Vector2 collisionAvoidenceOffset = GetAvoidenceOffset(obsticles);
        Vector2 combinedDirection = (targetDirection + collisionAvoidenceOffset).normalized;

     //   directionToMove = combinedDirection;

       // moveInput = InRange(_stopRange, target) ? Vector2.zero : Vector2.up;

     //   _targetToWeaponForwardDot = Vector2.Dot(transform.up, directionToMove);

    }

    private List<GameObject> GetObsticlesInRange()
    {
      //  var colliders = Physics2D.OverlapCircleAll(transform.position, asteroidAvoidanceRange);
        var obsticles = new List<GameObject>();

       // foreach (var collider in colliders)
       // {
            //if (collider.gameObject != gameObject && !collider.CompareTag("Player"))
        //    if (collider.gameObject != gameObject)
        //    {
         //       obsticles.Add(collider.gameObject);
       //     }
       // }

        return obsticles;
    }

    private Vector2 GetAvoidenceOffset(List<GameObject> Obsticles)
    {
        Vector2 avoidanceOffset = Vector2.zero;

        foreach (var obsticles in Obsticles)
        {
            Vector2 awayFromObsticle = (transform.position - obsticles.transform.position).normalized;
            Vector2 toObsticle = (obsticles.transform.position - transform.position).normalized;

            Vector2 perpendicularLeft = new Vector2(-toObsticle.y, toObsticle.x);
            Vector2 perpendicularRight = new Vector2(toObsticle.y, -toObsticle.x);

            float dotProduct = Vector2.Dot(transform.right, toObsticle);

            Vector2 sideAvoidance;

            if (dotProduct > 0)
            {
                sideAvoidance = perpendicularLeft;
            }
            else
            {
                sideAvoidance = perpendicularRight;
            }

            avoidanceOffset += (awayFromObsticle + sideAvoidance) / Vector2.Distance(transform.position, obsticles.transform.position);
        }

        return avoidanceOffset.normalized;

    }




}
