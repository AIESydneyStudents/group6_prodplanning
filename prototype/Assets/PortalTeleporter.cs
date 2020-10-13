using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    public Transform Reciver;

    private Transform player;

    [HideInInspector]
    public bool PlayerIsOverlapping = false;

    private bool canTeleport = true;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if(Reciver == null)
        {
            Debug.LogWarning("Portal " + gameObject.name + " is NOT linked to a reciver, this will make it unuseable.");
        }
    }

    private void Update()
    {
        if(canTeleport && PlayerIsOverlapping && Reciver != null)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            //If true player should be teleported
            if(dotProduct < 0f)
            {
                CharacterController cont = player.GetComponent<CharacterController>();
                cont.enabled = false;

                Debug.Log("Yop");
                //TP them
                float rotationDiff = -Quaternion.Angle(transform.rotation, Reciver.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 posOffset = Quaternion.Euler(0,rotationDiff,0) * portalToPlayer;
                player.position = Reciver.position + posOffset;

                PlayerIsOverlapping = false;

                cont.enabled = true;

                //Disable recivers collision thing
                PortalTeleporter tele = Reciver.GetComponent<PortalTeleporter>();
                tele.PlayerIsOverlapping = false;
                tele.RecivedTeleport();
            }
        }
    }

    //Runs an event at the recives teleporter
    public void RecivedTeleport()
    {
        StartCoroutine(DisableForAShortPeriod());
    }

    //Mark when player is in trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            player = other.gameObject.transform;
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

    IEnumerator DisableForAShortPeriod()
    {
        canTeleport = false;

        yield return new WaitForSeconds(1f);

        PlayerIsOverlapping = false;
        canTeleport = true;
    }
}
