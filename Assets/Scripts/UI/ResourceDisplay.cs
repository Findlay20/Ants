using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    
    public Text text;

     public ResourceDisplay() {
        text = FindAnyObjectByType<Text>();
    }

    public void UpdateResourceList(Dictionary<string, int> resourceList) {
        if (text == null) return;

        text.text = "";
        foreach (var resource in resourceList)
        {
            text.text += $"{resource.Key}: {resource.Value}\n";

        }
    }

}
