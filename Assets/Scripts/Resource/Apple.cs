using UnityEngine;

public class Apple : Resource
{
    
    MeshCollider meshCollider;
    public int numberOfDrops = 4;
    public GameObject dropItem;

    void Awake() {
        if ( gameObject.GetComponent<MeshCollider>() != null) {
            meshCollider = gameObject.GetComponent<MeshCollider>();
        } else {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

    }

    public void Start() 
    {
        // TODO: move to variables file probably
        resourceName = "Apple";
        description = "Press F to harvest this apple";
        gameObject.tag = Tags.Resource; // I want to do this in base idealy

        health = 30;
        
        dropItem = Resources.Load<GameObject>("Prefabs/AppleChunk");
    }



    public override void Destroy()
    {
        gameObject.SetActive(false);
        float spawnHeight = 0;
        for (int i = 0; i < numberOfDrops; i++) {     
            if (dropItem != null) SpawnDrop(dropItem, transform.position + (Vector3.up * spawnHeight), transform.rotation);
            spawnHeight += 1f;
        }

    }

    public override void SpawnDrop(GameObject drop, Vector3 spawnPos, Quaternion rotation)
    {
        Instantiate(drop, spawnPos, rotation);
    }
}