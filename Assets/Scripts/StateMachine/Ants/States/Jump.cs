using UnityEngine;

public class Jump: AntBaseState 
{
    
    public Jump(AntContext context, AntStateMachine.EAntStates estate) : base(context, estate) 
    {
        AntContext Context = context;
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