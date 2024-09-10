using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DependentTransformObject
{
    public Transform dependentTransform; // The child transform that will follow
    public float rotationOffset; // Offset for this child transform's rotation
    public float dampeningSpeed = 5f; // Speed at which the child follows the parent

    public float minRotation = -30f; // Minimum rotation angle for this child transform
    public float maxRotation = 30f;  // Maximum rotation angle for this child transform
}
