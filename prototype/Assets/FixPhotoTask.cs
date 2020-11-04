using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPhotoTask : Task
{
    F_PlayerMovement player;
    public CinemachineVirtualCamera TaskCamera;

    // Start is called before the first frame update
    void Start()
    {
        taskRunning = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
    }

    Vector2 previousMosuePos = Vector2.zero;

    private void Update()
    {
        if(taskRunning)
        {
            float dir = Input.mousePosition.x - previousMosuePos.x;

            transform.rotation *= Quaternion.Euler(dir * 0.05f,0,0);

            previousMosuePos = Input.mousePosition;
        }
    }

    public void InteractWithPhoto()
    {
        if(!taskFinished)
        {
            player.ChangePerspective(TaskCamera);
            taskRunning = true;

            previousMosuePos = Input.mousePosition;
        }
    }
}
