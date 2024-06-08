using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopView : GameCameraBaseState
{

    private StateManager<GameCameraStateMachine.ECameraStates> camera;
    public AntStateMachine target;
    private InputActionMap inputActionMap;
    public InputAction cameraMove;
    public InputAction zoom;


    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float zoomSensitivity = 0.1f;
    private Quaternion currentRotation = Quaternion.Euler(90, 0, 0);
    private float currentHeight;
    
    private float minZoom = 3.5f;
    private float maxZoom = 10f;


    public TopView(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(context, stateKey)
    {
        target = Context.target;
        camera = context.cameraStateMachine;
        currentHeight = context.TopViewCameraHeight;
        inputActionMap = context.inputActions.FindActionMap("TopView");

        cameraMove = inputActionMap.FindAction("cameraMove");
        inputActionMap.FindAction("switchView").performed += SwitchView;
        inputActionMap.FindAction("select").performed += SwitchTarget;
        zoom = inputActionMap.FindAction("zoom");
    }

    public override void EnterState()
    {
        inputActionMap.Enable();
        Vector3 topViewPos = new Vector3(0, currentHeight, 0) ;

        // if (target) {
        Vector3 oldCameraFwd = camera.transform.forward;
        if (target) topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);

        oldCameraFwd.y = 0;
        oldCameraFwd.Normalize();

        currentRotation = Quaternion.LookRotation(Vector3.down, oldCameraFwd);
        camera.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // } else {
        //     camera.transform.SetPositionAndRotation( topViewPos, currentRotation );
        // }
        
        
        if (target) target.TransitionToState(AntStateMachine.EAntStates.TopDownActive);
        Debug.Log("Entering Top View: " + (target ? target.gameObject.name : ""));

    }

    public override void ExitState()
    {
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
        camera.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
    }

    public override void UpdateState()
    {

        // Rotate camera
        UpdateRotation();
        if (zoom.IsPressed()) UpdateZoom();

        if (target) {
            // Follow target
            SetPositionToTarget();
        }
    }

    public void SetPositionToTarget() {
            Vector3 topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);
            // camera.transform.Translate(topViewPos.normalized) ;
            camera.transform.SetPositionAndRotation(topViewPos, currentRotation);

    }

    private void UpdateRotation() {
        Quaternion inputRotation = Quaternion.Euler(0, 0, cameraMove.ReadValue<Vector2>().x);
        currentRotation *= inputRotation;

        camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateZoom() {
        float heightChange = cameraMove.ReadValue<Vector2>().y;
        heightChange *= zoomSensitivity;

        currentHeight += heightChange;
        currentHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);
        camera.transform.position = new Vector3(camera.transform.position.x, currentHeight, camera.transform.position.z);
        
    }

    private void SwitchTarget(InputAction.CallbackContext context) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.GetComponent<AntStateMachine>()){
                if (target) target.TransitionToState(AntStateMachine.EAntStates.Idle);
                target = hit.collider.GetComponent<AntStateMachine>();
                
                Context.target = target;
                Debug.Log("New target: " + target.name);
                
                camera.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
            }
        };
    }
}