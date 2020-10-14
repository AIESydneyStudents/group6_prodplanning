using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    GameObject pickedObject;
    Rigidbody pickedObjectRb;

    public LayerMask PickableObjectLayer;
    public GameObject ObjectDestination;

    bool smoothMove = false;
    bool focusing = false;

    [SerializeField] float pickUpSmooth = 20f;
    [SerializeField] float carrySmooth = 4f;

    void Start()
    {
        ObjectDestination = GameObject.FindGameObjectWithTag("ObjectHolder");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) PickUpControl();

        if (pickedObject == null) return; // Won't continue past this point if there's no object.

        if (smoothMove) // When the object is first picked up..
        {
            pickedObject.transform.position = Vector3.MoveTowards(pickedObject.transform.position,
                ObjectDestination.transform.position, Time.deltaTime * pickUpSmooth); 

            if (pickedObject.transform.position == ObjectDestination.transform.position)
            {
                pickedObject.transform.parent = ObjectDestination.transform;
                
                smoothMove = false;
                focusing = true;
            }
        }

        if (focusing) // Object has been picked up and is now focusing on its position.
        {
            pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, ObjectDestination.transform.position, Time.deltaTime * carrySmooth);
            pickedObject.transform.rotation = Quaternion.identity; // So that the object doesn't spin, spin and spin.
        }

        // There's an issue with the lerping, it sometimes won't continue trying to lerp to desired position after random collision testing.
        // Issue with position looks unstable and uncentered, try recreating the issue and let me know what you think, Ryan.
    }

    void PickUpControl()
    {
        if (pickedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f, PickableObjectLayer))
            { // Raycast from camera perspective, this may need to be edited if multiple cameras are used.
                pickedObject = hit.collider.gameObject;
                smoothMove = true;

                pickedObjectRb = pickedObject.GetComponent<Rigidbody>();
                pickedObjectRb.useGravity = false;
            }
        }
        else // Reset everything.
        { 
            pickedObject.transform.parent = null;
            pickedObject = null;

            pickedObjectRb.useGravity = true;
            pickedObjectRb = null;

            smoothMove = false;
            focusing = false;
        }
    }
}
