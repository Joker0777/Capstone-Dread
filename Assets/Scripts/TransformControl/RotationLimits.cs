using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLimits : MonoBehaviour
{
    [SerializeField] private Transform constrainedTransform; // The transform you want to constrain
    [SerializeField] private float minAngle = -45f; // Minimum angle
    [SerializeField] private float maxAngle = 45f; // Maximum angle

    void LateUpdate()
    {
        // Get the current local rotation of the constrained object
        Vector3 currentEulerAngles = constrainedTransform.localEulerAngles;

        // Normalize angles between -180 and 180
        float zAngle = currentEulerAngles.z;
        if (zAngle > 180) zAngle -= 360;

        // Clamp the Z angle to min and max limits
        zAngle = Mathf.Clamp(zAngle, minAngle, maxAngle);

        // Apply the clamped rotation back to the constrained object
        constrainedTransform.localRotation = Quaternion.Euler(currentEulerAngles.x, currentEulerAngles.y, zAngle);
    }
}
