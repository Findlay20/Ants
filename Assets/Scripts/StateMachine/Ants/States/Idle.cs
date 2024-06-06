using UnityEngine;

public class Idle: AntBaseState 
{
    private StateManager<AntStateMachine.EAntStates> Ant;

    public Idle(AntContext context, AntStateMachine.EAntStates estate) : base(context, estate) 
    {
        AntContext Context = context;
        Ant = context.antStateMachine;
    }

    public override void EnterState()
    {

    }


    public override void ExitState()
    {

    }

    public override AntStateMachine.EAntStates GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other)
    {
       
    }

    public override void OnTriggerExit(Collider other)
    {
 
    }

    public override void OnTriggerStay(Collider other)
    {
        
    }


    public override void UpdateState()
    {
        
    }
}