using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Collectable : MonoBehaviour
{

    public string resourceName;
    public int value = 1; 
    public float maxCollectableRange = 1.5f;
    // TODO: move mesh collider to classes that expand this e.g. ApplePiece.cs


    void Start()
    {
        gameObject.tag = Tags.Collectable;
    }

    public void Collected() {
        this.gameObject.SetActive(false);
        ColonyResources.UpdateResource(resourceName, value);
    }    

    public abstract void Awake();

}
