using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Camera cameraBelow;
    public Material cameraMatB;

    // Start is called before the first frame update
    void Start()
    {
        if(cameraBelow.targetTexture != null)
        {
            cameraBelow.targetTexture.Release();
        }
        cameraBelow.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
        cameraMatB.mainTexture = cameraBelow.targetTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
