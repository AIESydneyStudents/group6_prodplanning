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

    private int state = 0;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = Camera.main.transform.position;
        startRot = Camera.main.transform.rotation;
    }


    public void StartAnimation()
    {
        Started = true;
    }

    private Vector3 startPos;
    private Quaternion startRot;

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            timer += Time.deltaTime;

            MonitorVirtualCam.Priority = -1999;
            VirtualCamera.Priority = 10;

            Player.enabled = true;
            enabled = false;
            
        }
    }
}
