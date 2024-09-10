using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWalkAnimationControl : MonoBehaviour
{
    // Foot movement
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    public Transform leftFootEffector;
    public Transform rightFootEffector;

    public Transform footEndCycle;
    public Transform footStartCycle;

    public float stepHeight = 0.5f;
    public float stepDuration = 0.5f;
    public float stepDistance;

    public LayerMask groundLayer;
    public float moveSpeed = 2f;

    public float leftStepProgress = 0f;
    public float rightStepProgress = 0.5f;

    // Body vertical movement
    public Transform LowerBodyTransform;
    public float verticalBodyMovement;
    public float verticalBodyMovementOffset = 0.0012f;

    // Shoulder movement
    public Transform leftShoulderTransform;
    public Transform rightShoulderTransform;
    public float horizontalShoulderMovementOffset = 0.001f;
    public float rightHorizontalShoulderMovement;
    public float leftHorizontalShoulderMovement;

    // Head movement
    public Transform neckTransform;
    public float horizontalNeckMovementOffset;
    public float horizontalNeckMovement;

    private void Update()
    {
        AdjustStepDuration();

        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

        HeadMovementControl();
        VerticalBodyMovementControl();
        ShoulderMovementControl();

        leftStepProgress += Time.deltaTime / stepDuration;
        rightStepProgress += Time.deltaTime / stepDuration;

        if (leftStepProgress > 1f) leftStepProgress = 0f;
        if (rightStepProgress > 1f) rightStepProgress = 0f;

        Vector2 leftFootPosition = CalculateFootPosition(footStartCycle.position, footEndCycle.position, leftStepProgress);
        Vector2 rightFootPosition = CalculateFootPosition(footStartCycle.position, footEndCycle.position, rightStepProgress);

        leftFootTarget.position = new Vector3(leftFootPosition.x, leftFootPosition.y, leftFootTarget.position.z);
        rightFootTarget.position = new Vector3(rightFootPosition.x, rightFootPosition.y, rightFootTarget.position.z);
    }

 
    private void AdjustStepDuration()
    {

        float stepDistance = Vector3.Distance(footStartCycle.position, footEndCycle.position);

        // Calculate the step duration: 
        // Increase the duration based on the step distance to move speed ratio
        stepDuration = stepDistance / moveSpeed;

        // Apply an additional multiplier to slow down the step duration if necessary
        stepDuration *= 1.5f; // Adjust this multiplier to fine-tune

        // Optionally, clamp the step duration to a sensible range
        stepDuration = Mathf.Clamp(stepDuration, 0.3f, 2f);
    }

    private void HeadMovementControl()
    {
        horizontalNeckMovement = Mathf.Sin(leftStepProgress * Mathf.PI * 2) * horizontalNeckMovementOffset;

        Vector3 neckPosition = neckTransform.position;

        neckPosition.x += horizontalNeckMovement;

        neckTransform.position = neckPosition;
    }

    private void ShoulderMovementControl()
    {
        leftHorizontalShoulderMovement = -Mathf.Sin(leftStepProgress * Mathf.PI * 2) * horizontalShoulderMovementOffset;
        rightHorizontalShoulderMovement = -Mathf.Sin(rightStepProgress * Mathf.PI * 2) * horizontalShoulderMovementOffset;

        Vector3 leftShoulderPosition = leftShoulderTransform.localPosition;
        Vector3 rightShoulderPosition = rightShoulderTransform.localPosition;

        leftShoulderPosition.y += leftHorizontalShoulderMovement;
        rightShoulderPosition.y += rightHorizontalShoulderMovement;

        leftShoulderTransform.localPosition = leftShoulderPosition;
        rightShoulderTransform.localPosition = rightShoulderPosition;
    }

    private void VerticalBodyMovementControl()
    {
        verticalBodyMovement = -Mathf.Sin(leftStepProgress * Mathf.PI * 4) * verticalBodyMovementOffset;

        Vector3 characterPosition = transform.position;
        characterPosition.y += verticalBodyMovement;
        transform.position = characterPosition;
    }

    private Vector2 CalculateFootPosition(Vector3 startPos, Vector3 endPos, float stepProgress)
    {
        /*
        float easedProgress = EasingFunction(stepProgress);

        if (easedProgress < 0.5f)
        {
            Vector2 backMovement = Vector2.Lerp(endPos, startPos, easedProgress * 2f);

            RaycastHit2D hit = Physics2D.Raycast(backMovement + new Vector2(0, 0.5f), Vector2.down, Mathf.Infinity, groundLayer);
            if (hit.collider != null)
            {
                return new Vector2(backMovement.x, hit.point.y);
            }
            return new Vector2(backMovement.x, backMovement.y);
        }

        Vector2 horizontalPosition = Vector2.Lerp(startPos, endPos, (easedProgress - 0.5f) * 2f);
        float verticalOffset = Mathf.Sin((easedProgress - 0.5f) * Mathf.PI * 2f) * stepHeight;
        Vector2 footPosition = new Vector2(horizontalPosition.x, horizontalPosition.y + verticalOffset);

        RaycastHit2D groundHit = Physics2D.Raycast(footPosition + new Vector2(0, 0.5f), Vector2.down, Mathf.Infinity, groundLayer);
        if (groundHit.collider != null)
        {
            footPosition.y = Mathf.Max(footPosition.y, groundHit.point.y);
        }

        return footPosition;
        */
      

        if (stepProgress < 0.5f)
        {
            Vector2 backMovement = Vector2.Lerp(endPos, startPos, stepProgress * 2f);

            RaycastHit2D hit = Physics2D.Raycast(backMovement + new Vector2(0, 0.5f), Vector2.down, Mathf.Infinity, groundLayer);
            if (hit.collider != null)
            {
                return new Vector2(backMovement.x, hit.point.y);
            }
            return new Vector2(backMovement.x, backMovement.y);
        }

        Vector2 horizontalPosition = Vector2.Lerp(startPos, endPos, (stepProgress - 0.5f) * 2f);
        float verticalOffset = Mathf.Sin((stepProgress - 0.5f) * Mathf.PI * 2f) * stepHeight;
        Vector2 footPosition = new Vector2(horizontalPosition.x, horizontalPosition.y + verticalOffset);

        RaycastHit2D groundHit = Physics2D.Raycast(footPosition + new Vector2(0, 0.5f), Vector2.down, Mathf.Infinity, groundLayer);
        if (groundHit.collider != null)
        {
            footPosition.y = Mathf.Max(footPosition.y, groundHit.point.y);
        }

        return footPosition;

    }

  //  private float EasingFunction(float t)
  //  {
        // Smoothstep function for easing
   ///     return t * t;
   // }
}
