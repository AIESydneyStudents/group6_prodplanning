using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectionEvent : MonoBehaviour
{
    public bool IsInspected = false;

    public bool PositionLocked = false;

    public Vector3 DesiredRotation;
    public float OffsetWithinRange = 100f; // If the object is within -OffsetWithinRange and +OffsetWithinRange of any axis of desired rotation

    [SerializeField] float ObjectSnapSmooth = 100f;

    void Update()
    { // Might update check to use Vector3.Distance lol..
        if (!IsInspected && PositionLocked == false) return;

        float dist = Vector3.Distance(transform.localEulerAngles, DesiredRotation);

        if (dist < OffsetWithinRange && dist > 1)
        {
            PositionLocked = true;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(DesiredRotation), ObjectSnapSmooth * Time.deltaTime);
        }

        if (Vector3.Distance(transform.localEulerAngles, DesiredRotation) < 50) 
        {
            Debug.Log("ok.ok.ok.");
            IsInspected = false;
            PositionLocked = false;
        }

        //if (transform.eulerAngles.x > DesiredRotation.x - OffsetWithinRange && transform.eulerAngles.x < DesiredRotation.x + OffsetWithinRange)
        //{ 
        //    if (transform.eulerAngles.y > DesiredRotation.y - OffsetWithinRange && transform.eulerAngles.y < DesiredRotation.y + OffsetWithinRange)
        //    {
        //        if (transform.eulerAngles.z > DesiredRotation.z - OffsetWithinRange && transform.eulerAngles.z < DesiredRotation.z + OffsetWithinRange)
        //        {
        //            Vector3 rot = transform.eulerAngles;
        //            rot.x = Mathf.Lerp(rot.x, DesiredRotation.x, ObjectSnapSmooth);
        //            rot.y = Mathf.Lerp(rot.y, DesiredRotation.y, ObjectSnapSmooth);
        //            rot.z = Mathf.Lerp(rot.z, DesiredRotation.z, ObjectSnapSmooth);

            //            transform.eulerAngles = rot;
            //        }
            //    }
            //}
    }
}
