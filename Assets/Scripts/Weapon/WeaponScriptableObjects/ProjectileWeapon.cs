using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "ProjectileWeapon")]
public class ProjectileWeapon : Weapon
{
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] protected int _poolSize = 10;
    [SerializeField] protected string _targetTag;
    [SerializeField] protected MuzzleFlash _muzzleFlashPrefab;
    [SerializeField] protected Transform _projectileParent;

    //Base Weapon Stats
    [Range(0f, 1f)]
    [SerializeField] protected float _accuracy;
    [SerializeField] protected float _weaponSpeed;
    [SerializeField] protected int _weaponDamage;
    [SerializeField] protected float _weaponRange;
    [SerializeField] protected int _magazineSize;


    //Current Weapon Stats
    protected float _currentAccuracy;
    protected float _currentWeaponSpeed;
    protected int _currentWeaponDamage;
    protected float _currentWeaponRange;
    protected int _currentMagazineSize;

    private int _currentAmmo;
    private float _maxAngle = 45f;



    public override void InitializeWeapon()
    {
        _currentAmmo = _magazineSize;
        _currentAccuracy = _accuracy;
        _currentWeaponSpeed = _weaponSpeed;
        _currentWeaponDamage = _weaponDamage;
        _currentWeaponRange = _weaponRange;
        _currentMagazineSize = _magazineSize;
    }

    public override void UseWeapon(Transform spawn, string targetTag)
    {
        MuzzleFlash muzzleFlash = ObjectPoolSystem<MuzzleFlash>.Instance.GetObject(_muzzleFlashPrefab._poolTag);

        if (muzzleFlash == null)
        {
            ObjectPoolSystem<MuzzleFlash>.Instance.AddPool(_poolSize, _muzzleFlashPrefab._poolTag, _muzzleFlashPrefab, _projectileParent);
            muzzleFlash = ObjectPoolSystem<MuzzleFlash>.Instance.GetObject(_muzzleFlashPrefab._poolTag);

        }

        if (muzzleFlash != null)
        {
            muzzleFlash.transform.position = spawn.position;
            muzzleFlash.transform.rotation = spawn.rotation;
            muzzleFlash.SetBarrelPosition(spawn);
        }



        Projectile nextProjectile = ObjectPoolSystem<Projectile>.Instance.GetObject(_weaponObjectPoolTag);

        if (nextProjectile == null)
        {
            ObjectPoolSystem<Projectile>.Instance.AddPool(_poolSize, _weaponObjectPoolTag, _projectilePrefab, _projectileParent);
            nextProjectile = ObjectPoolSystem<Projectile>.Instance.GetObject(_weaponObjectPoolTag);
        }

 
        if (nextProjectile != null)
        {
            nextProjectile.transform.position = spawn.position;
            nextProjectile.transform.rotation = SetTrajectoryAngle(spawn.right, spawn.rotation);
            nextProjectile.SetupProjectile(_weaponDamage, _weaponSpeed, targetTag);
        }
    }


    public void ApplyMod(WeaponMod newMod)
    {
        AddMod(newMod);
    }

    public void ClearMods()
    {
        InitializeWeapon();
    }

    public void Reload()
    {
        _currentAmmo = _currentMagazineSize;
    }

    private Quaternion SetTrajectoryAngle(Vector2 direction, Quaternion rotation)
    {
        float accuracyRange = Mathf.Lerp(_maxAngle, 0, _accuracy);
        float randomAngle = UnityEngine.Random.Range(-accuracyRange, accuracyRange);
        Quaternion projectileRotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        return rotation * projectileRotation;
    }

    private void AddMod(WeaponMod mod)
    {

        _currentWeaponDamage += mod.DamageModifier;

        _currentWeaponCooldown -= mod.FireRateModifier;

        _currentAccuracy += mod.AccuracyModifier;
        _currentAccuracy = Mathf.Clamp(_accuracy, 0f, 1f);

        _currentMagazineSize += mod.MagazineSizeModifier;

        _currentWeaponRange += mod.RangeModifier;

        _currentWeaponSpeed += mod.SpeedModifier;

        DisplayCurrentStatsDebug();
     
    }
  

    private void DisplayCurrentStatsDebug()
    {
      //  Debug.Log($"CUrrent stats/n Damage {_currentWeaponDamage}/n Cooldown {_currentWeaponCooldown}/n Accuracy {_currentAccuracy}/n Magazine {_currentMagazineSize}/n Range {_currentWeaponRange} Speed {_currentWeaponSpeed}");
    }
}
