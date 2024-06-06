using System;
using UnityEngine;

public class AntStateMachine : StateManager<AntStateMachine.EAntStates> 
{
    public Rigidbody rb;
    public float baseDmg = 10;


    public enum EAntStates {
        Idle,
        Active,
        TopDownActive
    }

    private AntContext _context;


    // public AntStateMachine() {}

    void Awake() {
        gameObject.tag = Tags.Player;
        InitialiseRigidBody();
        _context = new AntContext(this);
        InitiliaseStates();
    }

    private void InitialiseRigidBody()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void InitiliaseStates()
    {
        //Add states to inherited StateManager "States" dictionary and Set Initial State
        States.Add(EAntStates.Idle, new Idle(_context, EAntStates.Idle));
        States.Add(EAntStates.Active, new Active(_context, EAntStates.Active));
        States.Add(EAntStates.TopDownActive, new TopDownActive(_context, EAntStates.TopDownActive));

        CurrentState = States[EAntStates.Idle];
    }



}