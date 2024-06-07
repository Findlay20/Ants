using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class ColonyResources {

    public static Dictionary<string, int> ResourceList = new Dictionary<string, int>();

    public static int GetResourceCount(string resource) {
        if (ResourceList.ContainsKey(resource)) {
            return ResourceList[resource];
        } else {
            return 0;
        }
    }

    public static void UpdateResource(string resource, int change) {
        if (ResourceList.ContainsKey(resource)) {
            ResourceList[resource] += change;
        } else if (change > 0) {
            ResourceList.Add(resource, change);
        }
        
        Debug.Log("Resources updated: " + ResourceList.ToCommaSeparatedString());
    }

}