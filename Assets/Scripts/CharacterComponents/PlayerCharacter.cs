using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    protected override void Start()
    {
        base.Start();
        _eventManager.OnGameSceneStart += UpdateHealthUI;

        UpdateHealthUI();
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        _eventManager.OnGameSceneStart -= UpdateHealthUI;
    }

    private void UpdateHealthUI()
    {
        _eventManager.OnUIChange?.Invoke(UIElementType.Health, GetHealth().ToString());
    }


    public override void DamageTaken(int damage)
    {
        base.DamageTaken(damage);

        UpdateHealthUI();
        _eventManager.OnUnitHealthChanged?.Invoke(_healthSystem.CurrentHealth);
    }

    public override void HealthIncrease(int health)
    {
        base.HealthIncrease(health);

        UpdateHealthUI();
        _eventManager.OnUnitHealthChanged?.Invoke(_healthSystem.CurrentHealth);
    }
}
