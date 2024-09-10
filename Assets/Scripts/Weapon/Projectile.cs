using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _projectileTimerLength = 6f;
    [SerializeField] float _particleEffectScale = 1f;
    [SerializeField] LayerMask _damageLayer;
    [SerializeField] string _ignoreLayer;
    [SerializeField] string _audioClipTag;

    protected float _projectileSpeed;
    protected int _projectileDamage;
    protected string _targetTag;
    private EventManager _eventManager;

    private Timer _projectileTimer;
    private bool _fired;

    private Rigidbody2D rb;


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


        if (((collision.gameObject.CompareTag(_targetTag) || 1 << ((collision.gameObject.layer) & _damageLayer) != 0)))
        {
            collision.collider?.attachedRigidbody?.GetComponent<IDamagable>().DamageTaken(_projectileDamage);

        }


        this.gameObject.SetActive(false);

        _eventManager.OnPlayParticleEffect?.Invoke("Projectile", (Vector2)weaponHit,_particleEffectScale);
        _eventManager.OnPlaySoundEffect?.Invoke(_audioClipTag, (Vector2)weaponHit);
    }

 
    public void SetupProjectile(int damage, float projectileSpeed, string targetTag)
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
