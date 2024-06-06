using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameCamera : MonoBehaviour
{

    public AntStateMachine target;
    public bool topView;

    public Vector3 topViewPos = new Vector3(0, 10, 0);

    public float sensitivity = 20;
    public Vector3 cameraOffset = new Vector3(0, 0.36f, -1);
    private float cameraTurn;
    private Quaternion cameraRotation = Quaternion.Euler(0, 0, 0);

    // TODO: maybe all controller goes in one place
    public Gamepad controller = null;


    // Start is called before the first frame update
    void Start()
    {
        TopView();
                
        for (int i = 0; i < Gamepad.all.Count; i++) {Debug.Log(Gamepad.all[i].name);};

        if (Gamepad.all.Count > 0) controller = Gamepad.all[0];
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Make cursor for controllers
        // Set targets
        if (topView && Input.GetKeyUp(KeyCode.Mouse0)) {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)){
                if (hit.collider.GetComponent<AntStateMachine>()){
                    if (target) target.TransitionToState(AntStateMachine.EAntStates.Idle);
                    target = hit.collider.GetComponent<AntStateMachine>();
                    TargetView();
                    target.TransitionToState(AntStateMachine.EAntStates.Active);
                    Debug.Log("New target: " + target.name);
                }
            };

        }

        if (!topView) {
            TargetView();
        }


        if (controller.yButton.wasPressedThisFrame || Input.GetKeyDown(KeyCode.E)){
            SwitchView();
        }
    }

    private void TargetView() {
        topView = false;
        cameraTurn = Input.GetAxis("Mouse X") + controller.rightStick.x.ReadValue();
        cameraRotation = Quaternion.Euler(1, cameraTurn * sensitivity, 1); 

        transform.SetPositionAndRotation(target.transform.position + cameraOffset, cameraRotation);
    }

    private void TopView() {
        topView = true;
        transform.SetPositionAndRotation(topViewPos, Quaternion.Euler(90, 0, 0));
    }

    private void SwitchView() {
        if (topView  && target) {
            TargetView();
        } else { 
            TopView();
        }
    }

}
