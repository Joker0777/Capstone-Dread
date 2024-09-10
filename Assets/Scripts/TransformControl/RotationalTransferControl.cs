using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RotationalTransferControl : MonoBehaviour
{
    [SerializeField] private Transform sourceTransform; // The parent transform to follow
    [SerializeField] private DependentTransformObject[] dependentTransforms; // Child transforms with their offsets and dampening speeds
    [SerializeField] private float rotationSpeed;

    private EventManager _eventManager;
    private bool _isFacingRight;

    private void Awake()
    {
        _eventManager = EventManager.Instance;
    }

    private void OnEnable()
    {
        _eventManager.IsFacingRight += FlipCharacter;
    }

    private void OnDisable()
    {
        _eventManager.IsFacingRight -= FlipCharacter;
    }

    private void FlipCharacter(bool isFacingRight)
    {
        _isFacingRight = isFacingRight;
    }

    private void Update()
    {
        foreach (var dependentTransform in dependentTransforms)
        {
            // Calculate the offset based on the character's facing direction
            float adjustedOffset = _isFacingRight ? dependentTransform.rotationOffset : -dependentTransform.rotationOffset;

            // Calculate target rotation based on parent’s rotation and apply per-child offset
            float targetZRotation = sourceTransform.eulerAngles.z + adjustedOffset;

            // Apply rotation constraints for each dependent transform
            targetZRotation = Mathf.Clamp(targetZRotation, dependentTransform.minRotation, dependentTransform.maxRotation);

            // Convert current local rotation to a consistent 0-360 range (avoiding wrap-around issues)
            float currentLocalRotationZ = dependentTransform.dependentTransform.localEulerAngles.z;
            if (currentLocalRotationZ > 180f)
            {
                currentLocalRotationZ -= 360f; // Convert range from (0,360) to (-180,180)
            }

            // Smoothly interpolate the child’s local Z rotation towards the target Z rotation using LerpAngle
            float newLocalRotationZ = Mathf.LerpAngle(currentLocalRotationZ, targetZRotation, Time.deltaTime * dependentTransform.dampeningSpeed);

            // Apply the new local rotation to the child transform
            dependentTransform.dependentTransform.localRotation = Quaternion.Euler(0, 0, newLocalRotationZ);
        }
    }
}


