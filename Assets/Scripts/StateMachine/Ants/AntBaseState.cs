using UnityEngine;
using System;


public abstract class AntBaseState : BaseState<AntStateMachine.EAntStates>
{

    protected AntContext Context;

    public AntBaseState(AntContext context, AntStateMachine.EAntStates stateKey) : base(stateKey){
        Context = context;
    }

}
 