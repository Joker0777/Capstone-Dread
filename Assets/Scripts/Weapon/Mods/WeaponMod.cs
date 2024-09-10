using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "WeaponMod")]
public class WeaponMod : Mod
{
    [SerializeField] private int damageModifier;
    [SerializeField] private float fireRateModifier;
    [Range(0f, 1f)]
    [SerializeField] private float accuracyModifier;
    [SerializeField] private int magazineSizeModifier;
    [SerializeField] private float rangeModifier;
    [SerializeField] private float speedModifier;

    public int DamageModifier { get => damageModifier; }
    public float FireRateModifier { get => fireRateModifier; }
    public float AccuracyModifier { get => accuracyModifier; }
    public int MagazineSizeModifier { get => magazineSizeModifier; }
    public float RangeModifier { get => rangeModifier; }

    public float SpeedModifier { get => speedModifier; }


}
