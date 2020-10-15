using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    [Tooltip("The secondary camera")]
    public Camera cameraBelow;

    [Tooltip("The material the render texture is placed on")]
    public Material cameraMatB;

    [HideInInspector]
    public PortalCamera PortalCam; //Set from start, GetComponent from public cameraBelow.

    [HideInInspector]
    public bool PlayerJustTeleported = false;

    private Dictionary<Transform, PortalTeleporter> portals = new Dictionary<Transform, PortalTeleporter>();

    // Start is called before the first frame update
    void Start()
    {
        //Sets the render texture of the portal camera
        if(cameraBelow.targetTexture != null)
        {
            cameraBelow.targetTexture.Release();
        }
        cameraBelow.targetTexture = new RenderTexture(Screen.width,Screen.height,24);
        cameraMatB.mainTexture = cameraBelow.targetTexture;

        //Get All portas
        GameObject[] taggedPortals = GameObject.FindGameObjectsWithTag("Portal");

        //Add all portals to a dictinary
        foreach(GameObject obj in taggedPortals)
        {
            PortalTeleporter port = obj.GetComponentInChildren<PortalTeleporter>();

            if(port != null)
            {
                portals.Add(obj.transform,port);
                port.MainPortalManager = this;
            }
        }

        //Get the portal cam component
        PortalCam = cameraBelow.GetComponent<PortalCamera>();
    }

    private float teleporterTimer = 0;

    private void Update()
    {
        teleporterTimer += Time.deltaTime;
        //Player should not teleport if teleported into another telepoerter.
        if (PlayerJustTeleported)
        {
            if (teleporterTimer > 0.1f)
            {
                bool notTouching = false;
                foreach (KeyValuePair<Transform, PortalTeleporter> pair in portals)
                {
                    if (pair.Value.PlayerIsOverlapping)
                    {
                        notTouching = true;
                    }
                }

                PlayerJustTeleported = notTouching;
                teleporterTimer = 0;
            }
        }
        else if (teleporterTimer >= 0.1f)
        {
            Transform nearest = PortalCam.otherPortal;
            float nearestDist = Vector3.Distance(PortalCam.PlayerCamera.transform.position, nearest.position);

            foreach (KeyValuePair<Transform, PortalTeleporter> pair in portals)
            {
                float dist = Vector3.Distance(PortalCam.PlayerCamera.transform.position, pair.Key.position);
                if (dist < nearestDist)
                {
                    dist = nearestDist;
                    nearest = pair.Key;
                }
            }

            if (nearest != null)
            {
                if(!portals.ContainsKey(nearest))
                {
                    portals.Add(nearest,nearest.GetComponent<PortalTeleporter>());
                }

                PortalCam.portal = portals[nearest].Reciver;
                PortalCam.otherPortal = nearest;
                teleporterTimer = 0;
            }
        }
    }
}
