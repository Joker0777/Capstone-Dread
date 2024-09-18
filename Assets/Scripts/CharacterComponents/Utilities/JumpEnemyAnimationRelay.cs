using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpEnemyAnimationRelay : MonoBehaviour
{
    [SerializeField] JumpingEnemyBehaviourSystem behaviourScript;


    public void SetAttack()
    {
        behaviourScript.FireWeapon();
    }
}
