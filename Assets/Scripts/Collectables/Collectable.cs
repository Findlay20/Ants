using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Collectable : MonoBehaviour
{

    public int value = 1; 
    private Gamepad controller;
    public float maxCollectableRange = 1.5f;
    // TODO: move mesh collider to classes that expand this e.g. ApplePiece.cs
    private MeshCollider meshCollider;

    void Awake()
    {
        if (GetComponent<MeshCollider>() == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        } else {
            meshCollider = gameObject.GetComponent<MeshCollider>();
        }

        controller = Gamepad.current;
        gameObject.tag = Tags.Collectable;
    }

    public void Collected() {
        transform.localScale = new Vector3(0,0,0);
        //trigger an animation
        this.gameObject.SetActive(false);
    }    

}
