using UnityEngine;
using UnityEngine.InputSystem;

public class AntContext {

    // TODO: Grab the rb compoinent somewhere in code to avoid assigning in editor
    public Rigidbody rb;
    public AntStateMachine antStateMachine;

    public GameCameraStateMachine camera;
    public InputActionAsset inputActions;

    //TODO: Rework movement
    [Header("Moverment settings")]
    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float sidewaysSpeed = 0.2f;
    public float sidewaysSprintSpeed = 0.6f;
    public float jumpForce = 0.1f;
    public float stickToGroundForce = 1f;
    public float turningSpeed = 3f;
    public float sprintTurningSpeed = 5f;
    public float weight = 2f;

    public int score;

    public AntContext(AntStateMachine stateManager, InputActionAsset inputActions) {
        rb = stateManager.GetComponentInParent<Rigidbody>();
        antStateMachine = stateManager;
        camera = Camera.main.GetComponent<GameCameraStateMachine>();
        this.inputActions = inputActions;
    }

}