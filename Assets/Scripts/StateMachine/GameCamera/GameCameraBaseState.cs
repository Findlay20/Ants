using UnityEngine;
using System;
using Cinemachine;

public abstract class GameCameraBaseState : BaseState<GameCameraStateMachine.ECameraStates>
{

    public GameCameraContext Context;
    public abstract CinemachineVirtualCamera virtualCamera {
        get;
    }

    public GameCameraBaseState(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(stateKey){
        Context = context;
    }

}
 