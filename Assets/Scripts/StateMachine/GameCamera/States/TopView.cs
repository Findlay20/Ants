using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopView : GameCameraBaseState
{

    private StateManager<GameCameraStateMachine.ECameraStates> camera;
    private Gamepad controller;

    public AntStateMachine target;


    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float zoomSensitivity = 0.1f;
    private Quaternion currentRotation = Quaternion.Euler(90, 0, 0);
    private float currentHeight;
    
    private float minZoom = 3.5f;
    private float maxZoom = 10f;


    public TopView(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(context, stateKey)
    {
        target = Context.target;
        controller = context.controller;
        camera = context.cameraStateMachine;
        currentHeight = context.TopViewCameraHeight;
    
    }

    public override void EnterState()
    {

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
        Context.target = target;         
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

        // Rotate camera
        UpdateRotation();
        UpdateZoom();

        if (target) {
            // Follow target
            SetPositionToTarget();

            // Switch to active TargetView on button press
            if (controller.yButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.Tab)){
                camera.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
            }
        }
        

        // Switch to TargetView on clicked target
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            SwitchTarget();
        }

    }

    public void SetPositionToTarget() {
            Vector3 topViewPos = new Vector3(target.transform.position.x, currentHeight, target.transform.position.z);
            // camera.transform.Translate(topViewPos.normalized) ;
            camera.transform.SetPositionAndRotation(topViewPos, currentRotation);

    }

    private void UpdateRotation() {

        float mouseX = Input.GetAxis("Mouse X");
        float turn = 0f;
        if (Input.GetKey(KeyCode.E) || controller.rightShoulder.isPressed) {
            turn = -1;
        }
        if (Input.GetKey(KeyCode.Q) || controller.leftShoulder.isPressed) {
            turn = 1;  
        }

        Quaternion inputRotation = Quaternion.Euler(0, 0, turn);
        currentRotation *= inputRotation;

        camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void UpdateZoom() {
        bool keyboardZoom = Input.GetKey(KeyCode.Z);
        float mouseY = keyboardZoom ? Input.GetAxis("Mouse Y") : 0; 
        float heightChange = controller.rightStick.y.ReadValue() + mouseY;
        heightChange *= zoomSensitivity;

        currentHeight += heightChange;
        currentHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);
        camera.transform.position = new Vector3(camera.transform.position.x, currentHeight, camera.transform.position.z);
        
    }

    private void SwitchTarget() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.GetComponent<AntStateMachine>()){
                if (target) target.TransitionToState(AntStateMachine.EAntStates.Idle);
                target = hit.collider.GetComponent<AntStateMachine>();

                Debug.Log("New target: " + target.name);
                camera.TransitionToState(GameCameraStateMachine.ECameraStates.TargetView);
            }
        };
    }
}