using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{

    public static PickUpManager instance;

    [SerializeField] private PlayerCharacter _playerCharacter;
    private CharacterWeaponSystem _weaponSystem;

    private EventManager _eventManager;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _eventManager = EventManager.Instance;

        if(_playerCharacter == null)
        {
            _playerCharacter = FindObjectOfType<PlayerCharacter>();
        }

    }

    public PlayerCharacter PlayerCharacter 
    { 
        set 
        {
            _playerCharacter = value;
            _weaponSystem = _playerCharacter.GetComponent<CharacterWeaponSystem>();
        } 
    }

    private void Start()
    {
        if(_playerCharacter != null)
        {
            _weaponSystem = _playerCharacter.GetComponent<CharacterWeaponSystem>();
        }
        else
        {
            Debug.Log("Player not assinged in pickup manager");
        }

    }


    public void CollectPickup(PlayerCharacter character, PickUp pickup, WeaponMod mod = null, ProjectileWeapon weapon = null, int healthAmount = 10, int ammoAmount = 1)
    {
        _playerCharacter = character;

        if (_playerCharacter == null) return;
  
        _eventManager.OnPlaySoundEffect?.Invoke("Pickup", transform.position);
    
        switch (pickup.PickupType)
        {
            case PickupType.PrimaryWeaponPickup:
                if(weapon != null)
                {
                    CollectPrimaryWeapon(weapon);
                }
                break;

            case PickupType.SecondaryWeaponPickup:
                if (weapon != null)
                {
                    CollectSecondaryWeapon(weapon);
                }
                break;

            case PickupType.ModPickup:            
                if(mod != null)
                {
                    CollectMod(mod);
                }
                break;

            case PickupType.HealthPickup:
                CollectHealth(healthAmount);
                break;
            case PickupType.SecondaryAmmo:
                CollectSecondaryAmmo(weapon, ammoAmount);
                break;
        }      
    }


    private void CollectPrimaryWeapon(ProjectileWeapon weapon)
    {    
       _weaponSystem?.AddPrimaryWeapon(weapon);      
    }

    private void CollectSecondaryWeapon(ProjectileWeapon weapon)
    {
        _weaponSystem?.AddSecondaryWeapon(weapon);     
    }

    private void CollectHealth(int healthAmount)
    {
        _playerCharacter.HealthIncrease(healthAmount);
    }
  
    private void CollectMod(WeaponMod mod)
    {
        _weaponSystem.AddMod(mod);
    }

    private void CollectSecondaryAmmo(ProjectileWeapon weapon, int ammoAmount)
    {
        _weaponSystem?.AddSecondaryAmmo(weapon,ammoAmount);
    }
    
      
    private void ResetPlayer(PlayerCharacter character)
    {
        _playerCharacter = character;
        _weaponSystem = _playerCharacter.GetComponent<CharacterWeaponSystem>();
    }   
}
