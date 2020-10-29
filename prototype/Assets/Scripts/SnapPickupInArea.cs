using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class SnapPickupInArea : MonoBehaviour
{
    public LayerMask PickupLayer;
    public Transform TargetTransform;
    public UnityEvent OnSnapped;
    public float SmoothingAmount = 30f;

    public float SnappingReach = 0.01f;

    public GameObject SnappingObject { get { return snappingObjects.Count > 0 ? snappingObjects[snappingObjects.Count - 1].Item1.gameObject : null; } }

    private PickupObject pickupObject = null;
    private List<Tuple<Transform, Transform>> snappingObjects = new List<Tuple<Transform, Transform>>();

    private void Start()
    {
        pickupObject = FindObjectOfType<PickupObject>();
    }

    private void Update()
    {
        //Lerp all snapping objects if any
        if (snappingObjects.Count > 0)
        {
            for (int i = 0; i < snappingObjects.Count; i++)
            {
                snappingObjects[i].Item1.transform.position = Vector3.Lerp(snappingObjects[i].Item1.transform.position, snappingObjects[i].Item2.position, SmoothingAmount * Time.deltaTime);
                snappingObjects[i].Item1.transform.rotation = Quaternion.Lerp(snappingObjects[i].Item1.transform.rotation, snappingObjects[i].Item2.rotation, SmoothingAmount * Time.deltaTime);

                if (Vector3.Distance(snappingObjects[i].Item1.transform.position, TargetTransform.position) < SnappingReach)
                {
                    snappingObjects[i].Item1.transform.position = new Vector3(snappingObjects[i].Item2.position.x, snappingObjects[i].Item2.position.y, snappingObjects[i].Item2.position.z);
                    snappingObjects[i].Item1.transform.rotation = Quaternion.Euler(snappingObjects[i].Item2.rotation.x, snappingObjects[i].Item2.rotation.y, snappingObjects[i].Item2.rotation.z);
                    snappingObjects.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void RemoveSnap(Transform transform)
    {
        for (int i = 0; i < snappingObjects.Count; i++)
        {
            if (transform == snappingObjects[i].Item1)
            {
                snappingObjects.RemoveAt(i);
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
        snappingObjects.Add(new Tuple<Transform, Transform>(pickup, TargetTransform));
        pickup.gameObject.layer = TargetTransform.gameObject.layer;

        //Disable rigidbody if object has one
        Rigidbody pickupBody = pickup.gameObject.GetComponent<Rigidbody>();

        if (pickupBody != null)
        {
            //col.isTrigger = true;
            pickupBody.useGravity = false;
            pickupBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            pickupBody.isKinematic = true;
            pickupBody.detectCollisions = false;
            pickupBody.velocity = Vector3.zero;
            pickupBody.Sleep();
            //Destroy(pickupBody);
        }

        //Force drop later.
        if (pickupObject.IsHoldingObject())
        {
            pickupObject.PickUpControl();
        }
    }
}
