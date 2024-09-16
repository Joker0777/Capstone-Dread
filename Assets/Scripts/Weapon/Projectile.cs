using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float _projectileTimerLength = 6f;
    [SerializeField] protected float _particleEffectScale = 1f;
    [SerializeField] protected LayerMask _damageLayer;
    [SerializeField] protected string _ignoreLayer;
    [SerializeField] protected string _audioClipTag;
    [SerializeField] protected string _projectilePoolTag;

    protected float _projectileSpeed;
    protected int _projectileDamage;
    protected string _targetTag;
    protected EventManager _eventManager;

    protected Timer _projectileTimer;
    protected bool _fired;

    protected Rigidbody2D rb;


    private void Awake()
    {
        _projectileTimer = new Timer(_projectileTimerLength);
        _eventManager = EventManager.Instance;
        rb = GetComponent<Rigidbody2D>();
    }


    protected virtual void Update()
    {

        if (_fired)
        {
            _projectileTimer.UpdateTimerBasic(Time.deltaTime);
            
            if (!_projectileTimer.IsRunningBasic())
            {
                _fired = false;
                this.gameObject.SetActive(false);
            }
          
        }    
    }

    private void ShootProjectile()
    {
        rb.AddForce(transform.up * _projectileSpeed);
    }


    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 weaponHit = collision.contacts[0].point;

        Rigidbody2D rb = collision.collider?.attachedRigidbody;

        if (_damageLayer == (_damageLayer | (1 << collision.gameObject.layer)))
        {
            collision.collider?.attachedRigidbody?.GetComponent<IDamagable>()?.DamageTaken(_projectileDamage);

            _eventManager.OnPlayParticleEffect?.Invoke(_projectilePoolTag, (Vector2)weaponHit, _particleEffectScale);
            _eventManager.OnPlaySoundEffect?.Invoke(_audioClipTag, (Vector2)weaponHit);

            this.gameObject.SetActive(false);
        }
    }

 
    public virtual void SetupProjectile(int damage, float projectileSpeed, string targetTag)
    {
        _projectileSpeed = projectileSpeed;
        _projectileDamage = damage;
        _targetTag = targetTag;

        _projectileTimer.TimerDuration = _projectileTimerLength;
        _projectileTimer.StartTimerBasic();

        ShootProjectile();

        _fired = true;
    }
}
