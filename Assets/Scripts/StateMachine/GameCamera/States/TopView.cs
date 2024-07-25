using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class TopView : GameCameraBaseState
{

    private StateManager<GameCameraStateMachine.ECameraStates> cameraStateMachine;
    public override CinemachineVirtualCamera virtualCamera => cameraStateMachine.gameObject.transform.Find("Top Down Follow Camera").GetComponent<CinemachineVirtualCamera>();

    public AntStateMachine target;
    private InputActionMap inputActionMap;
    public InputAction zoomValue;
    public InputAction cameraRotate;
    public InputAction zoomKey;


    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float zoomSensitivity = 0.1f;
    private Quaternion currentRotation = Quaternion.Euler(90, 0, 0);
    private Vector2 cameraPositionOffset = Vector2.zero;
    private float currentHeight = 10f;
    
    private float minZoom = 3.5f;
    private float maxZoom = 20f;


    public TopView(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(context, stateKey)
    {
        target = Context.target;
        cameraStateMachine = context.cameraStateMachine;
        currentHeight = context.TopViewCameraHeight;
        inputActionMap = context.inputActions.FindActionMap("TopView").Clone();

        zoomValue = inputActionMap.FindAction("zoomValue");
        cameraRotate = inputActionMap.FindAction("cameraRotate");
        inputActionMap.FindAction("switchView").performed += SwitchView;
        inputActionMap.FindAction("select").performed += SelectTarget;
        zoomKey = inputActionMap.FindAction("zoomKey");
    }

    public override void EnterState()
    {
        virtualCamera.Priority = 1;

        inputActionMap.Enable();
        //Vector3 topViewPos = new Vector3(0, currentHeight, 0) ;

        // if (target) {
        Vector3 oldCameraFwd = cameraStateMachine.transform.forward;
        //if (target) topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);

        oldCameraFwd.y = 0;
        oldCameraFwd.Normalize();

        //currentRotation = Quaternion.LookRotation(Vector3.down, oldCameraFwd);
        //cameraStateMachine.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // } else {
        //     camera.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // }
        
        
        if (target) {
            target.TransitionToState(AntStateMachine.EAntStates.TopDownActive);
            virtualCamera.Follow = target.transform;
        }

        Debug.Log("Entering Top View: " + (target ? target.gameObject.name : ""));
        Debug.Log("Switching to camera: " + virtualCamera);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

    }

    public override void ExitState()
    {
        inputActionMap.Disable();         
        virtualCamera.Priority = 0;
    }
    
    public override GameCameraStateMachine.ECameraStates GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other){}

    public override void OnTriggerExit(Collider other){}

    public override void OnTriggerStay(Collider other){}
       
    public void SwitchView(InputAction.CallbackContext context) {
        if (!target) return;
        cameraStateMachine.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
    }

    public override void UpdateState()
    {

        // Rotate camera
        //UpdateRotation();
        if (zoomKey.IsPressed()){
            UpdateZoom();
        } else {
            // Move to active ant if moving else move with cursor
            //UpdatePosition();
        }

    }

    public void UpdatePosition() {
            // Instead need to move cursor and when near edge of window trigger camera move
            //cameraPositionOffset = cameraMove.ReadValue<Vector2>();
            Vector3 topViewPos = new Vector3(cameraStateMachine.transform.position.x + cameraPositionOffset.x, currentHeight, cameraStateMachine.transform.position.z + cameraPositionOffset.y);

            cameraStateMachine.transform.SetPositionAndRotation(topViewPos, currentRotation);
    }

    public void SetPositionToTarget() {
            Vector3 topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);
            cameraStateMachine.transform.SetPositionAndRotation(topViewPos, currentRotation);
    }

    private void UpdateRotation() {
        Quaternion inputRotation = Quaternion.Euler(0, 0, cameraRotate.ReadValue<float>());
        currentRotation *= inputRotation;

        cameraStateMachine.transform.rotation = Quaternion.RotateTowards(cameraStateMachine.transform.rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateZoom() {
        float heightChange = zoomValue.ReadValue<float>();
        heightChange *= zoomSensitivity;

        currentHeight += heightChange;
        currentHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);
        virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(0, 0, -currentHeight);
        
    }

    private void SelectTarget(InputAction.CallbackContext context) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.GetComponent<AntStateMachine>()){
                if (target) target.TransitionToState(AntStateMachine.EAntStates.Idle);
                target = hit.collider.GetComponent<AntStateMachine>();
                
                Context.target = target;
                Debug.Log("New target: " + target.name);
                
                cameraStateMachine.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
            }
        };
    }
}