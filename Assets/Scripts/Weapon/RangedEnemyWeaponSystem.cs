using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyWeaponSystem : CharacterSystems
{
    [SerializeField] protected Animator _animator;

    [SerializeField] protected List<ProjectileWeapon> _primaryWeaponList = new List<ProjectileWeapon>();


    [SerializeField] protected WeaponSpawnPoints[] _weaponSpawnPoints;
    [SerializeField] protected Transform _projectileParent;
    [SerializeField] protected Transform _weaponVisualTransform;

    [SerializeField] protected float _currentPrimaryWeaponCooldown;

    [SerializeField] protected string _weaponTarget;

    protected ProjectileWeapon _currentPrimaryWeapon;

    protected GameObject _currentPrimaryWeaponVisual;

    protected Transform[] _currentWeaponSpawnPoints;

    protected int _currentWeaponIndex = 0;
    protected int _currentSecondaryWeaponIndex = 0;

    protected int _primaryWeaponSpawnPointIndex;

    protected Timer _primaryCooldownTimer;

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
  
        InitializeWeaponLists();
    }

    private void InitializeWeaponLists()
    {
        if (_primaryWeaponList.Count > 0)
        {
            _currentPrimaryWeapon = _primaryWeaponList[_currentWeaponIndex];
            _currentPrimaryWeaponVisual = Instantiate(_currentPrimaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentPrimaryWeaponVisual.transform.localPosition = Vector3.zero;
            _currentPrimaryWeapon.InitializeWeapon();
        }
    }



    public void AddWeapon(ProjectileWeapon addedWeapon, List<ProjectileWeapon> weaponList, ref ProjectileWeapon currentWeapon, ref GameObject currentVisuals)
    {
        if (addedWeapon != null && !weaponList.Contains(addedWeapon))
        {
            weaponList.Add(addedWeapon);

            if (weaponList.Count == 1)
            {
                currentWeapon = addedWeapon;
                currentVisuals = Instantiate(currentWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
                currentVisuals.transform.localPosition = Vector3.zero;

            }
        }
    }

    public void AddPrimaryWeapon(ProjectileWeapon addedWeapon)
    {
        AddWeapon(addedWeapon, _primaryWeaponList, ref _currentPrimaryWeapon, ref _currentPrimaryWeaponVisual);
        addedWeapon.InitializeWeapon();
    }


    public void FirePrimaryWeapon()
    {
        if( _primaryCooldownTimer.IsRunningCoroutine || _currentPrimaryWeapon.CurrrentAmmo <= 0) return;

        FireWeaponLogic(_currentPrimaryWeapon, ref _primaryWeaponSpawnPointIndex, _primaryCooldownTimer);
    }

 



    private void FireWeaponLogic(ProjectileWeapon weapon, ref int spawnPointIndex, Timer cooldownTimer)
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
        Debug.LogWarning("No matching weapon type found for: " + weaponType);
    }



    public void SwitchWeapon(int switchDirection, List<ProjectileWeapon> weaponList, ref ProjectileWeapon currentWeapon, ref int currentIndex)
    {
        if (weaponList.Count == 0) return;

        currentIndex = (currentIndex + switchDirection + weaponList.Count) % weaponList.Count;
        currentWeapon = weaponList[currentIndex];
    }

    public void SwitchPrimaryWeapon(int switchDirection)
    {
        if (_primaryWeaponList.Count == 0) return;

        SwitchWeapon(switchDirection, _primaryWeaponList, ref _currentPrimaryWeapon, ref _currentWeaponIndex);
        if (_currentPrimaryWeaponVisual != null)
        {
            Destroy(_currentPrimaryWeaponVisual);
        }
        _currentPrimaryWeaponVisual = Instantiate(_currentPrimaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
        _currentPrimaryWeaponVisual.transform.localPosition = Vector3.zero;  
    }

  
}
