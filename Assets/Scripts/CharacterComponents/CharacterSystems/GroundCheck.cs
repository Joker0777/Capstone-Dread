using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public float distanceThreshold = 0.01f;
    public bool isGrounded = true;
    public Transform LeftFootPosition;
    public Transform RightFootPosition;

    public LayerMask groundLayerMask;

    private Vector2 LeftFootRaycastOrigin => LeftFootPosition.position;
    private Vector2 RightFootRaycastOrigin => RightFootPosition.position;
    private float RaycastDistance => distanceThreshold;

    private RaycastHit2D hitInfo;
    private int currentGroundLayer = -1;

    public bool IsGrounded
    {
        get { return isGrounded; }
        private set { isGrounded = value; }
    }

    private void FixedUpdate()
    {
        bool isGroundedLeft = Physics2D.Raycast(LeftFootRaycastOrigin, Vector2.down, RaycastDistance, groundLayerMask);
        bool isGroundedRight = Physics2D.Raycast(RightFootRaycastOrigin, Vector2.down, RaycastDistance, groundLayerMask);

        // Check if both feet are grounded
        bool isGroundedNow = isGroundedLeft || isGroundedRight;

        if (isGroundedNow && !isGrounded)
        {
            IsGrounded = true;
        }

        if (!isGroundedNow && isGrounded)
        {
            IsGrounded = false;
        }

        isGrounded = isGroundedNow;

        if (isGrounded)
        {
            hitInfo = Physics2D.Raycast(LeftFootRaycastOrigin, Vector2.down, RaycastDistance, groundLayerMask);
            currentGroundLayer = hitInfo.collider != null ? hitInfo.collider.gameObject.layer : -1;
        }
        else
        {
            currentGroundLayer = -1;
        }

        Debug.DrawRay(LeftFootRaycastOrigin, Vector2.down * RaycastDistance, Color.green);
        Debug.DrawRay(RightFootRaycastOrigin, Vector2.down * RaycastDistance, Color.blue);
    }

    public int GetGroundLayer()
    {
        return currentGroundLayer;
    }
}
