
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CharacterWeaponSystem : CharacterSystems
{
    [SerializeField] protected Animator _animator;
     
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
    [SerializeField] protected GameObject _weaponProjectileParent;

    private bool _isReloading = false;
    [SerializeField] protected GunPositonAdjust _gunPositonAdjust;


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
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeapon, "None");
        _eventManager.OnUIChange?.Invoke(UIElementType.BarrelMod, "0");
        _eventManager.OnUIChange?.Invoke(UIElementType.ScopeMod, "0");
        _eventManager.OnUIChange?.Invoke(UIElementType.GripMod, "0");

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

        if (_secondaryWeaponList.Count > 0)
        {
            _currentSecondaryWeapon = _secondaryWeaponList[_currentSecondaryWeaponIndex];
        }

        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeapon, _currentPrimaryWeapon.name);
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, _currentPrimaryWeapon.CurrrentAmmo.ToString() + "/" + _currentPrimaryWeapon.CurrentMagazineSize.ToString());
    }



    public void AddWeapon(ProjectileWeapon addedWeapon, List<ProjectileWeapon> weaponList, ref ProjectileWeapon currentWeapon, ref GameObject currentVisuals)
    {
        if (addedWeapon != null && !weaponList.Contains(addedWeapon))
        {
            weaponList.Add(addedWeapon);

            if(weaponList.Count == 1)
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

    public void AddSecondaryWeapon(ProjectileWeapon addedWeapon)
    {
        AddWeapon(addedWeapon, _secondaryWeaponList, ref _currentSecondaryWeapon, ref _currentSecondaryWeaponVisual);
        addedWeapon.InitializeWeapon();
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
                    _eventManager.OnUIChange?.Invoke(UIElementType.BarrelMod, _barrelModsList.Count.ToString());
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
                    _eventManager.OnUIChange?.Invoke(UIElementType.ScopeMod, _sightModsList.Count.ToString());
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
                    _eventManager.OnUIChange?.Invoke(UIElementType.GripMod, _gripModsList.Count.ToString());
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
            _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, _currentPrimaryWeapon.CurrrentAmmo.ToString() + "/" + _currentPrimaryWeapon.CurrentMagazineSize.ToString());
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
        if (_isReloading || _primaryCooldownTimer.IsRunningCoroutine || _currentPrimaryWeapon.CurrrentAmmo <= 0) return;

        FireWeaponLogic(_currentPrimaryWeapon, ref _primaryWeaponSpawnPointIndex, _primaryCooldownTimer);
    }

    public void StopPrimaryWeapon()
    {
        _eventManager.OnWeaponStoped?.Invoke();
    }

    public void FireSecondaryWeapon()
    {
        if (_secondaryCooldownTimer.IsRunningCoroutine || _currentSecondaryWeapon.CurrrentAmmo <=0) return;

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
        _eventManager.OnWeaponFired?.Invoke(_currentPrimaryWeapon.ShellCasingTag, _currentPrimaryWeaponCooldown);
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, _currentPrimaryWeapon.CurrrentAmmo.ToString() + "/" + _currentPrimaryWeapon.CurrentMagazineSize.ToString());

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
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeaponAmmo, _currentSecondaryWeapon.CurrrentAmmo.ToString() + "/" + _currentSecondaryWeapon.CurrentMagazineSize.ToString());
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
        UpdateWeaponMods();
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeapon, _currentPrimaryWeapon.name);
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, _currentPrimaryWeapon.CurrrentAmmo.ToString() + "/" + _currentPrimaryWeapon.CurrentMagazineSize.ToString());
    }

    public void SwitchSecondaryWeapon(int switchDirection)
    {
        if (_secondaryWeaponList.Count == 0)  return; 

        SwitchWeapon(switchDirection, _secondaryWeaponList, ref _currentSecondaryWeapon, ref _currentSecondaryWeaponIndex);
        if (_currentSecondaryWeaponVisual != null)
        {
            Destroy(_currentSecondaryWeaponVisual);
        }
        _currentSecondaryWeaponVisual = Instantiate(_currentSecondaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
        _currentSecondaryWeaponVisual.transform.localPosition = Vector3.zero;
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeapon, _currentSecondaryWeapon.name);
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeaponAmmo, _currentSecondaryWeapon.CurrrentAmmo.ToString() + "/" + _currentSecondaryWeapon.CurrentMagazineSize.ToString());
    }

    public void ReloadPrimaryWeapon()
    {
        if (_currentPrimaryWeapon.CurrrentAmmo < _currentPrimaryWeapon.CurrentMagazineSize)
        {
            _animator.SetTrigger("Reload");
            StartCoroutine(HandleReload(_currentPrimaryWeapon));
        }

    }

    private IEnumerator HandleReload(ProjectileWeapon weapon)
    {
        _isReloading = true;  
        Vector3 currentGunOffset = _gunPositonAdjust.gunOffset;
        Vector3 targetGunOffset = Vector3.zero; 

  
        float transitionDuration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startOffset = currentGunOffset;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            _gunPositonAdjust.gunOffset = Vector3.Lerp(startOffset, targetGunOffset, t);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the gun offset is exactly zero
        _gunPositonAdjust.gunOffset = targetGunOffset;

        // Get the duration of the reload animation
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float reloadDuration = stateInfo.length;

        // Wait for the duration of the reload animation
        yield return new WaitForSeconds(reloadDuration);

        // Perform the reload action on the weapon
        weapon.Reload();

        // Smoothly transition the gun offset back to the original value
        elapsedTime = 0f;
        startOffset = Vector3.zero; // Start from zero after reload

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            _gunPositonAdjust.gunOffset = Vector3.Lerp(startOffset, currentGunOffset, t);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the gun offset is exactly at the original position
        _gunPositonAdjust.gunOffset = currentGunOffset;

        _isReloading = false;  // Re-enable firing after reloading

        // Update UI with new ammo count
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo,
            weapon.CurrrentAmmo.ToString() + "/" + weapon.CurrentMagazineSize.ToString());
    }
}
