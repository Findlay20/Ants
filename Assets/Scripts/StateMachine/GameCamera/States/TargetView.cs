using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetView : GameCameraBaseState
{

    private GameCameraStateMachine camera;

    // TODO: Make this generic? So that target can be non-ants in futere 
    // maybe it would be in it's own state anyway - AntTopView.cs, BeetleTopView.cs, ...
    public AntStateMachine target;
    private InputActionMap inputActionMap;
    public InputAction cameraMove;

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
        target = context.target;
        inputActionMap = context.inputActions.FindActionMap("TargetView").Clone();

        cameraMove = inputActionMap.FindAction("cameraMove");
        inputActionMap.FindAction("switchView").performed += SwitchView;
    }

    public override void EnterState()
    {
        target = Context.target;
        Debug.Log("Entering Target View: " + target.gameObject.name);
        
        cameraRotation = Quaternion.LookRotation(camera.transform.up);
        Debug.Log("target transitioning");
        target.TransitionToState(AntStateMachine.EAntStates.Active);
        Debug.Log("target transitioned");

        inputActionMap.Enable();
        Debug.Log("inputActionMap enabled");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public override void ExitState()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        inputActionMap.Disable();
    }
    
    public override GameCameraStateMachine.ECameraStates GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other){}

    public override void OnTriggerExit(Collider other){}

    public override void OnTriggerStay(Collider other){}
       
    public void SwitchView(InputAction.CallbackContext context) {
        camera.TransitionToState(GameCameraStateMachine.ECameraStates.TopView);
    }

    public override void UpdateState()
    {

        Quaternion inputRotation = Quaternion.Euler(0, cameraMove.ReadValue<Vector2>().x, 0);
        cameraRotation *= inputRotation;

        cameraHeightOffset += cameraMove.ReadValue<Vector2>().y * heightChangeSensitivity;

        cameraHeightOffset = Mathf.Clamp(cameraHeightOffset, cameraMinHeight, cameraMaxHeight);

        camera.transform.position = target.transform.position +  cameraRotation * new Vector3(0, cameraHeightOffset, -cameraDistance);
        camera.gameCamera.transform.LookAt(target.transform.position + new Vector3(cameraFrameOffset.x, cameraFrameOffset.y));

    }

    public Quaternion CameraRotation => cameraRotation;

}