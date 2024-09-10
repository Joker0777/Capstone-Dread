using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIState
{
    protected AIController Controller;

    public abstract void OnStateEnter();
    public abstract void OnStateExit();
    public abstract void OnStateRun();

    public AIState(AIController controller)
    {
        Controller = controller;
    }
}
