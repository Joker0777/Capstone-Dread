using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseSpeedPowerUP : PowerUpPickUp
{
    private float _speedIncreaseFactor;

    private float _currentSpeed;

    protected MovmentSystem movmentSystem;

    public IncreaseSpeedPowerUP(float speedIncreaseFactor = 2f)
    {  
        _speedIncreaseFactor = speedIncreaseFactor; 
    }

    public override void ActivatePowerUp(Character character)
    {
        if ((movmentSystem = character.GetComponentInChildren<MovmentSystem>()) == null)
            return;

        _currentSpeed = movmentSystem.Speed;

        movmentSystem.Speed *= _speedIncreaseFactor;
    }

    public override void DeactivatePowerUp(Character character)
    {
        if (movmentSystem != null)
        {
            movmentSystem.Speed = _currentSpeed;
        }
    }
}
