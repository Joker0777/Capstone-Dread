using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CloseRangeEnemyWeapon")]
public class MeleeWeapon : Weapon
{
    [Header("Attack Setup")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 1f; 
    [SerializeField] private LayerMask targetLayer;    

    private EventManager _eventManager;

    public override void InitializeWeapon()
    {
        _eventManager = EventManager.Instance;
    }

    public override void UseWeapon(Transform spawn, string targetTag)
    {

       
        var hits = Physics2D.CircleCastAll(spawn.position, attackRange,spawn.right, 0f,  targetLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            IDamagable damagable = hits[i].collider.gameObject.GetComponent<IDamagable>();

            if (damagable != null)
            {
                damagable.DamageTaken(damage);
            }
        }    
    }


}
