using UnityEngine;
using UnityEngine.InputSystem;

public class GameCameraContext {

    public Camera gameCamera;
    public GameCameraStateMachine cameraStateMachine;
    public AntStateMachine target;
    public InputActionAsset inputActions;
    public GameObject cameraTarget;


    // Settings like move speed, rotate etc
    [Header("Camera Settings")]
    public float topViewMoveSpeed = 5f;
    public float TopViewCameraHeight = 10f;

    public GameCameraContext(GameCameraStateMachine stateManager, Camera GameCamera, InputActionAsset inputActions) {
        gameCamera = GameCamera;
        cameraStateMachine = stateManager;
        this.inputActions = inputActions; 
        cameraTarget = cameraStateMachine.GetComponentInChildren<GameObject>();
    }

}