using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class GameCameraStateMachine : StateManager<GameCameraStateMachine.ECameraStates> {
    

    public Camera gameCamera;
    public MapGenerator mapGenerator;
    public InputActionAsset inputActions;

    public enum ECameraStates {
        TopView,
        TargetView
    }

    private GameCameraContext _context;


    void Awake() {
        mapGenerator = FindFirstObjectByType<MapGenerator>();
        mapGenerator.drawMode = MapGenerator.DrawMode.Mesh;
        mapGenerator.DrawMapInEditior();

        gameCamera = GetComponent<Camera>();
        _context = new GameCameraContext(this, gameCamera, inputActions);
        InitiliaseStates();
    }


    private void InitiliaseStates() {
        
        States.Add(ECameraStates.TopView, new TopView(_context, ECameraStates.TopView));
        States.Add(ECameraStates.TargetView, new TargetView(_context, ECameraStates.TargetView));
    
        CurrentState = States[ECameraStates.TopView];

    }

}