using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTransferControl : MonoBehaviour
{
    [SerializeField] private Transform neckTransform;
    [SerializeField] private Transform torsoTransform;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float rotationAmount = 15f;

    void Update()
    {
        // Simple test to rotate neck and torso
        if (neckTransform != null)
        {
            RotateTransform(neckTransform, rotationAmount);
        }

        if (torsoTransform != null)
        {
            RotateTransform(torsoTransform, rotationAmount * 0.5f); // Adjust rotation amount for torso
        }
    }

    private void RotateTransform(Transform transformToRotate, float angle)
    {
        float currentAngle = transformToRotate.localEulerAngles.z;
        float targetAngle = currentAngle + angle * Mathf.Sin(Time.time) * rotationSpeed;
        transformToRotate.localRotation = Quaternion.Euler(0, 0, targetAngle);
    }


}
