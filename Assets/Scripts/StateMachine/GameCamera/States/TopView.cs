using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class TopView : GameCameraBaseState
{

    private GameCameraStateMachine cameraStateMachine;
    public CinemachineVirtualCamera virtualCamera => cameraStateMachine.gameObject.transform.Find("Top Down Follow Camera").GetComponent<CinemachineVirtualCamera>();
    public CinemachineFreeLook otherCam => cameraStateMachine.gameObject.transform.Find("Active Orbit Camera").GetComponent<CinemachineFreeLook>();

    public AntStateMachine target;
    private InputActionMap inputActionMap;
    public InputAction zoomValue;
    public InputAction cameraRotate;
    public InputAction zoomKey;

    private List<float> zoomHeights = new List<float> {5f, 10f, 20f, 100f};
    private int zoomSelected = 2;

    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float rotationSmoothing = 0.3f;
    private Quaternion currentRotation = Quaternion.Euler(90, 0, 0);

    CinemachineCameraOffset cameraOffset;

    public TopView(GameCameraContext context, GameCameraStateMachine.ECameraStates stateKey) : base(context, stateKey)
    {
        target = Context.target;
        cameraStateMachine = context.cameraStateMachine;
        cameraOffset = virtualCamera.GetComponent<CinemachineCameraOffset>();

        inputActionMap = context.inputActions.FindActionMap("TopView").Clone();

        zoomValue = inputActionMap.FindAction("zoomValue");
        cameraRotate = inputActionMap.FindAction("cameraRotate");
        inputActionMap.FindAction("switchView").performed += SwitchView;
        inputActionMap.FindAction("select").performed += SelectTarget;
        inputActionMap.FindAction("zoomIn").performed += context => SwitchZoom(false);
        inputActionMap.FindAction("zoomOut").performed += context => SwitchZoom(true);
       
    }

    public override void EnterState()
    {
        Vector3 oldCameraFwd = cameraStateMachine.gameCamera.transform.forward;
        virtualCamera.Priority = 1;
        cameraOffset.m_Offset = new Vector3(0, 0, -zoomHeights[zoomSelected]);

        inputActionMap.Enable();

        oldCameraFwd.y = 0;
        oldCameraFwd.Normalize();

        // Make camera up be forward direction of old camera
        currentRotation = Quaternion.LookRotation(Vector3.down, oldCameraFwd);
        virtualCamera.transform.rotation = currentRotation;
        
        
        if (target) {
            target.TransitionToState(AntStateMachine.EAntStates.TopDownActive);
            virtualCamera.Follow = target.transform;
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

    }

    public override void ExitState()
    {
        inputActionMap.Disable();         

        cameraStateMachine.StartCoroutine(SmoothTransitionToYAxisValue(-zoomHeights[0], 0.3f));

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
        Quaternion inputRotation = Quaternion.Euler(0, 0, cameraRotate.ReadValue<float>() * rotationSmoothing);
        currentRotation *= inputRotation;

        virtualCamera.transform.rotation = Quaternion.RotateTowards(virtualCamera.transform.rotation, currentRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void SwitchZoom(bool zoomOut) {
        if (zoomOut && zoomSelected < zoomHeights.Count - 1) {
            zoomSelected++; 
        } 
        else if (!zoomOut && zoomSelected != 0) {
            zoomSelected--;
        }

//        cameraOffset.m_Offset = new Vector3(0, 0, -zoomHeights[zoomSelected]);
        cameraStateMachine.StartCoroutine(SmoothTransitionToYAxisValue(-zoomHeights[zoomSelected], 0.2f));
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


    private IEnumerator SmoothTransitionToYAxisValue(float targetValue, float duration) {
        float startValue = cameraOffset.m_Offset.z;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            cameraOffset.m_Offset.z = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            yield return null; // Wait until the next frame
        }

        // Ensure the final value is set exactly to the target
        cameraOffset.m_Offset.z = targetValue;
    }
}