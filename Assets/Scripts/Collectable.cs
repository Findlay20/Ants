using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Collectable : MonoBehaviour
{

    public int value = 1; 
    private Gamepad controller;

    void Awake()
    {
        // Add a MeshCollider to the GameObject if it doesn't already have one
        if (GetComponent<MeshCollider>() == null)
        {
            MeshCollider neshCollider = gameObject.AddComponent<MeshCollider>();
            // meshCollider.isTrigger = true; // Make it a trigger collider if necessary
        }
        controller = Gamepad.current;
    }

    public void Collected() {
        transform.localScale = new Vector3(0,0,0);
        //trigger an animation
        this.gameObject.SetActive(false);
    }    


    void OnCollisionEnter(Collision collision) {
        
        GameObject collider = collision.collider.gameObject;
        Debug.Log("collider.tag: " + collider.tag);
        if (collider.CompareTag(Tags.Player)) {
            //score += collectable.value;
            if (Input.GetKey(KeyCode.E) || controller.xButton.isPressed) Collected();
        }
    }

    void OnCollisionExit(Collision collision) {

        GameObject collider = collision.collider.gameObject;

    }
}
