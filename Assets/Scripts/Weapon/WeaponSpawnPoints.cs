using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponSpawnPoints
{ 
    [SerializeField] Transform[] spawnLocations;
    [SerializeField] string weaponTypeTag;

    public string WeaponTypeTag{ get {  return weaponTypeTag; } }

    public Transform[] SpawnLocations { get { return spawnLocations; } }
}
