using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishWashingTask : MonoBehaviour
{
    public DishStack DishesToWash;
    public SnapPickupInArea SnappingArea;

    private bool running = false;
    private bool hasRun = false;
    private int dishIndex = 0;
    private float dishCleanAmount = 0f;
    private List<GameObject> stackedDishes;
    private Dictionary<Transform, Tuple<Material, Collider>> dishMaterialLookup = new Dictionary<Transform, Tuple<Material,Collider>>();

    void StartTask()
    {
        //Reset base values
        dishIndex = 0;
        dishCleanAmount = 0;
        stackedDishes = DishesToWash.stackedPlates;

        //Get the materials of all the dishes
        for (int i = 0; i < DishesToWash.stackedPlates.Count; i++)
        {
            //Store components we'll need later
            dishMaterialLookup.Add(stackedDishes[i].transform, new Tuple<Material, Collider>(stackedDishes[i].GetComponent<MeshRenderer>().material,
                stackedDishes[i].GetComponent<Collider>()));
        }

        SnappingArea.RemoveSnap(stackedDishes[dishIndex].transform);

        running = true;
    }

    void NextDish()
    {
        Rigidbody bod = stackedDishes[dishIndex].GetComponent<Rigidbody>();
        bod.isKinematic = false;
        bod.useGravity = true;
        bod.detectCollisions = true;

        dishIndex++;
        dishCleanAmount = 0;

        if(dishIndex >= stackedDishes.Count)
        {
            running = false;
            hasRun = true;
            dishIndex = 0;
            dishCleanAmount = 0;
        }

        SnappingArea.RemoveSnap(stackedDishes[dishIndex].transform);
    }

    private void Update()
    {

        if (!hasRun && DishesToWash.Done && !running)
        {
            StartTask();
        }

        if (running)
        {
            GameObject currentDish = stackedDishes[dishIndex];
            currentDish.transform.position = Vector3.Lerp(currentDish.transform.position, transform.position, 25 * Time.deltaTime);
            currentDish.transform.LookAt(Camera.main.transform.position, Vector3.up);
            currentDish.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
            CleanDish(currentDish);
        }
    }

    private Vector3 oldScrubPos = Vector3.zero;

    private void CleanDish(GameObject dish)
    {
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
                        dishCleanAmount += 1 * Time.deltaTime;
                        dishMaterialLookup[dish.transform].Item1.SetFloat("_Percent", dishCleanAmount);

                        if(dishCleanAmount >= 1)
                        {
                            NextDish();
                        }
                    }
                }

                oldScrubPos = hit.point;
            }
        }
    }
}
