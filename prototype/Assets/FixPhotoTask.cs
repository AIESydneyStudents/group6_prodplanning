using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
[RequireComponent(typeof(Collider))]
public class FixPhotoTask : Task
{
    F_PlayerMovement player;
    public CinemachineVirtualCamera TaskCamera;
    public AdjustAxis AxisToAdjust = AdjustAxis.x;

    public enum AdjustAxis
    {
        x,
        y,
        z
    }

    // Start is called before the first frame update
    void Start()
    {
        taskRunning = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
    }

    Vector2 previousMosuePos = Vector2.zero;
    Vector3 euler;

    private void Update()
    {
        if(taskRunning)
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousMosuePos = GetNormalizedMousePosition();
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = GetNormalizedMousePosition();
                euler = transform.eulerAngles;

                float dir = mousePos.x - previousMosuePos.x;

                switch (AxisToAdjust)
                {
                    case AdjustAxis.x: euler.x += dir * 20f; break;
                    case AdjustAxis.y: euler.y += dir * 20f; break;
                    case AdjustAxis.z: euler.z += dir * 20f; break;
                }

                transform.eulerAngles = euler;
                previousMosuePos = mousePos;

                bool CorrectCheck = false;

                switch (AxisToAdjust)
                {
                    case AdjustAxis.x: CorrectCheck = transform.localRotation.eulerAngles.x < 1f && transform.localRotation.eulerAngles.x > -1f; break;
                    case AdjustAxis.y: CorrectCheck = transform.localRotation.eulerAngles.y < 1f && transform.localRotation.eulerAngles.y > -1f; break;
                    case AdjustAxis.z: CorrectCheck = transform.localRotation.eulerAngles.z < 1f && transform.localRotation.eulerAngles.z > -1f; break;
                }

                if (CorrectCheck)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);

                    switch (AxisToAdjust)
                    {
                        case AdjustAxis.x: transform.rotation = Quaternion.Euler(0, euler.y, euler.z); break;
                        case AdjustAxis.y: transform.rotation = Quaternion.Euler(euler.x, 0, euler.z); break;
                        case AdjustAxis.z: transform.rotation = Quaternion.Euler(euler.x, euler.y, 0); break;
                    }

                    TaskFinished();
                    OnTaskProgressed.Invoke();
                    player.ChangePerspective(null);
                    gameObject.layer = transform.parent.gameObject.layer;
                }
            }
        }
    }

    public Vector2 GetNormalizedMousePosition() //Mouse positions between 0 to 1
    {
        Vector2 mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;

        return mousePos;
    }

    public void InteractWithPhoto()
    {
        if(!taskFinished)
        {
            player.ChangePerspective(TaskCamera);
            taskRunning = true;

            previousMosuePos = GetNormalizedMousePosition();
        }
    }
}
