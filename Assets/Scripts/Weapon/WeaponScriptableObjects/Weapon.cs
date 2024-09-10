using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    [Header("Weapon Setup")]
    [SerializeField] protected GameObject weaponVisual;
    [SerializeField] protected string _weaponObjectPoolTag;
    [SerializeField] protected float _weaponCooldown;
    [SerializeField] protected float _currentWeaponCooldown;
    public string WeaponType
    {
        get { return _weaponObjectPoolTag; }
    }

    public float WeaponCooldown
    {
        get { return _weaponCooldown; }
    }

    public GameObject WeaponVisual
    {
        get { return weaponVisual; }
    }


    public abstract void InitializeWeapon();

    public abstract void UseWeapon(Transform spawn, string targetTag);
}
