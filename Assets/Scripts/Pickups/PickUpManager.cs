using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{

    public static PickUpManager instance;

    //[SerializeField] private PickupSpawner _pickupSpawner;
    [SerializeField] private PlayerCharacter _playerCharacter;
   // [SerializeField] private ScoreManager _scoreManager;

    //powerUp pickups
   // private PowerUpPickUp _currentPowerUpPickUp;

    //powerup timer
    [SerializeField] protected float _powerUpTimerLength = 10;
    private Timer _PowerUpTimer;
    private float _timeRemaining;
    private bool _powerUpTimerRunning;

    //player systems references
    private CharacterWeaponSystem _weaponSystem;
    private MovmentSystem _movmentSystem;

    private EventManager _eventManager;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
          //  DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _eventManager = EventManager.Instance;
    }

    private void Start()
    {
        _PowerUpTimer = new Timer(_powerUpTimerLength);

        _weaponSystem = _playerCharacter.GetComponent<CharacterWeaponSystem>();
        _movmentSystem = _playerCharacter.GetComponent<MovmentSystem>();

      //  _eventManager.OnUIChange?.Invoke(UIElementType.PickUpTimer, "0");
       // _eventManager.OnUIChange?.Invoke(UIElementType.pickUp, "EmptyPowerUp");

    }

    private void OnEnable()
    {
       // _eventManager.OnPlayerRespawn += ResetPlayer;

    }

    private void OnDisable()
    {
       // _eventManager.OnPlayerRespawn -= ResetPlayer;
    }

    private void Update()
    {
        if (_powerUpTimerRunning)
        {
            _PowerUpTimer.UpdateTimerBasic(Time.deltaTime);

          //  _eventManager.OnUIChange?.Invoke(UIElementType.PickUpTimer, Mathf.CeilToInt(_PowerUpTimer.TimeRemaining).ToString());
          //  _eventManager.OnUIChange?.Invoke(UIElementType.pickUp,_currentPowerUpPickUp.ToString());

            if (!_PowerUpTimer.IsRunningBasic())
            {
                _powerUpTimerRunning = false;
               // _currentPowerUpPickUp.DeactivatePowerUp(_playerCharacter);
              //  _eventManager.OnUIChange?.Invoke(UIElementType.PickUpTimer, "0");
              //  _eventManager.OnUIChange?.Invoke(UIElementType.pickUp, "EmptyPowerUp");
            }
        }
       
    }

    

    public void CollectPickup(PlayerCharacter character, PickUp pickup, WeaponMod mod = null, ProjectileWeapon weapon = null, int healthAmount = 10)
    {
        _playerCharacter = character;

        if (_playerCharacter == null) return;
  
        _eventManager.OnScoreIncrease?.Invoke("Pickup");
    
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
    
    
    
    


    public void DestoyAllPickUps()
    {
     //   List<GameObject> spawnedPickUps = _pickupSpawner.SpawnedPickUps;

     //   if(spawnedPickUps != null && spawnedPickUps.Count > 0)
       // {
       //     foreach (GameObject pickUp in spawnedPickUps)
       //     {
         //       if (pickUp != null)
          //      {
         //           Destroy(pickUp.gameObject);
           //     }
         //   }
          //  spawnedPickUps.Clear();
       // }
    }

    private void ResetPlayer(PlayerCharacter character)
    {
        _playerCharacter = character;

     //   playerWeaponSystem = _playerUnit.GetComponentInChildren<PlayerWeaponSystem>();
        _movmentSystem = _playerCharacter.GetComponentInChildren<MovmentSystem>();
    }
           
    
}
