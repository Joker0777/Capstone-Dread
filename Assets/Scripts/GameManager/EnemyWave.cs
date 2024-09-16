using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class EnemyWave
{
    [SerializeField] private List<EnemyCharacter> _enemies = new List<EnemyCharacter>();

    public List<EnemyCharacter> GetEnemyUnits()
    {
        return new List<EnemyCharacter>(_enemies);
    }
}
