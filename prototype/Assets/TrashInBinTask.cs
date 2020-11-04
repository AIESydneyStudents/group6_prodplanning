using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashInBinTask : Task
{
    public int TrashCapacity = 10;

    private PickupObject pickupObject = null;
    private int trashCurrent = 0;

    // Start is called before the first frame update
    void Start()
    {
        pickupObject = FindObjectOfType<PickupObject>();
        taskRunning = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(taskRunning && other.tag == "Trash")
        {
            DisableTrashInteraction(other.gameObject);            
            trashCurrent++;

            if(trashCurrent >= TrashCapacity)
            {
                TaskFinished();
            }
            else
            {
                OnTaskProgressed.Invoke();
            }
        }
    }

    private void DisableTrashInteraction(GameObject trash)
    {
        trash.gameObject.layer = gameObject.layer;

        //Force drop later.
        if (pickupObject.IsHoldingObject())
        {
            pickupObject.PickUpControl();
        }
    }
}
