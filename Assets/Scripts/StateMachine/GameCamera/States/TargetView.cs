using System;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetView : GameCameraBaseState
{

    private GameCameraStateMachine camera;
    public CinemachineFreeLook virtualCamera => camera.gameObject.transform.Find("Active Orbit Camera").GetComponent<CinemachineFreeLook>();
    //public CinemachineVirtualCamera virtualCamera => camera.gameObject.transform.Find("Active Orbit Camera").GetComponent<CinemachineVirtualCamera>();

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
     
        virtualCamera.LookAt = target.transform;
        virtualCamera.Follow = target.transform;
        
        
        // I want to first set to this:
        virtualCamera.m_YAxis.Value = 1f;
                
        
        virtualCamera.Priority = 1;
        
        // Then smoothly move to this:
        camera.StartCoroutine(SmoothTransitionToYAxisValue(0.05f, 0.8f));

        Debug.Log("Entering Target View: " + target.gameObject.name);
        Debug.Log("Switching to camera: " + virtualCamera);
        
        cameraRotation = Quaternion.LookRotation(camera.transform.up);
        target.TransitionToState(AntStateMachine.EAntStates.Active);

        inputActionMap.Enable();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public override void ExitState()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        camera.StartCoroutine(SmoothTransitionToYAxisValue(1f, 0.4f));
        virtualCamera.Priority = 0;
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

    private IEnumerator SmoothTransitionToYAxisValue(float targetValue, float duration) {
        float startValue = virtualCamera.m_YAxis.Value;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            virtualCamera.m_YAxis.Value = Mathf.Lerp(startValue, targetValue, timeElapsed / duration);
            yield return null; // Wait until the next frame
        }

        // Ensure the final value is set exactly to the target
        virtualCamera.m_YAxis.Value = targetValue;
    }

}