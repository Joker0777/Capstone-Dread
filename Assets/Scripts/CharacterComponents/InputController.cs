using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{

    private float horizontal;
    private float vertical;

    private ICharacter2D _movableCharacter;

    protected void Awake()
    {
        _movableCharacter = transform.GetComponent<ICharacter2D>();
    }


    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        Vector2 moveInput = new Vector2(horizontal, vertical);

        _movableCharacter?.Move(moveInput);
       
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            _movableCharacter?.JumpButtonDown();
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            _movableCharacter?.JumpButtonUp();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            _movableCharacter?.SprintOn();
        }

        if(Input.GetKeyUp(KeyCode.LeftShift)) 
        { 
                    _movableCharacter?.SprintOff();
        }


    }
}