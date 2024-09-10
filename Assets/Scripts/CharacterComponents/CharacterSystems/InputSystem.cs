using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputSystem : CharacterSystems
{
    protected float _horizontalInput;
    protected float _verticalInput;
    protected float _scrolledInput;

    private IMovable movement;
    private CharacterWeaponSystem weapon;

    Vector2 _distanceToFace;
    float _angleToTurn;

    protected override void Start()
    {
        base.Start();
        movement = GetComponentInChildren<IMovable>();
        weapon = GetComponentInChildren<CharacterWeaponSystem>();

        if (movement == null)
        {
            Debug.Log("Movment not assigned or does not implement IMovable");
        }

        if (weapon == null)
        {
            Debug.Log("Weapon not assigned or does not implement IWeapon");
        }
    }


    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _scrolledInput = Input.GetAxis("Mouse ScrollWheel");

        Vector2 moveInput = new Vector2(_horizontalInput, _verticalInput);



        movement?.Move(moveInput);


        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        movement?.SetRunning(isRunning);

        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        movement?.SetCrouching(isCrouching);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement?.Jump();
        }

        //Primary Weapon
         if (Input.GetMouseButton(0))
          {
            weapon?.FirePrimaryWeapon();
          }

        //Secondary Weapon
        if (Input.GetMouseButtonDown(1))
        {
            weapon?.FireSecondaryWeapon();
        }

        //Change Primary Weapon
        if (_scrolledInput > 0 || Input.GetKeyDown(KeyCode.E))
        {
            weapon?.SwitchPrimaryWeapon(1);
        }

        if (_scrolledInput < 0 || Input.GetKeyDown(KeyCode.Q))
        {
            weapon?.SwitchPrimaryWeapon(-1);
        }
    }
}
