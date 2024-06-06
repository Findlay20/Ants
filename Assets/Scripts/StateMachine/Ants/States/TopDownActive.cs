using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownActive: AntBaseState 
{
    private StateManager<AntStateMachine.EAntStates> Ant;
    private Gamepad controller;
    public Rigidbody rb;
    public GameCameraStateMachine camera;

    public bool isActive = false;

    
    private float horizontalInput;
    private float verticalInput;
    public bool running;
    public bool climbing;
    public bool isGrounded;
    public bool jumping;

    private Vector3 moveDirection;
    private Quaternion targetRotation;
    [SerializeField] float rotationSpeed = 500f;

    public TopDownActive(AntContext context, AntStateMachine.EAntStates estate) : base(context, estate) 
    {
        AntContext Context = context;
        Ant = Context.antStateMachine;
        controller = Context.controller;
        rb = Context.rb;
        camera = Context.camera;
        Debug.Log(rb);
    }

    public override void EnterState()
    {
        isActive = true;
        // move controller to some common place
        controller = Gamepad.current;
    }

    public override void ExitState()
    {
        isActive = false;
    }

    public override AntStateMachine.EAntStates GetNextState()
    {
        return StateKey;
    }

    // TODO: Copy collision from AntMovement.cs and fix jump isGrounded
    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerExit(Collider other){}
    public override void OnTriggerStay(Collider other){}


    public override void UpdateState()
    {
        running = Input.GetKey(KeyCode.LeftShift) || controller.leftTrigger.IsPressed();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        jumping = Input.GetKey(KeyCode.Space) || controller.aButton.isPressed;

        HandleMovement();
    }

    private void HandleMovement() {

        if (jumping){
            rb.AddForce(Ant.transform.up * Context.jumpForce, ForceMode.VelocityChange);
            isGrounded = false;
        }

        // make direction of camera forward
        Vector3 cameraForward = camera.gameCamera.transform.up;
        Vector3 cameraRight = camera.gameCamera.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        float speedMult = running ? Context.runSpeed : Context.walkSpeed; 
        moveDirection = (cameraForward * verticalInput) + (cameraRight * horizontalInput);
        moveDirection *= speedMult * Time.fixedDeltaTime;
        // TODO: For Walking on walls: rotate directon based on vector of point infront -> point behind        


        Vector3 newPos = rb.position + moveDirection;
        rb.MovePosition(newPos);

        // face direction of movement
        if (moveDirection != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        
        Ant.transform.rotation = Quaternion.RotateTowards(Ant.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        
        // TODO: Better Handle Animations - Procedural foot placement on nearest surface
        if(moveDirection.magnitude > 0) {
            Ant.animations.Play("Walk");
        } else {
            Ant.animations.Stop("Walk");
        }


        // Reset position 
        if (isActive && controller.bButton.isPressed || Input.GetKey(KeyCode.R)) {
            Ant.transform.position = new Vector3(0,3.24f,0);
        }

    }
}