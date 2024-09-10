using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharcterAim : CharacterSystems
{
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform characterTransform;

    [SerializeField] private Transform aimTarget;
    [SerializeField] private Vector3 offsetWhenFacingRight;
    [SerializeField] private Vector3 offsetWhenFacingLeft;

    [SerializeField] private float flipBuffer = 0.1f;
    [SerializeField] private float angleCorrection = 0f;
    public bool IsFacingRight { get; set; }

    protected override void Start()
    {
        base.Start();
        IsFacingRight = true;
        _eventManager.IsFacingRight?.Invoke(IsFacingRight);
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 aimDirection = mousePosition - gunTransform.position;

        if (!IsFacingRight)
        {
            aimTarget.position = new Vector3(mousePosition.x, -mousePosition.y, mousePosition.z) + offsetWhenFacingLeft;
            aimDirection.x = -aimDirection.x;

        }
        else
        {
            aimTarget.position = mousePosition + offsetWhenFacingRight;
        }

        if (mousePosition.x > characterTransform.position.x + flipBuffer && !IsFacingRight)
        {
            FlipCharacter(true);
        }
        else if (mousePosition.x < characterTransform.position.x - flipBuffer && IsFacingRight)
        {
            FlipCharacter(false);
        }

        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);

        if (!IsFacingRight)
        {
            targetAngle += angleCorrection;
        }

        float currentAngle = gunTransform.localEulerAngles.z;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        gunTransform.localRotation = Quaternion.Euler(0, 0, newAngle);
    }

    private void FlipCharacter(bool facingRight)
    {
        IsFacingRight = facingRight;
        _eventManager.IsFacingRight?.Invoke(IsFacingRight);
        characterTransform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }
}
