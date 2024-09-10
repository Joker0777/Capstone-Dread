using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseFireRatePowerUp : PowerUpPickUp
{
    private float _fireRateIncreaseFactor;

    private float _currentCooldown;

    private float _currentFireRateIncreaseFactor;

   // private PlayerWeaponSystem weaponSystem;

    public IncreaseFireRatePowerUp(float fireRateIncreaseFactor = 2f) 
    { 
        _fireRateIncreaseFactor = fireRateIncreaseFactor; 
    }

    public override void ActivatePowerUp(Character character)
    {
       // if ((weaponSystem = character.GetComponentInChildren<PlayerWeaponSystem>()) == null)
         //   return;

       
      // _currentCooldown = weaponSystem.WeaponCooldown;
      // _currentFireRateIncreaseFactor = weaponSystem.FireRateIncreaseFactor;

      //  weaponSystem.FireRateIncreaseFactor = _fireRateIncreaseFactor;
    }

    public override void DeactivatePowerUp(Character character)
    {
      //  if (weaponSystem != null)
      //  {
       //     weaponSystem.WeaponCooldown = _currentCooldown;
       //     weaponSystem.FireRateIncreaseFactor = _currentFireRateIncreaseFactor;
      //  }
    }
}
