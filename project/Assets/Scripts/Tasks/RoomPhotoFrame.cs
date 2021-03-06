﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomPhotoFrame : Task
{
    public CinemachineVirtualCamera vCamPersoective;

    private F_PlayerMovement player;
    private DialougeManager dialougeManager;
    public DialougeSequence sequenceToPlay;

    private bool delay = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();

        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(taskRunning)
        {
            if(delay && Input.GetMouseButtonDown(0))
            {
                if(!taskFinished) TaskFinished();
                player.ChangePerspective(null);
            }
        }
    }

    public void OnInteract()
    {
        taskRunning = true;
        player.ChangePerspective(vCamPersoective);
        if(!taskFinished) StartCoroutine(WaitToPlayDialouge());

        delay = false;
        StartCoroutine(WaitForDelay());
    }

    IEnumerator WaitForDelay()
    {
        yield return new WaitForSeconds(2.0f);
        delay = true;
    }

    IEnumerator WaitToPlayDialouge()
    {
        yield return new WaitForSeconds(1.0f);

        if (taskRunning)
        {
            dialougeManager.PlayDialouge(sequenceToPlay);
        }
    }
}
