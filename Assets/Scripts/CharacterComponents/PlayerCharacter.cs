using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    protected override void Start()
    {
        base.Start();

        _eventManager.OnUIChange?.Invoke(UIElementType.Health, GetHealth().ToString());
    }

    public override void DamageTaken(int damage)
    {
        base.DamageTaken(damage);

        _eventManager.OnUIChange?.Invoke(UIElementType.Health, GetHealth().ToString());
        _eventManager.OnUnitHealthChanged?.Invoke(_healthSystem.CurrentHealth);
    }

    public override void HealthIncrease(int health)
    {
        base.HealthIncrease(health);

        _eventManager.OnUIChange?.Invoke(UIElementType.Health, GetHealth().ToString());
        _eventManager.OnUnitHealthChanged?.Invoke(_healthSystem.CurrentHealth);
    }
}
