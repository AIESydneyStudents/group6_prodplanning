using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntroAnimation : MonoBehaviour
{
    public CinemachineVirtualCamera MonitorVirtualCam;
    public CinemachineVirtualCamera VirtualCamera;
    public CinemachineBrain MainCameraBrain;
    public F_PlayerMovement Player;
    public bool Started = false;

    public void StartAnimation()
    {
        Started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            MonitorVirtualCam.Priority = -1999;
            VirtualCamera.Priority = 10;

            Player.enabled = true;
            enabled = false;
        }
    }
}
