using UnityEngine;
using System;


public abstract class GameCameraBaseState : BaseState<GameCameraStateMachine.ECameraStates>
{

    public GameCameraContext Context;

    public GameCameraBaseState(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(stateKey){
        Context = context;
    }

}
 