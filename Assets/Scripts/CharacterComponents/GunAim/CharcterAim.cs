using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class CharcterAim : CharacterSystems
{
    [SerializeField] private MultiAimConstraint[] _constraints;
    [SerializeField] private Transform gunTransform;
    [SerializeField] private float minAngle = -45f;
    [SerializeField] private float maxAngle = 45f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform characterTransform;


    [SerializeField] private Vector3 offsetWhenFacingRight;
    [SerializeField] private Vector3 offsetWhenFacingLeft;

    [SerializeField] private float flipBuffer = 0.1f;
    [SerializeField] private float angleCorrection = 0f;

    [SerializeField] private string aimTargetTag;
    [SerializeField] private string gunSightTag;

    private Transform _gunSightTransform;
    [SerializeField] private Transform _aimTarget;
    public bool IsFacingRight { get; set; }


    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _eventManager.OnGameSceneStart += ResetAimTarget;
    }
    protected override void Start()
    {
        base.Start();
        IsFacingRight = true;
        _eventManager.IsFacingRight?.Invoke(IsFacingRight);

        AssignSceneTransforms();
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
      
        _gunSightTransform.position = mousePosition;
     
        Vector3 aimDirection = mousePosition - gunTransform.position;

        if (!IsFacingRight)
        {
            _aimTarget.position = new Vector3(mousePosition.x, -mousePosition.y, mousePosition.z) + offsetWhenFacingLeft;
            aimDirection.x = -aimDirection.x;
        }
        else
        {
            _aimTarget.position = mousePosition + offsetWhenFacingRight;        
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
    private void OnDestroy()
    {

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignSceneTransforms();
    }

    private void ResetAimTarget()
    {
       // _aimTarget.SetParent(this.transform);
    }

    private void AssignSceneTransforms()
    {
        _gunSightTransform = GameObject.FindWithTag("GunSight").transform;
        // _aimTarget = GameObject.FindWithTag("AimTarget").transform;

       // _aimTarget.SetParent(null);
        if (_aimTarget == null || _gunSightTransform == null)
        {
            Debug.LogWarning("One or more required objects were not found in the scene!");
        }
        else
        {
            Debug.Log("Objects successfully assigned.");
        }
    }
}
