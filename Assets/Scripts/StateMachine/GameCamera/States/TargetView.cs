using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetView : GameCameraBaseState
{

    private GameCameraStateMachine camera;
    private Gamepad controller;

    // TODO: Make this generic? So that target can be non-ants in futere 
    // maybe it would be in it's own state any - AntTopView.cs, BeetleTopView.cs, ...
    public AntStateMachine target;

    [SerializeField] Transform followTarget;
    [SerializeField] float cameraDistance = 1;
    [SerializeField] float cameraMinHeight = 0.1f;
    [SerializeField] float cameraMaxHeight = 1f;
    [SerializeField] Vector2 cameraFrameOffset = new Vector2(0, 0.2f);



    public float heightChangeSensitivity = 0.02f;
    public Vector3 cameraOffset = new Vector3(0, 0.36f, -1);
    public Quaternion cameraRotation = Quaternion.Euler(0, 0, 0);
    private float cameraHeightOffset = 0.36f;

    public TargetView(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(context, stateKey)
    {
        camera = context.cameraStateMachine;
        controller = context.controller;
        target = context.target;

    }

    public override void EnterState()
    {
        target = Context.target;
        Debug.Log("Entering Target View: " + target.gameObject.name);
        
        cameraRotation = Quaternion.LookRotation(target.transform.forward);
        target.TransitionToState(AntStateMachine.EAntStates.Active);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public override void ExitState()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    public override GameCameraStateMachine.ECameraStates GetNextState()
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
        if (controller.yButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Tab)){
            camera.TransitionToState(GameCameraStateMachine.ECameraStates.TopView);
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion inputRotation = Quaternion.Euler(0, controller.rightStick.x.ReadValue() + mouseX, 0);
        cameraRotation *= inputRotation;

        float inputHeightOffset = controller.rightStick.y.ReadValue() + mouseY;
        cameraHeightOffset += inputHeightOffset * heightChangeSensitivity;

        cameraHeightOffset = Mathf.Clamp(cameraHeightOffset, cameraMinHeight, cameraMaxHeight);

        camera.transform.position = target.transform.position +  cameraRotation * new Vector3(0, cameraHeightOffset, -cameraDistance);
        camera.gameCamera.transform.LookAt(target.transform.position + new Vector3(cameraFrameOffset.x, cameraFrameOffset.y));

    }

    public Quaternion CameraRotation => cameraRotation;

}