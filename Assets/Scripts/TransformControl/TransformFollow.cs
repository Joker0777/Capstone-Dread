using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
    [SerializeField] private Transform neckTransform; // Reference to the neck (or head) transform
    [SerializeField] private Transform targetTransform; // The target point to look at
    [SerializeField] private float rotationSpeed = 2f; // Speed of neck rotation

    private void Update()
    {
        if (neckTransform == null || targetTransform == null) return;

        // Calculate direction from the neck to the target point
        Vector3 directionToTarget = targetTransform.position - neckTransform.position;
        directionToTarget.z = 0; // Ensure movement is only in the X-Y plane

        // Calculate the target angle
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Get current local rotation angle
        float currentAngle = neckTransform.localEulerAngles.z;

        // Smoothly rotate the neck towards the target angle
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);
        neckTransform.localRotation = Quaternion.Euler(0, 0, newAngle);
    }
}
