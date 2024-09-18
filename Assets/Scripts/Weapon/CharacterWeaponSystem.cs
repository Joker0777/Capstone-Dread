
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

    [SerializeField] protected GameObject[] _weaponVisuals;


    private bool _isReloading = false;
    [SerializeField] protected GunPositonAdjust _gunPositonAdjust;


    public float PrimaryWeaponCooldown
    {
        get => _currentPrimaryWeaponCooldown;
        set
        {
            _currentPrimaryWeaponCooldown = value;
            if (_primaryCooldownTimer != null)
            {
                _primaryCooldownTimer.TimerDuration = _currentPrimaryWeaponCooldown;
            }
        }
    }

    public float SecondaryWeaponCooldown
    {
        get => _currentSecondaryWeaponCooldown;
        set
        {
            _currentSecondaryWeaponCooldown = value;
            if (_secondaryCooldownTimer != null)
            {
                _secondaryCooldownTimer.TimerDuration = _currentSecondaryWeaponCooldown;
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        InitializeTimers();
        InitializeUI();
        //  _eventManager.OnGameSceneStart += InitializePlayer;
        InitializeWeaponLists();
    }

    private void InitializeTimers()
    {
        _primaryCooldownTimer = new Timer(_currentPrimaryWeaponCooldown);
        _secondaryCooldownTimer = new Timer(_currentSecondaryWeaponCooldown);
    }

    private void InitializeUI()
    {
        _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeapon, "None");
        _eventManager.OnUIChange?.Invoke(UIElementType.BarrelMod, "0");
        _eventManager.OnUIChange?.Invoke(UIElementType.ScopeMod, "0");
        _eventManager.OnUIChange?.Invoke(UIElementType.GripMod, "0");
    }

    private void InitializeWeaponLists()
    {
        InitializeWeapon(_primaryWeaponList, ref _currentPrimaryWeapon, ref _currentPrimaryWeaponVisual, ref _currentWeaponIndex);
        InitializeWeapon(_secondaryWeaponList, ref _currentSecondaryWeapon, ref _currentSecondaryWeaponVisual, ref _currentSecondaryWeaponIndex);

        UpdateUI();
    }

    private void InitializePlayer()
    {
        _primaryCooldownTimer.StopTimerCoroutine();
        _secondaryCooldownTimer.StopTimerCoroutine();
    }
    private void OnEnable()
    {
        _eventManager.OnCharacterDestroyed += OnCharacterDie;
        _eventManager.OnGameSceneStart += InitializePlayer;

    }

    private void OnDisable()
    {
        _eventManager.OnCharacterDestroyed -= OnCharacterDie;
        _eventManager.OnGameSceneStart -= InitializePlayer;
    }



    private void OnCharacterDie(CharacterType characterType, Character character, Vector3 position)
    {
      //  this.enabled = false;
    }

  /*  private void InitializeWeaponLists()
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
            _currentSecondaryWeaponVisual = Instantiate(_currentSecondaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentSecondaryWeaponVisual.transform.localPosition = Vector3.zero;
            _currentSecondaryWeapon.InitializeWeapon();
        }
    

        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeapon, _currentPrimaryWeapon.name);
        _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, _currentPrimaryWeapon.CurrrentAmmo.ToString() + "/" + _currentPrimaryWeapon.CurrentMagazineSize.ToString());

    }  */
    private void InitializeWeapon(List<ProjectileWeapon> weaponList, ref ProjectileWeapon currentWeapon, ref GameObject currentVisuals, ref int currentIndex)
    {
        if (weaponList.Count > 0)
        {
            currentWeapon = weaponList[currentIndex];
            currentVisuals = Instantiate(currentWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            currentVisuals.transform.localPosition = Vector3.zero;
            currentWeapon.InitializeWeapon();
        }
    }

    private void UpdateUI()
    {
        if (_currentPrimaryWeapon != null)
        {
            _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeapon, _currentPrimaryWeapon.name);
            _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, GetAmmoText(_currentPrimaryWeapon));
        }

        if (_currentSecondaryWeapon != null)
        {
            _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeapon, _currentSecondaryWeapon.name);
            _eventManager.OnUIChange?.Invoke(UIElementType.SecondaryWeaponAmmo, GetAmmoText(_currentSecondaryWeapon));
        }
    }
    private string GetAmmoText(ProjectileWeapon weapon) => $"{weapon.CurrrentAmmo}/{weapon.CurrentMagazineSize}";



    //ADD NEW WEAPONS
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
        if (_secondaryWeaponList.Count == 1)
        {
            UpdateUI();
        }
    }





    //ADD MODS
    public void AddMod(WeaponMod addedMod)
    {
        if (addedMod == null) return;

        switch (addedMod.ModType)
        {
            case ModType.Barrel:
                AddModToList(addedMod, _barrelModsList, ref _currentBarrelMod, UIElementType.BarrelMod);
                break;

            case ModType.Sight:
                AddModToList(addedMod, _sightModsList, ref _currentSightMod, UIElementType.ScopeMod);
                break;

            case ModType.Grip:
                AddModToList(addedMod, _gripModsList, ref _currentGripMod, UIElementType.GripMod);
                break;
        }

        UpdateWeaponMods();
    }

    private void AddModToList(WeaponMod mod, List<WeaponMod> modList, ref WeaponMod currentMod, UIElementType uiElementType)
    {
        if (!modList.Contains(mod))
        {
            modList.Add(mod);
            if (modList.Count == 1)
            {
                currentMod = mod;
            }
            _eventManager.OnUIChange?.Invoke(uiElementType, modList.Count.ToString());
        }
    }

    private void UpdateWeaponMods()
    {
        ApplyWeaponMod(_currentGripMod, ref _currentGripVisual);
        ApplyWeaponMod(_currentBarrelMod, ref _currentBarrelVisual);
        ApplyWeaponMod(_currentSightMod, ref _currentSightVisual);
    }

    private void ApplyWeaponMod(WeaponMod mod, ref GameObject modVisual)
    {
        if (mod != null)
        {
            _currentPrimaryWeapon.ApplyMod(mod);
            if (modVisual != null)
            {
                Destroy(modVisual);
            }
            modVisual = Instantiate(mod.ModVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            modVisual.transform.localPosition = Vector3.zero;
            _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo, GetAmmoText(_currentPrimaryWeapon));
        }
    }



    //FIRE WEAPONS
    public void FirePrimaryWeapon()
    {
        if (_isReloading || _primaryCooldownTimer.IsRunningCoroutine || _currentPrimaryWeapon.CurrrentAmmo <= 0) return;
        PrimaryWeaponLogic(_currentPrimaryWeapon, ref _primaryWeaponSpawnPointIndex, _primaryCooldownTimer);
    }

    public void StopPrimaryWeapon()
    {
        _eventManager.OnWeaponStoped?.Invoke();
    }

    public void FireSecondaryWeapon()
    {
        if (_secondaryWeaponList.Count == 0 ||_secondaryCooldownTimer.IsRunningCoroutine || _currentSecondaryWeapon.CurrrentAmmo <=0  ) return;
        SecondaryWeaponLogic(_currentSecondaryWeapon, _secondaryWeaponSpawnPoint, _secondaryCooldownTimer);
    }

    private void PrimaryWeaponLogic(ProjectileWeapon weapon, ref int spawnPointIndex, Timer cooldownTimer)
    {
        Debug.Log("In primary weapon logic" + weapon.name);
        if (weapon == null)
        {
            Debug.LogWarning("Current weapon is null, cannot fire weapon.");
            return;
        }

        SetUpPrimaryWeapon(weapon.WeaponType);

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
        _eventManager.OnWeaponFired?.Invoke(_currentPrimaryWeapon.ShellCasingTag, _currentPrimaryWeapon.WeaponCooldown);
        _eventManager.OnPlaySoundEffect?.Invoke(weapon.name + "Effect", weaponSpawn.position);
        UpdateUI();

        cooldownTimer.StartTimerCoroutine();
    }

    private void SecondaryWeaponLogic(ProjectileWeapon weapon, Transform weaponSpawnPoint, Timer cooldownTimer)
    {
        if (weapon == null)
        {
            Debug.LogWarning("Current weapon is null, cannot fire weapon.");
            return;
        }

        SetUpSecondaryWeapon();

        weapon.UseWeapon(weaponSpawnPoint, _weaponTarget);
        _eventManager.OnPlaySoundEffect?.Invoke(weapon.name + "Effect", weaponSpawnPoint.position);
        cooldownTimer.StartTimerCoroutine();
        UpdateUI();
    }


    protected void SetUpPrimaryWeapon(string weaponType)
    {
        foreach (var weaponSpawn in _weaponSpawnPoints)
        {
            if (weaponSpawn.WeaponTypeTag == weaponType)
            {
                _currentWeaponSpawnPoints = weaponSpawn.SpawnLocations;
                PrimaryWeaponCooldown = _currentPrimaryWeapon.WeaponCooldown;
                return;
            }
        }
        Debug.LogWarning("No matching weapon type found for: " + weaponType);
    }

    protected void SetUpSecondaryWeapon()
    {
        SecondaryWeaponCooldown = _currentSecondaryWeapon.WeaponCooldown;
    }


    //GET SECONDARY AMMO
    public void AddSecondaryAmmo(ProjectileWeapon projectileWeapon, int secondaryAmmoAmmount)
    {
        if (projectileWeapon != null && _secondaryWeaponList.Contains(projectileWeapon))
        {
            ProjectileWeapon secondaryWeapon = _secondaryWeaponList.Find(weapon => weapon == projectileWeapon);

            if (secondaryWeapon != null)
            {
                secondaryWeapon.AddAmmo(secondaryAmmoAmmount);
                UpdateUI();
            }
        }
    }



    //WEAPON SWITCHING
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
        UpdateUI();
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
        UpdateUI();
    }


    //RELOAD PRIMARY WEAPON
    public void ReloadPrimaryWeapon()
    {
        if (_currentPrimaryWeapon.CurrrentAmmo < _currentPrimaryWeapon.CurrentMagazineSize)
        {
            _animator.SetTrigger("Reload");

            StartCoroutine(HandleReload(_currentPrimaryWeapon));
            _eventManager.OnPlaySoundEffect?.Invoke("Reload", transform.position);
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
            yield return null; 
        }

        _gunPositonAdjust.gunOffset = targetGunOffset;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float reloadDuration = stateInfo.length;

        yield return new WaitForSeconds(reloadDuration);

        weapon.Reload();

        elapsedTime = 0f;
        startOffset = Vector3.zero;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            _gunPositonAdjust.gunOffset = Vector3.Lerp(startOffset, currentGunOffset, t);
            elapsedTime += Time.deltaTime;
            yield return null;

            _gunPositonAdjust.gunOffset = currentGunOffset;

            _isReloading = false;


            _eventManager.OnUIChange?.Invoke(UIElementType.PrimaryWeaponAmmo,
                weapon.CurrrentAmmo.ToString() + "/" + weapon.CurrentMagazineSize.ToString());
        }
    }
}
