using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeState : AIState
{

    private Transform target;
    public EvadeState(AIController controller, Transform target) : base(controller)
    {
        this.target = target;
    }

    public override void OnStateEnter()
    {
       
    }

    public override void OnStateExit()
    {
      
    }

    public override void OnStateRun()
    {
      
    }

  
}
