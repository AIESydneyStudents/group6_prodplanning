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
    {
        if(Mathf.Abs(transform.eulerAngles.x) > Mathf.Abs(DesiredRotation.x) + OffsetWithinRange)
        {
            if (Mathf.Abs(transform.eulerAngles.y) > Mathf.Abs(DesiredRotation.y) + OffsetWithinRange)
            {
                if (Mathf.Abs(transform.eulerAngles.z) > Mathf.Abs(DesiredRotation.z) + OffsetWithinRange)
                {
                    PositionLocked = true;
                    transform.localRotation = Quaternion.Lerp(Quaternion.Euler(transform.localRotation.eulerAngles), Quaternion.Euler(DesiredRotation), Time.deltaTime * ObjectSnapSmooth);

                }
            }
        }

        // Might update check to use Vector3.Distance lol..
        //float dist = Vector3.Angle(transform.localEulerAngles, DesiredRotation);
        //Debug.Log(dist);

        //if (dist < OffsetWithinRange && dist > 1)
        //{
        //    PositionLocked = true;
        //    transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(DesiredRotation), ObjectSnapSmooth * Time.deltaTime);
        //}

        //if (dist < 2)
        //{
        //    IsInspected = false;
        //    PositionLocked = false;
        //}

        //    bool rotating = false;
        //    if (Mathf.Abs(transform.localEulerAngles.x) > Mathf.Abs(DesiredRotation.x - OffsetWithinRange))
        //    {
        //        if (Mathf.Abs(transform.localEulerAngles.y) > Mathf.Abs(DesiredRotation.y - OffsetWithinRange))
        //        {
        //            if (Mathf.Abs(transform.localEulerAngles.z) > Mathf.Abs(DesiredRotation.z - OffsetWithinRange))
        //            {
        //                rotating = true;

        //                PositionLocked = true;
        //                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(DesiredRotation), ObjectSnapSmooth * Time.deltaTime);
        //            }
        //        }
        //    }

        //    if (!rotating)
        //    {
        //        IsInspected = false;
        //        PositionLocked = false;
        //    }
    }
}
