using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    private int _currentHealth;
    private int _startingHealth;
    private int _maxHealth;
    private int _minHealth;

    private bool _isDestroyed;
  
    public int CurrentHealth 
    {  
        get { return _currentHealth; }
        set { _currentHealth = value;}
    }
    public int StartingHealth
    {
        get { return _startingHealth; }
        set { 
            _startingHealth = value;
            _isDestroyed = false;
        }
    }
    public int MaxHealth 
    { 
        get {return _maxHealth; }
        set
        {
            if (value > _minHealth)
            {
                _maxHealth = value;
            }
        }
    }

    public int MinHealth 
    {
        get { return _minHealth; }
        set { _minHealth = value; }
    }
    
    public bool IsDestroyed
    {
        get { return _isDestroyed; }
        set { _isDestroyed = value; }
    }

    public int DecreaseHealth()
    {
       return DecreaseHealth(1);
    }

    public int DecreaseHealth(int health)
    {
        _currentHealth -= health;
        if(_currentHealth <= _minHealth)
        {
            _currentHealth = _minHealth;
            _isDestroyed = true;
        }
        return _currentHealth;
    }

    public int IncreaseHealth()
    {
        return IncreaseHealth(1);
    }

    public int IncreaseHealth(int health) 
    { 
        _currentHealth += health;
        if(_currentHealth >= _maxHealth)
        {
            _currentHealth = MaxHealth;
        }

        return _currentHealth;
    }

    public void ResetHealth()
    {
        _currentHealth = StartingHealth;
        _isDestroyed = false;
    }

}
