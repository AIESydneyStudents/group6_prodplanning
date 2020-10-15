using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    [Tooltip("The PortalBox (Spacifically the PortalBox!) of the target portal")]
    public Transform Reciver;

    private PlayerMovement player;

    [HideInInspector]
    public bool PlayerIsOverlapping = false;

    [HideInInspector]
    public PortalManager MainPortalManager; //Stores the Portal Manager from the scene, The portal manager itself sets this variable

    private bool canTeleport = true; // Used to disable teleportation when just teleporting.

    //Display linked portal
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if(Reciver != null)
        {
            Vector3 p1 = transform.position;
            Vector3 p2 = Reciver.position;
            Handles.DrawBezier(p1,p2,p1,p1,Color.green,null,4);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if(Reciver == null)
        {
            Debug.LogWarning("Portal " + gameObject.name + " is NOT linked to a reciver, this will make it unuseable.");
        }
    }

    private void Update()
    {
        if(canTeleport && PlayerIsOverlapping && !MainPortalManager.PlayerJustTeleported && Reciver != null)
        {
            Vector3 portalToPlayer = player.transform.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            //If true player should be teleported
            if(dotProduct < 0f)
            {       
                //Make sure the player is moving through portal
                Vector3 pf = player.Velocity.normalized;
                Vector3 tf = transform.forward.normalized;
                Debug.Log(pf.ToString());

                //Compare forwards to make sure the player is facing the right direction to teleport
                if (Vector3.Dot(tf, pf) < 0f)
                {
                    MainPortalManager.PlayerJustTeleported = true;
                    CharacterController cont = player.GetComponent<CharacterController>();
                    cont.enabled = false; //Disable the controller, messes with collisions when changing position.

                    //TP them, Position and rotate them accordingly
                    float rotationDiff = -Quaternion.Angle(transform.rotation, Reciver.rotation);
                    rotationDiff += 180;
                    player.transform.Rotate(Vector3.up, rotationDiff);

                    Vector3 posOffset = Quaternion.Euler(0, rotationDiff, 0) * portalToPlayer;
                    player.transform.position = Reciver.position + posOffset;

                    //Reenable the controller
                    cont.enabled = true;

                    //Disable recivers collision thing
                    PortalTeleporter tele = Reciver.GetComponent<PortalTeleporter>();          
                    tele.RecivedTeleport(this);
                    tele.PlayerIsOverlapping = false;
                }
                PlayerIsOverlapping = false;
            }
        }
    }

    //Runs an event at the recives teleporter, As in when teleported TO.
    public void RecivedTeleport(PortalTeleporter source)
    {
        StartCoroutine(DisableForAShortPeriod());

        if(MainPortalManager != null)
        {
            MainPortalManager.PortalCam.portal = source.transform;
            MainPortalManager.PortalCam.otherPortal = this.transform;
        }
    }

    //Mark when player is in trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(player == null || other.transform != player.transform)
            {
                player = other.GetComponent<PlayerMovement>();
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

    //Disables the telpoerter for a second.
    IEnumerator DisableForAShortPeriod()
    {
        canTeleport = false;

        yield return new WaitForSeconds(1f);

        PlayerIsOverlapping = false;
        canTeleport = true;
    }
}
