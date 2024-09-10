using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DependentTransform : MonoBehaviour
{
    public Transform sourceObject;
    public DependentTransformObject[] dependentTransforms;

    public abstract void UpdateDependentTransforms();
}
