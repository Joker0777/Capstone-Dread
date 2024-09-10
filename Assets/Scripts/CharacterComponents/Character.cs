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
    [SerializeField] protected int __impactDamage = 10;
    [SerializeField] string _particleEffectTag;
    [SerializeField] string _soundEffectTag;
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
        currentHealth = _healthSystem.CurrentHealth;
        if (_healthSystem.IsDestroyed)
        {
            if (!_isDead)
            {
                this.gameObject.layer = LayerMask.NameToLayer(_deadLayer);
                _isDead = true;
                _eventManager.OnPlayParticleEffect?.Invoke(CharacterType.ToString(), transform.position, 1);
                _eventManager.OnCharacterDestroyed?.Invoke(CharacterType, this, transform.position);
                _eventManager.OnPlaySoundEffect?.Invoke("ShipEffect", transform.position);
            }
        }
    }


    public virtual void DamageTaken(int damage)
    {
        if (!_isDead && !_healthSystem.IsDestroyed)
        {
            _healthSystem.DecreaseHealth(damage);
            _eventManager.OnCharacterHurt?.Invoke(CharacterType, this, transform.position);
            Debug.Log("Damage taken. Current Health " + _healthSystem.CurrentHealth);
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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 hitPoint = collision.contacts[0].point;
    //    if (!collision.collider.gameObject.CompareTag(_collisionIgnore))
      //  {
            collision.collider?.attachedRigidbody?.GetComponent<IDamagable>()?.DamageTaken(__impactDamage);
      //  }

        _eventManager.OnPlayParticleEffect?.Invoke(_particleEffectTag, (Vector2)hitPoint, 1f);
        _eventManager.OnPlaySoundEffect?.Invoke(_soundEffectTag, (Vector2)hitPoint);
    }

}
