using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Character : MonoBehaviour, IDamagable
{
    protected HealthSystem _healthSystem;

    protected EventManager _eventManager;
    [SerializeField] protected CharacterType _characterType;
    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] string _particleEffectTag;
    [SerializeField] string _deathSoundEffectTag;
    [SerializeField] string _hurtSoundEffectTag;
    [SerializeField] string _collisionIgnore;
    [SerializeField] string _deadLayer;
    [SerializeField] Animator _animator;
    public int currentHealth;

    private bool _isHurt;
    private bool _isDead;

    public CharacterType CharacterType { get { return _characterType; } }

    public EventManager EventManager { get { return _eventManager; } }

    protected virtual void Awake()
    {
        _eventManager = EventManager.Instance;
    }

    protected virtual void Start()
    {
        _healthSystem = new HealthSystem();
        _healthSystem.CurrentHealth = _maxHealth;
        _healthSystem.MaxHealth = _maxHealth;



    }

    protected void Update()
    {
        if (_eventManager == null)
        {
            _eventManager = EventManager.Instance;
        }

        currentHealth = _healthSystem.CurrentHealth;
        if (_healthSystem.IsDestroyed)
        {
            if (!_isDead)
            {
                this.gameObject.layer = LayerMask.NameToLayer(_deadLayer);
                _isDead = true;
                _eventManager.OnPlayParticleEffect?.Invoke(_particleEffectTag, transform.position, 1);
                _eventManager.OnCharacterDestroyed?.Invoke(CharacterType, this, transform.position);
                _eventManager.OnPlaySoundEffect?.Invoke(_deathSoundEffectTag, transform.position);

                Debug.Log("In health system character destroyed ");
            }
        }
    }


    public virtual void DamageTaken(int damage)
    {
  
    
        if (!_isDead && !_healthSystem.IsDestroyed)
        {
            _healthSystem.DecreaseHealth(damage);
            _eventManager.OnCharacterHurt?.Invoke(CharacterType, this, transform.position);
            _eventManager.OnPlaySoundEffect?.Invoke(_hurtSoundEffectTag, transform.position);
           // Debug.Log("Damage taken. Current Health " + _healthSystem.CurrentHealth);
        }
    }

    public virtual void HealthIncrease(int health)
    {
        _healthSystem.IncreaseHealth(health);
    }

    public virtual void ResetHealth(int health)
    {
        _healthSystem.CurrentHealth = health;
    }

    public int GetHealth()
    {
        return _healthSystem.CurrentHealth;
    }

}
