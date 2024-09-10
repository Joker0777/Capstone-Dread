using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : CharacterSystems
{
    [SerializeField] private Transform _transformToRotate;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float flipBuffer = 0.1f;
    [SerializeField] private bool _flipSprite;
    public bool IsFacingRight { get; set; }

    private int _rightDirection = 1;

    // Define the event for flipping direction
    public event Action<bool> OnFlipDirection;

    protected override void Start()
    {
        base.Start();  
        IsFacingRight = true;
        _rightDirection = _flipSprite ? -1 : 1;
        characterTransform.localScale = new Vector3(IsFacingRight ? _rightDirection : -_rightDirection, 1, 1);
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 aimDirection = mousePosition - _transformToRotate.position;

        if (!IsFacingRight)
        {
            aimTarget.position = new Vector3(-mousePosition.x, mousePosition.y, mousePosition.z);
            aimDirection.x = -aimDirection.x;
            aimDirection.y = -aimDirection.y;
        }
        else
        {
            aimTarget.position = mousePosition;
            aimDirection.y = -aimDirection.y;
        }

        if (mousePosition.x > characterTransform.position.x + flipBuffer && !IsFacingRight)
        {
            FlipTarget(true);
        }
        else if (mousePosition.x < characterTransform.position.x - flipBuffer && IsFacingRight)
        {
            FlipTarget(false);
        }

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        float currentAngle = _transformToRotate.localEulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        _transformToRotate.localRotation = Quaternion.Euler(0, 0, newAngle);

        Vector2 target = aimTarget.position;
     
            transform.position = Vector2.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
        

    }

    private void FlipTarget(bool facingRight)
    {
        IsFacingRight = facingRight;

        // Invoke the OnFlipDirection event to notify subscribers
        OnFlipDirection?.Invoke(IsFacingRight);
        characterTransform.localScale = new Vector3(IsFacingRight ? _rightDirection : -_rightDirection, 1, 1);
    }


}
