using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    [SerializeField] FlyingENemyBehaviourScript behaviourScript;
    

    public void SetAttack()
    {
        behaviourScript.FireWeapon();
    }
}
