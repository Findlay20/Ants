using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class TopView : GameCameraBaseState
{

    private GameCameraStateMachine cameraStateMachine;
    public override CinemachineVirtualCamera virtualCamera => cameraStateMachine.gameObject.transform.Find("Top Down Follow Camera").GetComponent<CinemachineVirtualCamera>();

    public AntStateMachine target;
    private InputActionMap inputActionMap;
    public InputAction zoomValue;
    public InputAction cameraRotate;
    public InputAction zoomKey;

    private List<float> zoomHeights = new List<float> {5f, 10f, 20f, 100f};
    private int zoomSelected = 0;

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
        inputActionMap.FindAction("zoomKey").performed += SwitchZoom;
    }

    public override void EnterState()
    {
        Vector3 oldCameraFwd = cameraStateMachine.gameCamera.transform.forward;
        virtualCamera.Priority = 1;

        inputActionMap.Enable();
        //Vector3 topViewPos = new Vector3(0, currentHeight, 0) ;

        // if (target) {
        
        //if (target) topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);

        oldCameraFwd.y = 0;
        oldCameraFwd.Normalize();

        currentRotation = Quaternion.LookRotation(Vector3.down, oldCameraFwd);
        //cameraStateMachine.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // } else {
        //     camera.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // }
        virtualCamera.transform.rotation = currentRotation;
        
        
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
        UpdateRotation();

    }

    private void UpdateRotation() {
        Quaternion inputRotation = Quaternion.Euler(0, 0, cameraRotate.ReadValue<float>());
        currentRotation *= inputRotation;

        virtualCamera.transform.rotation = Quaternion.RotateTowards(virtualCamera.transform.rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void SwitchZoom(InputAction.CallbackContext context) {
        zoomSelected++;
        if (zoomSelected >= zoomHeights.Count) zoomSelected = 0;
        
        virtualCamera.GetComponent<CinemachineCameraOffset>().m_Offset = new Vector3(0, 0, -zoomHeights[zoomSelected]);
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