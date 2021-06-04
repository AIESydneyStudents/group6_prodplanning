using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CimachineTeleportFix : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public float Height = 1.7f;

    // Update is called once per frame
    void Update()
    {
        VirtualCamera.transform.localPosition = new Vector3(0, Height, 0);
    }
}
