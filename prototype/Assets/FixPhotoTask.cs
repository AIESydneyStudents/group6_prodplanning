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

    private AudioSource audioSource;
    private PickupTextPrompt prompt;

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

        //Get the text Prompt
        prompt = GameObject.FindGameObjectWithTag("PromptPanel").GetComponent<PickupTextPrompt>();

        audioSource = GetComponent<AudioSource>();
    }

    Vector2 previousMosuePos = Vector2.zero;
    Vector3 euler;
    float soundDelayTimer = 0f;

    private void Update()
    {
        if(taskRunning)
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousMosuePos = GetNormalizedMousePosition();
            }

            float dir = 0;

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = GetNormalizedMousePosition();
                euler = transform.eulerAngles;

                dir = mousePos.x - previousMosuePos.x;

                switch (AxisToAdjust)
                {
                    case AdjustAxis.x: euler.x -= dir * 20f; break;
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
                    audioSource.Stop();
                    prompt.DisableText();
                    OnTaskProgressed.Invoke();
                    player.ChangePerspective(null);
                    gameObject.layer = transform.parent.gameObject.layer;
                }
            }


            //Audio
            if (!taskFinished && audioSource != null) //Task can finish after top check so adding it again here
            {
                if (dir != 0)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.volume = 0.2f;
                        audioSource.Play();
                        soundDelayTimer = 0.5f;
                    }
                }
                else
                {
                    soundDelayTimer -= Time.deltaTime;
                    if (soundDelayTimer <= 0)
                    {
                        audioSource.Pause();
                    }
                    else
                    {
                        audioSource.volume = 0.2f * (soundDelayTimer / 0.5f);
                    }
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
            StartCoroutine(WaitToDisplayText());

            player.ChangePerspective(TaskCamera);
            taskRunning = true;

            previousMosuePos = GetNormalizedMousePosition();
        }
    }

    //Wait for a short time, so the previous prompt can fade
    IEnumerator WaitToDisplayText()
    {
        yield return new WaitForSeconds(1.0f);

        if(taskRunning)
        {
            prompt.Text.text = "Click and move the mouse left/right to rotate.";
            prompt.EnableText();
        }
    }
}
