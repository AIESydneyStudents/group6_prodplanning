using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PortalTeleporter : MonoBehaviour
{
    [Tooltip("The PortalBox (Spacifically the PortalBox!) of the target portal")]
    public Transform Reciver;

    public MeshRenderer planeRenderer;
    public UnityEvent OnTeleport;

    [HideInInspector]
    public bool PlayerIsOverlapping = false;

    [HideInInspector]
    public PortalManager MainPortalManager; //Stores the Portal Manager from the scene, The portal manager itself sets this variable

    private F_PlayerMovement player;
    private bool canTeleport = true; // Used to disable teleportation when just teleporting.
    private float initalZ = 0;

    //Display linked portal
    public void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR // prevents failiure to build on other platforms
        
            Gizmos.color = Color.green;
            if (Reciver != null)
            {
                Vector3 p1 = transform.position;
                Vector3 p2 = Reciver.position;
                Handles.DrawBezier(p1, p2, p1, p1, Color.green, null, 4);
            }
        #endif
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();

        initalZ = transform.position.z;

        if(Reciver == null)
        {
            Debug.LogWarning("Portal " + gameObject.name + " is NOT linked to a reciver, this will make it unuseable.");
        }
    }

    private void Update()
    {
        if(canTeleport && MainPortalManager != null && !MainPortalManager.PlayerJustTeleported && Reciver != null)
        {
            //Makes the protals surface visible if the player can teleport
            planeRenderer.enabled = true;

            if (PlayerIsOverlapping)
            {
                Vector3 portalToPlayer = player.transform.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                //If true player should be teleported
                if (dotProduct < 0f)
                {
                    //Make sure the player is moving through portal
                    Vector3 pf = player.Velocity.normalized;
                    Vector3 tf = transform.forward.normalized;

                    //Compare forwards to make sure the player is facing the right direction to teleport
                    if (Vector3.Dot(tf, pf) < 0f)
                    {
                        MainPortalManager.PlayerJustTeleported = true;
                        CharacterController cont = player.GetComponent<CharacterController>();
                        cont.enabled = false; //Disable the controller, messes with collisions when changing position.

                        //TP them, Position and rotate them accordingly
                        float rotationDiff = -Quaternion.Angle(transform.rotation, Reciver.rotation);
                        //rotationDiff += 180;
                        player.transform.Rotate(Vector3.up, rotationDiff);

                        Vector3 posOffset = Quaternion.Euler(0, rotationDiff, 0) * portalToPlayer;
                        player.transform.position = Reciver.position + posOffset;

                        //Reenable the controller
                        cont.enabled = true;

                        //Disable recivers collision thing
                        PortalTeleporter tele = Reciver.GetComponent<PortalTeleporter>();
                        StartCoroutine(DisableForAShortPeriod());
                        tele.PlayerIsOverlapping = false;
                        OnTeleport.Invoke();
                        player.BroadcastMessage("HasTeleported");
                    }
                    PlayerIsOverlapping = false;
                }
            }
        }
        else
        {
            //Player cannot teleport so don't display the portal
            planeRenderer.enabled = false;
        }
    }

    //Mark when player is in trigger
    private void OnTriggerEnter(Collider other)
    {
        //Only teleport if it's the player
        if(other.tag == "Player")
        {
            //If we don't have a stored instance of the player (Or it's a different one) set it.
            if(player == null || other.transform != player.transform)
            {
                player = other.GetComponent<F_PlayerMovement>();
            }

            PlayerIsOverlapping = true;
        }
    }

    //Mark when the player leaves it
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerIsOverlapping = false;
        }
    }

    public bool FacingCamera(Camera cam) //Checks if the plane is facing the camera, does not respect visibility through objects
    {
        return Vector3.Dot(transform.forward,cam.transform.position - transform.position) > 0;
    }

    //Disables the telpoerter for 0.1 seconds.
    IEnumerator DisableForAShortPeriod()
    {
        canTeleport = false;

        yield return new WaitForSeconds(0.1f);

        PlayerIsOverlapping = false;
        canTeleport = true;
    }
}
