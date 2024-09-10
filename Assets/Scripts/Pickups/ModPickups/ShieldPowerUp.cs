using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : PowerUpPickUp
{

    protected ShieldSystem _shieldSystem;


    private EventManager _eventManager;

    

    public override void ActivatePowerUp(Character character)
    {     
        if ((_shieldSystem = character.GetComponentInChildren<ShieldSystem>(true)) == null)
            return;
  
        _shieldSystem.gameObject.SetActive(true);

        _shieldSystem.ActivateShield();
    }

    public override void DeactivatePowerUp(Character character)
    {
        if (_shieldSystem != null)
        {
            _shieldSystem?.DisableShield();
            character.EventManager.OnPlaySoundEffect?.Invoke("ShieldDownEffect", character.transform.position);
        }
    }
}
