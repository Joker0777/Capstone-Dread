using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponSystem : WeaponSystem
{

    [SerializeField] protected Transform _weaponVisualTransform;
    [SerializeField] protected Transform _projectileParent;


    protected GameObject _currentPrimaryWeaponVisual;


    protected override void InitializeWeaponList()
    {
        if (_primaryWeaponList.Count > 0)
        {
            _currentPrimaryWeapon = _primaryWeaponList[_currentWeaponIndex];
            _currentPrimaryWeaponVisual = Instantiate(_currentPrimaryWeapon.WeaponVisual, _weaponVisualTransform.position, _weaponVisualTransform.rotation, _weaponVisualTransform);
            _currentPrimaryWeaponVisual.transform.localPosition = Vector3.zero;
        }

    }

    protected override void UseWeaponLogic(Weapon weapon, ref int spawnPointIndex, Timer cooldownTimer)
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


    public void SwitchWeapon(int switchDirection, List<Weapon> weaponList, ref Weapon currentWeapon, ref int currentIndex)
    {
        if (weaponList.Count == 0) return;

        currentIndex = (currentIndex + switchDirection + weaponList.Count) % weaponList.Count;
        currentWeapon = weaponList[currentIndex];
        currentWeapon.InitializeWeapon();
    }
}
