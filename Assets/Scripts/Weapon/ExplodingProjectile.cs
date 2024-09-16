using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingProjectile : Projectile
{
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private int _aoeDamage = 10;

    [SerializeField] private bool useGravity = true; 
    [SerializeField] private float gravityScale = 1f; 

    private void Start()
    {
        if (rb != null && useGravity)
        {
            rb.gravityScale = gravityScale;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {

        Vector3 weaponHit = collision.contacts[0].point;

        if (( 1 << ((collision.gameObject.layer) & _damageLayer) != 0))
        {
            Collider2D[] affectedColliders = Physics2D.OverlapCircleAll(weaponHit, _explosionRadius, _damageLayer);

            foreach (Collider2D collider in affectedColliders)
            {
                if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.DamageTaken(_aoeDamage);
                }
            }
        }

        _eventManager.OnPlayParticleEffect?.Invoke(_projectilePoolTag, (Vector2)weaponHit, _particleEffectScale);
        _eventManager.OnPlaySoundEffect?.Invoke(_audioClipTag, (Vector2)weaponHit);

        this.gameObject.SetActive(false);
    }

    public override void SetupProjectile(int damage, float projectileSpeed, string targetTag)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            Debug.LogWarning("Target tag is null or empty, using default tag 'Untagged'.");
            targetTag = "Enemy"; 
        }

        base.SetupProjectile(damage, projectileSpeed, targetTag);
        rb.gravityScale = useGravity ? gravityScale : 0f;
    }
}
