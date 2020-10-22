using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnapPickupInArea : MonoBehaviour
{
    public LayerMask PickupLayer;
    public Transform TargetTransform;
    public UnityEvent OnSnapped;
    public float SmoothingAmount = 30f;

    private PickupObject pickupObject = null;
    private List<Transform> snappingObjects = new List<Transform>();

    private void Start()
    {
        pickupObject = FindObjectOfType<PickupObject>();
    }

    private void Update()
    {
        //Lerp all snapping objects if any
        if(snappingObjects.Count > 0)
        {
            for(int i = 0; i < snappingObjects.Count; i++)
            {
                snappingObjects[i].transform.position =    Vector3.Lerp(snappingObjects[i].transform.position,TargetTransform.position,SmoothingAmount * Time.deltaTime);
                snappingObjects[i].transform.rotation = Quaternion.Lerp(snappingObjects[i].transform.rotation,TargetTransform.rotation,SmoothingAmount * Time.deltaTime);

                if(Vector3.Distance(snappingObjects[i].transform.position, TargetTransform.position) < 0.1f && Quaternion.Dot(snappingObjects[i].transform.rotation, TargetTransform.rotation) > 0.999f)
                {
                    Debug.Log("AHH");
                    snappingObjects[i].transform.position = TargetTransform.position;
                    snappingObjects[i].transform.rotation = TargetTransform.rotation;
                    snappingObjects.RemoveAt(i);
                    i--;
                }
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
        snappingObjects.Add(pickup);
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
