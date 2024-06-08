using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Active: AntBaseState 
{
    private AntStateMachine Ant;
    public Rigidbody rb;
    public GameCameraStateMachine camera;
    public InputActionMap inputActionMap;
    public InputAction move;
    public InputAction running;

    public bool isActive = false;

    public bool climbing;
    public bool isGrounded;
    public bool jumping;

    private Vector3 moveDirection;
    private Quaternion targetRotation;
    [SerializeField] float rotationSpeed = 500f;

    public Active(AntContext context, AntStateMachine.EAntStates estate) : base(context, estate) 
    {
        AntContext Context = context;
        Ant = Context.antStateMachine;
        rb = Context.rb;
        camera = Context.camera;

        inputActionMap = context.inputActions.FindActionMap("TargetView").Clone();
        move = inputActionMap.FindAction("move");
        running = inputActionMap.FindAction("sprint");
        
        inputActionMap.FindAction("jump").performed += OnJump;
        inputActionMap.FindAction("interact").performed += Interact;

    }


    public override void EnterState()
    {
        Debug.Log(Ant.gameObject.name + " entering Active state");
        isActive = true;
        inputActionMap.Enable();
    }

    public override void ExitState()
    {
        isActive = false;
        inputActionMap.Disable();
    }

    public override void UpdateState()
    {

        HandleMovement();
    }



    public override AntStateMachine.EAntStates GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerExit(Collider other){}
    public override void OnTriggerStay(Collider other){}



    private void Interact(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.tag == Tags.Collectable) {
                Collectable collectable = hit.collider.GetComponent<Collectable>();
                if (hit.distance < collectable.maxCollectableRange) collectable.Collected();
            }

            if (hit.collider.tag == Tags.Resource) {
                Resource resource = hit.collider.GetComponent<Resource>();
                if (hit.distance < resource.maxCollectableRange) resource.Damage(Ant.baseDmg);
            }

        }
    }

    private void OnJump(InputAction.CallbackContext context) {
        Debug.Log("Jump!");
        rb.AddForce(Ant.transform.up * Context.jumpForce, ForceMode.VelocityChange);
    }


    private void HandleMovement() {

        // make direction of camera forward
        Vector3 cameraForward = camera.gameCamera.transform.forward;
        Vector3 cameraRight = camera.gameCamera.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        
        float speedMult = running.IsPressed() ? Context.runSpeed : Context.walkSpeed; 
        moveDirection = (cameraForward * move.ReadValue<Vector2>().y) + (cameraRight * move.ReadValue<Vector2>().x);
        moveDirection *= speedMult * Time.fixedDeltaTime;
        // TODO: For Walking on walls: rotate directon based on vector of point infront -> point behind        



        Vector3 newPos = rb.position + moveDirection;
        // rb.MovePosition(newPos);

        // face direction of movement
        if (moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        
        Quaternion newRot = Quaternion.RotateTowards(Ant.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        Ant.transform.SetPositionAndRotation(newPos, newRot);


        // TODO: Better Handle Animations - Procedural foot placement on nearest surface
        if(moveDirection.magnitude > 0) {
            Ant.animations.Play("Walk");
        } else {
            Ant.animations.Stop("Walk");
        }

    }
}