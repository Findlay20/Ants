using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownActive: AntBaseState 
{
    private AntStateMachine Ant;
    public Rigidbody rb;
    public GameCameraStateMachine camera;
    public InputActionMap inputActionMap;
    public InputAction move;


    public bool isActive = false;

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
        rb = Context.rb;
        camera = Context.camera;
        Debug.Log(rb);

        inputActionMap = context.inputActions.FindActionMap("TopView");
        move = inputActionMap.FindAction("move");
        running = inputActionMap.FindAction("sprint").IsPressed();
        
        inputActionMap.FindAction("interact").performed += Interact;
    }

    public override void EnterState()
    {
        isActive = true;
        inputActionMap.Enable();
    }

    public override void ExitState()
    {
        isActive = false;
        inputActionMap.Disable();
    }

    public override AntStateMachine.EAntStates GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter(Collider other){}
    public override void OnTriggerExit(Collider other){}
    public override void OnTriggerStay(Collider other){}


    public override void UpdateState()
    {

        HandleMovement();
    }

      private void Interact(InputAction.CallbackContext context)
    {
        Ray ray = new Ray(Ant.transform.position, Ant.transform.forward);
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

    private void HandleMovement() {

        // make direction of camera forward
        Vector3 cameraForward = camera.gameCamera.transform.up;
        Vector3 cameraRight = camera.gameCamera.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        float speedMult = running ? Context.runSpeed : Context.walkSpeed; 
        moveDirection = (cameraForward * move.ReadValue<Vector2>().y) + (cameraRight * move.ReadValue<Vector2>().x);
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

    }
}