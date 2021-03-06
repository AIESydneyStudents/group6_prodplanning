﻿using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DishWashingTask : Task
{
    public DishStack DishesToWash;
    public SnapPickupInArea SnappingArea;
    public CinemachineVirtualCamera TaskCamera;
    public ParticleSystem SoapParticles;
    public SnapPickupInArea CleanPlateStack;

    private float dishCleanAmount = 0f;
    private GameObject currentDishToClean;
    private Dictionary<Transform, Tuple<Material, Collider>> dishMaterialLookup = new Dictionary<Transform, Tuple<Material,Collider>>();
    private F_PlayerMovement player;
    private DialougeManager dialougeManager;
    private AudioSource audioPlayer;

    private void Start()
    {
        dialougeManager = GameObject.FindGameObjectWithTag("DialougeSystem").GetComponent<DialougeManager>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void StartTask()
    {
        if (!DishesToWash.HasDishes())
        {
            dialougeManager.PlayDialougeIfNotPlaying(new List<DialougeLine> { new DialougeLine("There are no dishes to wash.",3f) });
            return;
        }

        //Reset base values
        dishCleanAmount = 0;
        currentDishToClean = DishesToWash.PopDish();


        //Store components we'll need later
        dishMaterialLookup[currentDishToClean.transform] = new Tuple<Material, Collider>(currentDishToClean.GetComponent<MeshRenderer>().material,
            currentDishToClean.GetComponent<Collider>());


        player = GameObject.FindGameObjectWithTag("Player").GetComponent<F_PlayerMovement>();
        player.ChangePerspective(TaskCamera);

        SnappingArea.RemoveSnap(currentDishToClean.transform);

        taskRunning = true;
    }

    void NextDish()
    {

        CleanPlateStack.SnapPickup(currentDishToClean.transform);

        OnTaskProgressed.Invoke();

        if (!DishesToWash.HasDishes())
        {
            dishCleanAmount = 0;
            player.ChangePerspective(null);
            TaskFinished();
            Cursor.lockState = CursorLockMode.Locked;
            return;
        }

        //Reset stas and get next dish
        dishCleanAmount = 0;
        currentDishToClean = DishesToWash.PopDish();

        //Add to dictinary
        dishMaterialLookup[currentDishToClean.transform] = new Tuple<Material, Collider>(currentDishToClean.GetComponent<MeshRenderer>().material,
                currentDishToClean.GetComponent<Collider>());

        SnappingArea.RemoveSnap(currentDishToClean.transform);
    }

    private void Update()
    {
        if (taskRunning)
        {
            GameObject currentDish = currentDishToClean;
            currentDish.transform.position = Vector3.Lerp(currentDish.transform.position, transform.position, 25 * Time.deltaTime);
            currentDish.transform.LookAt(Camera.main.transform.position, Vector3.up);
            currentDish.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
            CleanDish(currentDish);
        }
    }

    private Vector3 oldScrubPos = Vector3.zero;
    private ParticleSystem.EmitParams soapParams = new ParticleSystem.EmitParams();
    private float dishCleanTimer = 0;

    private void CleanDish(GameObject dish)
    {
        bool hittem = false;
        //Check if player clicking on dish
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            

            if (dishMaterialLookup[dish.transform].Item2.Raycast(ray, out hit, 5f))
            {
                if (hit.transform.gameObject == dish)
                {
                    if(Vector3.Distance(hit.point,oldScrubPos) > 0.01f)
                    {
                        hittem = true;
                        dishCleanAmount += 1 * Time.deltaTime;

                        if(!audioPlayer.isPlaying)
                        {
                            audioPlayer.Play();
                            audioPlayer.volume = 1;
                        }

                        dishCleanTimer = 0.1f;

                        soapParams.position = hit.point;
                        soapParams.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.05f,0.1f),0);
                        soapParams.startSize = Random.Range(0.5f,1f);
                        SoapParticles.Emit(soapParams,2);

                        dishMaterialLookup[dish.transform].Item1.SetFloat("_Percent", dishCleanAmount);

                        if(dishCleanAmount >= 1)
                        {
                            NextDish();
                            audioPlayer.Stop();
                        }
                    }
                }

                oldScrubPos = hit.point;
            }
        }

        if(!hittem) // stop sound if not playing.
        {
            dishCleanTimer -= Time.deltaTime;
            audioPlayer.volume = dishCleanTimer * 10f;
            if (dishCleanTimer <= 0)
            {
                audioPlayer.Pause();
            }
        }
    }
}
