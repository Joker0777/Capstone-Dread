using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTenticlePart : MonoBehaviour
{
    public float speed;
    public Transform target;
    private Vector2 direction;
    public bool IsFacingRight = true;

    void Start()
    {
        // Find the RotateToTarget component in the scene (make sure it exists and is properly assigned)
        RotateToTarget rotateToTarget = GetComponentInParent<RotateToTarget>();

        // Subscribe to the OnFlipDirection event
        if (rotateToTarget != null)
        {
            rotateToTarget.OnFlipDirection += FlipTentacle;
        }
    }

    void Update()
    {
        // Calculate the direction to the target
        direction = target.position - transform.position;

        // Calculate the angle to rotate towards
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle if the tentacle is facing left
       // if (!IsFacingRight)
      //  {
       //     angle += 180f; // Flip the angle to face the opposite direction
       // }

        // Create the target rotation and smoothly interpolate towards it
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
    }

    private void FlipTentacle(bool facingRight)
    {
        IsFacingRight = facingRight;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        RotateToTarget rotateToTarget = FindObjectOfType<RotateToTarget>();
        if (rotateToTarget != null)
        {
            rotateToTarget.OnFlipDirection -= FlipTentacle;
        }
    }
}
