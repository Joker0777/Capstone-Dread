using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : CharacterSystems
{
    [SerializeField] protected List<Weapon> _primaryWeaponList = new List<Weapon>();
    [SerializeField] protected WeaponSpawnPoints[] _weaponSpawnPoints;

    [SerializeField] protected string _weaponProjectileParentTag;
    [SerializeField] protected string _weaponTarget;

    protected Weapon _currentPrimaryWeapon;
    protected Transform[] _currentWeaponSpawnPoints;
    protected int _currentWeaponIndex = 0;
    protected int _primaryWeaponSpawnPointIndex;
    protected GameObject _weaponProjectileParent;
    protected Timer _primaryCooldownTimer;
    protected float _currentPrimaryWeaponCooldown;
    protected float _fireRateIncreaseFactor = 1f;

    public float WeaponCooldown
    {
        get => _currentPrimaryWeaponCooldown;
        set
        {
            _currentPrimaryWeaponCooldown = value / _fireRateIncreaseFactor;
            if (_primaryCooldownTimer != null)
            {
                _primaryCooldownTimer.TimerDuration = _currentPrimaryWeaponCooldown;
            }
        }
    }

    public float FireRateIncreaseFactor
    {
        get => _fireRateIncreaseFactor;
        set
        {
            _fireRateIncreaseFactor = value;
            WeaponCooldown = _currentPrimaryWeaponCooldown;
        }
    }


    protected override void Start()
    {
        base.Start();
        _primaryCooldownTimer = new Timer(_currentPrimaryWeaponCooldown);
        _weaponProjectileParent = GameObject.FindWithTag(_weaponProjectileParentTag);
        InitializeWeaponList();
    }

  

    protected virtual void InitializeWeaponList()
    {
        if (_primaryWeaponList.Count > 0)
        {
            _currentPrimaryWeapon = _primaryWeaponList[_currentWeaponIndex];
            Debug.Log(_primaryWeaponList.Count);

        }
    }

    public virtual void UsePrimaryWeapon()
    {
        if (_primaryCooldownTimer.IsRunningCoroutine) return;
        UseWeaponLogic(_currentPrimaryWeapon, ref _primaryWeaponSpawnPointIndex, _primaryCooldownTimer);
        Debug.Log("In use primary weapon");
    }

    protected virtual void UseWeaponLogic(Weapon weapon, ref int spawnPointIndex, Timer cooldownTimer)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Current weapon is null, cannot fire weapon.");
            return;
        }

        SetUpWeapon(weapon.WeaponType);

        if (_currentWeaponSpawnPoints == null || _currentWeaponSpawnPoints.Length == 0)
        {
            Debug.LogWarning("No valid spawn points available for current weapon.");
            return;
        }

        spawnPointIndex = (spawnPointIndex + 1) % _currentWeaponSpawnPoints.Length;
        Transform weaponSpawn = _currentWeaponSpawnPoints[spawnPointIndex];

        if (weaponSpawn == null)
        {
            Debug.LogWarning("Weapon spawn point is null.");
            return;
        }

        weapon.UseWeapon(weaponSpawn, _weaponTarget);
        _eventManager.OnPlaySoundEffect?.Invoke(weapon.name + "Effect", weaponSpawn.position);

        cooldownTimer.StartTimerCoroutine();
    }

    protected void SetUpWeapon(string weaponType)
    {
        foreach (var weaponSpawn in _weaponSpawnPoints)
        {
            if (weaponSpawn.WeaponTypeTag == weaponType)
            {
                _currentWeaponSpawnPoints = weaponSpawn.SpawnLocations;
                WeaponCooldown = _currentPrimaryWeapon.WeaponCooldown;
                return;
            }
        }
    }

    public virtual void SwitchWeapon(int switchDirection)
    {
        if (_primaryWeaponList.Count == 0) return;

        _currentWeaponIndex = (_currentWeaponIndex + switchDirection + _primaryWeaponList.Count) % _primaryWeaponList.Count;
        _currentPrimaryWeapon = _primaryWeaponList[_currentWeaponIndex];
        _currentPrimaryWeapon.InitializeWeapon();
    }
}
