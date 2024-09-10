using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFeetControler : MonoBehaviour
{
    public Transform leftFootEffector;  // Reference to the left foot effector
    public Transform rightFootEffector; // Reference to the right foot effector
    public float stepHeight = 0.5f;     // Maximum height of the foot arc for walking
    public float stepDuration = 0.5f;   // Duration of one step cycle
    public float moveSpeed = 2f;        // Speed of forward movement
    public LayerMask groundLayer;       // Layer mask to detect the ground

    private float leftStepProgress = 0f;
    private float rightStepProgress = 0.5f; // Right foot starts halfway through the cycle
    private Vector3 leftStartPos;
    private Vector3 leftEndPos;
    private Vector3 rightStartPos;
    private Vector3 rightEndPos;

    public Vector3 backPostion;
    public Vector3 forwardPostion;

    private bool _isFirstStep;

    void Start()
    {
        // Initialize the start and end positions based on the current position
        leftStartPos = leftFootEffector.position;
        rightStartPos = rightFootEffector.position;

        CalculateStepEndPosition(ref leftEndPos, leftStartPos);
        CalculateStepEndPosition(ref rightEndPos, rightStartPos);


    }

    void Update()
    {
        // Move the character forward at a constant speed
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);

        // Update the step progress for each foot
        leftStepProgress += Time.deltaTime / stepDuration;
        rightStepProgress += Time.deltaTime / stepDuration;

        // Reset progress after a full step cycle and calculate new positions
        if (leftStepProgress > 1f)
        {
            leftStepProgress = 0f;
            leftStartPos = leftFootEffector.position;
            CalculateStepEndPosition(ref leftEndPos, leftStartPos);
        }

        if (rightStepProgress > 1f)
        {
            rightStepProgress = 0f;
            rightStartPos = rightFootEffector.position;
            CalculateStepEndPosition(ref rightEndPos, rightStartPos);
        }

        // Calculate the positions for both feet
        Vector2 leftFootPosition = CalculateFootPosition(leftStartPos, leftEndPos, leftStepProgress);
        Vector2 rightFootPosition = CalculateFootPosition(rightStartPos, rightEndPos, rightStepProgress);

        // Update the foot effectors' positions
        leftFootEffector.position = new Vector3(leftFootPosition.x, leftFootPosition.y, leftFootEffector.position.z);
        rightFootEffector.position = new Vector3(rightFootPosition.x, rightFootPosition.y, rightFootEffector.position.z);
    }

    void CalculateStepEndPosition(ref Vector3 endPos, Vector3 startPos)
    {
        float stepDistance = moveSpeed * stepDuration;
        endPos = startPos + new Vector3(stepDistance, 0f, 0f);
    }

    Vector2 CalculateFootPosition(Vector3 startPos, Vector3 endPos, float stepProgress)
    {
        // Pin the foot to the start position during the ground contact phase
        if (stepProgress < 0.5f)
        {
            // Perform a raycast to detect the ground
            RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, Mathf.Infinity, groundLayer);
            if (hit.collider != null)
            {
                // Adjust the foot's position based on the terrain height
                return new Vector2(startPos.x, hit.point.y);
            }
            else
            {
                // If no ground is detected, fall back to the default position
                return new Vector2(startPos.x, startPos.y);
            }
        }

        // Otherwise, calculate the forward step arc
        Vector2 horizontalPosition = Vector2.Lerp(startPos, endPos, (stepProgress - 0.5f) * 2f);
        float verticalOffset = Mathf.Sin((stepProgress - 0.5f) * Mathf.PI *2f) * stepHeight;

        // Adjust the foot position based on the terrain at the current horizontal position
        Vector2 footPosition = new Vector2(horizontalPosition.x, horizontalPosition.y + verticalOffset);
        RaycastHit2D groundHit = Physics2D.Raycast(footPosition, Vector2.down, Mathf.Infinity, groundLayer);
        if (groundHit.collider != null)
        {
            // Adjust the foot's height to match the terrain
            footPosition.y = Mathf.Max(footPosition.y, groundHit.point.y);
        }

        return footPosition;
    }
}
