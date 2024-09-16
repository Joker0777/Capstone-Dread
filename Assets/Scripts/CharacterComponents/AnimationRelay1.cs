using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRelay1 : MonoBehaviour
{
    [SerializeField] EnemyBehaviourSystem behaviourScript;


    public void SetAttack()
    {
        behaviourScript.FireWeapon();
    }
}
