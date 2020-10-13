using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    public Transform PlayerCamera;

    [Tooltip("The portal the camera is linked to.")]
    public Transform portal;

    [Tooltip("The source portal.")]
    public Transform otherPortal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerOffsetFromPortal = PlayerCamera.position - otherPortal.position;
        transform.position = portal.position + playerOffsetFromPortal;

        float angleDifferenceBetweenPortalsRots = Quaternion.Angle(portal.rotation,otherPortal.rotation);
        Quaternion portalRotationalDif = Quaternion.AngleAxis(angleDifferenceBetweenPortalsRots,Vector3.up);
        portalRotationalDif = Quaternion.Euler(portalRotationalDif.eulerAngles.x, portalRotationalDif.eulerAngles.y + 180f, portalRotationalDif.eulerAngles.z);
        Vector3 newCamDir = portalRotationalDif * PlayerCamera.forward;
        transform.rotation = Quaternion.LookRotation(newCamDir,Vector3.up);

    }
}
