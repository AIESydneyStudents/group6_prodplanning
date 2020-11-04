using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlowPickupable : MonoBehaviour
{
    void Start()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Pickupable")) // change cause not tag, layer
        {
            obj.AddComponent<PostProcessVolume>();
            obj.GetComponent<PostProcessVolume>().profile = new PostProcessProfile();
        }
    }

    void Update()
    {
        
    }

    void SetGlow(GameObject obj, bool _set)
    {
        if (_set)
        {
            obj.GetComponent<Renderer>().material.SetColor("_EMISSION", new Color(255, 255, 255));
            obj.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");

            //obj.GetComponent<PostProcessVolume>().
        }
        else
        {
            obj.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
