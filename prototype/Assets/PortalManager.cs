using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public Camera cameraBelow;
    public Material cameraMatB;

    private Dictionary<Transform, PortalTeleporter> portals = new Dictionary<Transform, PortalTeleporter>();

    // Start is called before the first frame update
    void Start()
    {
        if(cameraBelow.targetTexture != null)
        {
            cameraBelow.targetTexture.Release();
        }
        cameraBelow.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
        cameraMatB.mainTexture = cameraBelow.targetTexture;

        //Get All portas
        GameObject[] taggedPortals = GameObject.FindGameObjectsWithTag("Portal");

        foreach(GameObject obj in taggedPortals)
        {
            PortalTeleporter port = obj.GetComponentInChildren<PortalTeleporter>();

            if(port != null)
            {
                portals.Add(obj.transform,port);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
