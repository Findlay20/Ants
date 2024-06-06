// using System;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.InputSystem.Controls;

// public class AntMovement : MonoBehaviour
// {
//     public Rigidbody rb;
//     public CapsuleCollider collisionCapsion;
//     public Animation animations;

//     public bool isActive = false;

//     // TODO: maybe all controller goes in one place 
//     public Gamepad controller;
    

//     [Header("Movement settings")]
//     public float walkSpeed = 5f;
//     public float runSpeed = 12f;
//     public float sidewaysSpeed = 3f;
//     public float sidewaysSprintSpeed = 5f;
//     public float jumpForce = 2;
//     public float stickToGroundForce = 0f;
//     public float turningSpeed = 3f;
//     public float sprintTurningSpeed = 5f;
//     public float weight = 2f;

//     private Vector3 moveDirection;

//     private float horizontalInput;
//     private float verticalInput;


//     // States
//     [Header("Status")]
//     public bool running;
//     public bool climbing;
//     public bool isGrounded;
//     public bool jumping;

//     public int score;


//     void Start()
//     {
//         // Initiliase Gamepad
//         if (Gamepad.all.Count > 0) controller = Gamepad.all[0];

//     }

//     public void toggleActive() {
//         isActive = !isActive;
//     }

//     void Update() {

//         if (isActive) {
//             running = Input.GetKey(KeyCode.LeftShift) || controller.leftTrigger.IsPressed();
//             horizontalInput = Input.GetAxis("Horizontal");
//             verticalInput = Input.GetAxis("Vertical");

//             jumping = Input.GetKey(KeyCode.Space) || controller.aButton.isPressed;
//         }

//     }

//     // Update is called once per frame
//     void FixedUpdate()
//     {
//         HandleMovement();
//     }

//     void HandleMovement() {

//         if (isGrounded) {
//             //rb.isKinematic = false;
//             if (jumping){
//                 rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
//                 isGrounded = false;
//             } else {
//                 // downward force stops player bouncing on slopes
//                 rb.AddForce(-transform.up * stickToGroundForce, ForceMode.VelocityChange);
//             }
//         } else {
//             //TODO: some kind of falling animation
//             //rb.isKinematic = true;
//         }



//         //controller.Move(moveDirection * Time.fixedDeltaTime);
//         //rb.AddForce(verticalInput * (running ? runSpeed : walkSpeed) * -transform.right, ForceMode.Acceleration);

//         //moveDirection += -transform.forward * horizontalInput * (running ? sidewaysSprintSpeed : sidewaysSpeed) * Time.fixedDeltaTime;
//         //Vector3 moveSpeed = verticalInput * (running ? runSpeed : walkSpeed) * transform.right * Time.fixedDeltaTime;
//         //transform.Rotate(Vector3.up * Vector3.Angle(transform.position, newPos), Space.Self);
        

        
//         //right is forward and forwards is left  - x
//         moveDirection = ((transform.forward * verticalInput) + (transform.right * horizontalInput)) * (running ? runSpeed : walkSpeed) * Time.fixedDeltaTime;
        
//         Vector3 newPos = rb.position + moveDirection;
        
//         transform.SetPositionAndRotation(newPos, transform.rotation);
//         //transform.SetLocalPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 1));

//         if(moveDirection.magnitude > 0) {
//             animations.Play("Walk");
//         } else {
//             animations.Stop("Walk");
//         }

//         // Reset position 
//         if (isActive && controller.bButton.isPressed || Input.GetKey(KeyCode.Z)) {
//             transform.position = new Vector3(0,3.24f,0);
//         }

//     }

//     void OnGround() {

//     }

//     void OnClimbing() {
        
//     }

//     void OnCollisionEnter(Collision collision) {
        
//         GameObject collider = collision.collider.gameObject;

//         if (collider.CompareTag("Ground")) isGrounded = true;
//         // if (collider.CompareTag("Climbable")){
//         //     if (!climbing) transform.Rotate(0,0,90);
//         //     climbing = true;
//         // }

//         if (collider.CompareTag("Collectable")) {
//             Collectable collectable = collider.GetComponent<Collectable>();
//             score += collectable.value;
//             collectable.OnCollected();
//         }
//     }

//     void OnCollisionExit(Collision collision) {

//         GameObject collider = collision.collider.gameObject;

//         if (collider.CompareTag("Ground")) isGrounded = false;
//         // if (collider.CompareTag("Climbable")) {
//         //     if (climbing) transform.Rotate(0,0,-90);
//         //     climbing = false;
//         // }

//     }

// }
