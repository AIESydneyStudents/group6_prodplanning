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

    // Start is called before the first frame update
    void Start()
    {
        taskRunning = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
    }

    Vector2 previousMosuePos = Vector2.zero;

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

                float dir = mousePos.x - previousMosuePos.x;
                transform.rotation *= Quaternion.Euler(dir * 20.0f, 0, 0);
                previousMosuePos = mousePos;

                if (transform.localRotation.eulerAngles.x < 0.25f && transform.localRotation.eulerAngles.x > -0.25f)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
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
