using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AmmoMod")]
public class AmmoMod : WeaponMod
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private string ammoPoolTag;

    [SerializeField] private float damageRadius;
    [SerializeField] private bool pireceProjectile;
    [SerializeField] private int projectilesOnFire;
    [SerializeField] private int projectilesOnHit;

    public Projectile Projectile { get => projectile; set => projectile = value; }
    public string AmmoPoolTag { get => ammoPoolTag; set => ammoPoolTag = value; }
    public float DamageRadius { get => damageRadius; set => damageRadius = value; }
    public bool PireceProjectile { get => pireceProjectile; set => pireceProjectile = value; }
    public int ProjectilesOnFire { get => projectilesOnFire; set => projectilesOnFire = value; }
    public int ProjectilesOnHit { get => projectilesOnHit; set => projectilesOnHit = value; }
}
