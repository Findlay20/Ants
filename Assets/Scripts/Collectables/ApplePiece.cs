using UnityEngine;

public class ApplePiece : Collectable {

    public override void Awake()
    {
        // TODO: Move to common values file
        resourceName = "Apple";
        value = 1;
        maxCollectableRange = 1.5f;

    }
}