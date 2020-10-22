using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnapPickupInArea : MonoBehaviour
{
    public LayerMask PickupLayer;
    public Transform TargetTransform;
    public UnityEvent OnSnapped;

    private PickupObject pickupObject;

    private void Start()
    {
        GameObject[] playerObj = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerObj.Length; i++)
        {
            PickupObject com = playerObj[i].GetComponent<PickupObject>();
            if (com != null)
            {
                pickupObject = com;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Check if object is on pickup layer
        if (PickupLayer == (PickupLayer | (1 << other.gameObject.layer)))
        {
            SnapPickup(other.transform);
            OnSnapped.Invoke();
        }
    }

    void SnapPickup(Transform pickup)
    {
        //Set all pickup stuff active.
        pickup.transform.position = TargetTransform.position;
        pickup.transform.rotation = TargetTransform.rotation;
        pickup.gameObject.layer   = TargetTransform.gameObject.layer;

        //Disable rigidbody if object has one
        Rigidbody pickupBody = pickup.gameObject.GetComponent<Rigidbody>();

        if(pickupBody != null)
        {
            pickupBody.useGravity = false;
            pickupBody.isKinematic = true;
        }

        //Force drop later.
        if(pickupObject.IsHoldingObject())
        {
            pickupObject.PickUpControl();
        }
    }
}
