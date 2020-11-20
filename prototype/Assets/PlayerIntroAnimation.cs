using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntroAnimation : MonoBehaviour
{
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
            switch (state)
            {
                case 0:
                {
                    Camera.main.transform.position = Vector3.Lerp(startPos, VirtualCamera.transform.position, timer / 2f);
                    Camera.main.transform.rotation = Quaternion.Lerp(startRot, VirtualCamera.transform.rotation, timer / 2f);
                    
                    if (timer > 0f)
                    {
                        state = 0;
                        timer = 0;
                        Player.enabled = true;
                        Started = false;
                        MainCameraBrain.enabled = true;
                    }
                    break;
                }

            }
        }
    }
}
