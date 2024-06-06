using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Resource : MonoBehaviour
{

    public string resourceName;
    public string description;

    public float maxCollectableRange = 1.8f;
    public float health = float.MaxValue;
    // public Enum validTools;

    public Resource() {    
        // gameObject.tag = Tags.Resource;
    }

    public float Damage(float damage) {
        health -= damage;
        Debug.Log(resourceName + " damaged: " + damage);
        return health;
    }

    public abstract void Destroy();
    public abstract void SpawnDrop(GameObject drop, Vector3 spawnPos, Quaternion rotation);
    
    public void Update() {
        if (health <= 0) Destroy();
    }
}
