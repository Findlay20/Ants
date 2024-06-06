using UnityEngine;
using UnityEngine.InputSystem;

public class GameCameraContext {

    public Camera gameCamera;
    public GameCameraStateMachine cameraStateMachine;
    public Gamepad controller;

    public AntStateMachine target;



    // Settings like move speed, rotate etc
    [Header("Camera Settings")]
    public float topViewMoveSpeed = 5f;
    public float TopViewCameraHeight = 10f;

    public GameCameraContext(GameCameraStateMachine stateManager, Camera GameCamera) {
        gameCamera = GameCamera;
        cameraStateMachine = stateManager;
        controller = Gamepad.current;
    }

}