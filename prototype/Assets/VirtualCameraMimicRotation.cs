using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraMimicRotation : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCam;
    public GameObject ObjectToMimic;

    // Update is called once per frame
    void Update()
    {
        VirtualCam.transform.rotation = ObjectToMimic.transform.rotation;
    }
}
