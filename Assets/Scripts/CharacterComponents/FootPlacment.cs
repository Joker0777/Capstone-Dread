using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPlacment : MonoBehaviour
{

    [SerializeField] private Transform leftFootTarget;
    [SerializeField] private Transform rightFootTarget;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f; // Offset to prevent foot from sinking into the ground
    [SerializeField] private float raycastDistance = 1f;

    private void Update()
    {
        AdjustFootTarget(leftFootTarget);
        AdjustFootTarget(rightFootTarget);
    }

    private void AdjustFootTarget(Transform footTarget)
    {
        // Cast a ray down from the foot target
        RaycastHit2D hit = Physics2D.Raycast(footTarget.position + Vector3.up * 0.5f, Vector2.down, raycastDistance, groundLayer);

        if (hit.collider != null)
        {
            // Move the foot target to the ground position with a slight offset
            Vector3 targetPosition = new Vector3(hit.point.x, hit.point.y, 0) + Vector3.up * footOffset;
            footTarget.position = targetPosition;
        }
    }

}
