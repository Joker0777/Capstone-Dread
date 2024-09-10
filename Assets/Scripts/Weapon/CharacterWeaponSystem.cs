
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CharacterWeaponSystem : CharacterSystems
{
    [SerializeField] protected List<ProjectileWeapon> _primaryWeaponList = new List<ProjectileWeapon>();
    [SerializeField] protected List<ProjectileWeapon> _secondaryWeaponList = new List<ProjectileWeapon>();

    protected List<WeaponMod> _sightModsList = new List<WeaponMod>();
    protected List<WeaponMod> _gripModsList = new List<WeaponMod>();
    protected List<WeaponMod> _barrelModsList = new List<WeaponMod>();

    [SerializeField] protected WeaponSpawnPoints[] _weaponSpawnPoints;
    [SerializeField] protected Transform _projectileParent;
    [SerializeField] protected Transform _weaponVisualTransform;
    [SerializeField] protected Vector3 _weaponVisualOffset;

    [SerializeField] protected float _currentPrimaryWeaponCooldown;
    [SerializeField] protected float _currentSecondaryWeaponCooldown;

    [SerializeField] protected string _weaponTarget;

    protected ProjectileWeapon _currentPrimaryWeapon;
    protected ProjectileWeapon _currentSecondaryWeapon;

    private WeaponMod _currentSightMod;
    private WeaponMod _currentGripMod;
    private WeaponMod _currentBarrelMod;

    protected GameObject _currentPrimaryWeaponVisual;
    protected GameObject _currentSecondaryWeaponVisual;
    protected GameObject _currentGripVisual;
    protected GameObject _currentBarrelVisual;
    protected GameObject _currentSightVisual;

    protected Transform[] _currentWeaponSpawnPoints;
    [SerializeField] Transform _secondaryWeaponSpawnPoint;

    protected int _currentWeaponIndex = 0;
    protected int _currentSecondaryWeaponIndex = 0;

    protected int _primaryWeaponSpawnPointIndex;

    protected Timer _primaryCooldownTimer;
    protected Timer _secondaryCooldownTimer;

    protected float _fireRateIncreaseFactor = 1f;

    [SerializeField] protected GameObject[] _weaponVisuals;

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
        _secondaryCooldownTimer = new Timer(_currentSecondaryWeaponCooldown);
 
        InitializeWeaponLists();
    }

    private void InitializeWeaponLists()
    {
        if (_primaryWeaponList.Count > 0)
        {
            _currentPrimaryWeapon = _primaryWeaponList[_currentWeaponIndex];
            _currentPrimaryWeaponVisual = Instantiate(_currentPrimaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentPrimaryWeaponVisual.transform.localPosition = Vector3.zero;
        }

        if (_secondaryWeaponList.Count > 0)
        {
            _currentSecondaryWeapon = _secondaryWeaponList[_currentSecondaryWeaponIndex];
        }
    }



    public void AddWeapon(ProjectileWeapon addedWeapon, List<ProjectileWeapon> weaponList, ref ProjectileWeapon currentWeapon, ref GameObject currentVisuals)
    {
        if (addedWeapon != null && !weaponList.Contains(addedWeapon))
        {
            weaponList.Add(addedWeapon);
            currentWeapon = addedWeapon;
            if(weaponList.Count == 1)
            {
                currentVisuals = Instantiate(currentWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
                currentVisuals.transform.localPosition = Vector3.zero;
            }

  

          //  foreach (var weapon in _weaponVisuals)
         //   {
            //    if (weapon != null && addedWeapon.WeaponType == weapon.name)
            //    {
             //       weapon.SetActive(true);
              //  }
          //  }
        }
    }

    public void AddPrimaryWeapon(ProjectileWeapon addedWeapon)
    {
        AddWeapon(addedWeapon, _primaryWeaponList, ref _currentPrimaryWeapon, ref _currentPrimaryWeaponVisual);
    }

    public void AddSecondaryWeapon(ProjectileWeapon addedWeapon)
    {
        AddWeapon(addedWeapon, _secondaryWeaponList, ref _currentSecondaryWeapon, ref _currentSecondaryWeaponVisual);
    }




    public void AddMod(WeaponMod addedMod)
    {

        if (addedMod == null) return;

        switch (addedMod.ModType)
        {
            case ModType.Barrel:
                if (!_barrelModsList.Contains(addedMod))
                {
                    _barrelModsList.Add(addedMod);
                    if(_barrelModsList.Count == 1)
                    {
                        _currentBarrelMod = _barrelModsList[0];
                        
                    }
                }

                break;

            case ModType.Sight:
                if (!_sightModsList.Contains(addedMod))
                {
                    _sightModsList.Add(addedMod);
                    if(_sightModsList.Count == 1)
                    {
                        _currentSightMod = _sightModsList[0];
                    }
                }

                break;

            case ModType.Grip:
                if (!_gripModsList.Contains(addedMod))
                {
                    _gripModsList.Add(addedMod);
                    if(_gripModsList.Count == 1)
                    {
                        _currentGripMod = _gripModsList[0];
                    }
                }

                break;
        }
        UpdateWeaponMods();
    }

    private void UpdateWeaponMods()
    {
        _currentPrimaryWeapon.ClearMods();

        if (_currentGripMod != null)
        {
            _currentPrimaryWeapon.ApplyMod(_currentGripMod);

            if (_currentGripVisual != null)
            {
                Destroy(_currentGripVisual);
            }
            _currentGripVisual = Instantiate(_currentGripMod.ModVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentGripVisual.transform.localPosition = Vector3.zero;
        }

        if (_currentBarrelMod != null)
        {
            _currentPrimaryWeapon.ApplyMod(_currentBarrelMod);

            if (_currentBarrelVisual != null)
            {
                Destroy(_currentBarrelVisual);
            }
            _currentBarrelVisual = Instantiate(_currentBarrelMod.ModVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentBarrelVisual.transform.localPosition = Vector3.zero;
        }

        if (_currentSightMod != null)
        {
            _currentPrimaryWeapon.ApplyMod(_currentSightMod);

            if (_currentSightVisual != null)
            {
                Destroy(_currentSightVisual);
            }
            _currentSightVisual = Instantiate(_currentSightMod.ModVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentSightVisual.transform.localPosition = Vector3.zero;
        }
    }





    public void FirePrimaryWeapon()
    {
        if (_primaryCooldownTimer.IsRunningCoroutine) return;

        FireWeaponLogic(_currentPrimaryWeapon, ref _primaryWeaponSpawnPointIndex, _primaryCooldownTimer);
    }

    public void FireSecondaryWeapon()
    {
        if (_secondaryCooldownTimer.IsRunningCoroutine) return;

        FireWeaponLogic(_currentSecondaryWeapon, _secondaryWeaponSpawnPoint, _secondaryCooldownTimer);
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

    private void FireWeaponLogic(ProjectileWeapon weapon, Transform weaponSpawnPoint, Timer cooldownTimer)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Current weapon is null, cannot fire weapon.");
            return;
        }

        SetUpWeapon(weapon.WeaponType);

        weapon.UseWeapon(weaponSpawnPoint, _weaponTarget);
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
        currentWeapon.InitializeWeapon();
        



    }

    public void SwitchPrimaryWeapon(int switchDirection)
    {
        SwitchWeapon(switchDirection, _primaryWeaponList, ref _currentPrimaryWeapon, ref _currentWeaponIndex);
        if (_currentPrimaryWeaponVisual != null)
        {
            Destroy(_currentPrimaryWeaponVisual);
        }
        _currentPrimaryWeaponVisual = Instantiate(_currentPrimaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
        _currentPrimaryWeaponVisual.transform.localPosition = Vector3.zero;
        UpdateWeaponMods();
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeapon, _currentPrimaryWeapon.name);
    }

    public void SwitchSecondaryWeapon(int switchDirection)
    {
        SwitchWeapon(switchDirection, _secondaryWeaponList, ref _currentSecondaryWeapon, ref _currentSecondaryWeaponIndex);
        if (_currentSecondaryWeaponVisual != null)
        {
            Destroy(_currentSecondaryWeaponVisual);
        }
        _currentSecondaryWeaponVisual = Instantiate(_currentSecondaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
        _currentSecondaryWeaponVisual.transform.localPosition = Vector3.zero;
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeapon, _currentSecondaryWeapon.name);
    }

    /*



    public void AddMod(WeaponMod weaponMod, ModType modType)
    {
        if (weaponMod == null) return;

        switch (modType)
        {
            case ModType.Barrel:
                _barrelMods.Add(weaponMod);
                break;

            case ModType.Ammo:
                _ammoMods.Add((AmmoMod)weaponMod);
                break;

            case ModType.Secondary:
                _ammoMods.Add((AmmoMod)weaponMod);
                break;

            case ModType.Sight:
                _sightMods.Add(weaponMod);
                break;

            case ModType.Grip:
                _gripMods.Add(weaponMod);
                break;
        }
    }


    public void SwitchAmmo(int switchDirection)
    {
        if (_ammoMods.Count == 0) return;

        _currentAmmoIndex = (switchDirection > 0) ? (_currentAmmoIndex + 1) % _ammoMods.Count : (_currentAmmoIndex - 1 + _ammoMods.Count) % _ammoMods.Count;
        AmmoMod newAmmo = _ammoMods[_currentAmmoIndex];

        if (_currentAmmo != newAmmo)
        {
            _primaryWeapon.ChangeAmmo(_currentAmmo, newAmmo);
            _currentAmmo = newAmmo;
        }

      //  _eventManager.OnUIChange?.Invoke(UIElementType.Weapon, _currentWeapon.name);
    }



    public void SwitchWeaponMod(WeaponMod newWeaponMod) 
    {
        if (_primaryWeapon == null)
        {
            Debug.LogWarning("Primary weapon is not set.");
            return;
        }

        switch (newWeaponMod.ModType)
        {
            case ModType.Barrel:
                if (_currentBarrelMod != null)
                {
                    _currentBarrelMod.RemoveMod(_primaryWeapon);
                }
                _currentBarrelMod = newWeaponMod;
                _currentBarrelMod.AddMod(_primaryWeapon);
              //  UpdateModVisuals();
                break;

            case ModType.Sight:
                if (_currentSightMod != null)
                {
                    _currentSightMod.RemoveMod(_primaryWeapon);
                }
                _currentSightMod = newWeaponMod;
                _currentSightMod.AddMod(_primaryWeapon);
               // UpdateModVisuals();
                break;

            case ModType.Grip:
                if (_currentGripMod != null)
                {
                    _currentGripMod.RemoveMod(_primaryWeapon);
                }
                _currentGripMod = newWeaponMod;
                _currentGripMod.AddMod(_primaryWeapon);
               // UpdateModVisuals();
                break;

            default:
                Debug.LogWarning($"Unsupported mod type: {newWeaponMod.ModType}");
                break;
        }
    }
    */
}
