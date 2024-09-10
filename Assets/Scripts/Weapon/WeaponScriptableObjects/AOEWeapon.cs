using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AOEWeapon")]
public class AOEWeapon : Weapon
{
    [SerializeField] protected int _weaponDamage = 20;
    [SerializeField] protected float explosionRadius = 5f;
    [SerializeField] private float _particleEffectScale = 1f;
    [SerializeField] LayerMask _targetLayer;

    private EventManager _eventManager;
    private string _targetTag;

    bool _isFired = false;



    public override void InitializeWeapon()
    {
        if(_eventManager == null)
        {
            _eventManager = EventManager.Instance;
        }
    }

    public override void UseWeapon(Transform spawn, string targetTag)
    {
        Debug.Log("In use weapon in AOE weapon");
        _targetTag = targetTag;
        SelfDestruct(spawn.position);

       // _eventManager.OnPlayParticleEffect?.Invoke("SelfDestruct", spawn.position , _particleEffectScale);
    }

    private void SelfDestruct(Vector2 position)
    {
            Debug.Log("In melee use weapon");
            var hits = Physics2D.CircleCastAll(position, explosionRadius, Vector2.right, 0f);

            for (int i = 0; i < hits.Length; i++)
            {
                IDamagable damagable = hits[i].collider.gameObject.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    damagable.DamageTaken(_weaponDamage);
                }
     
            }
    }

}
