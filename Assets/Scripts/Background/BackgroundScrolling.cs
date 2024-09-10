using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    [SerializeField] private float scrollSpeedFactor;

    private Transform _mainCameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {      
        _mainCameraTransform = Camera.main.transform;
        lastCameraPosition = transform.position;
    }

    void Update()
    {
        Vector3 movmentDirection = (_mainCameraTransform.position - lastCameraPosition).normalized;
        transform.position += new Vector3(movmentDirection.x * scrollSpeedFactor, movmentDirection.y * scrollSpeedFactor, movmentDirection.z);
        lastCameraPosition = _mainCameraTransform.position;
    }
}
