using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashInBinTask : Task
{
    private class ObjectLerpHolder
    {
        public Transform transform;
        public Vector3 TargetPos;

        private Vector3 start;
        private float progress;
        private float rate;

        private Collider col;

        public ObjectLerpHolder(Transform tran, Vector3 target)
        {
            transform = tran;
            TargetPos = target;
            start = transform.position;
            progress = 0;
            rate = 4;

            col = tran.gameObject.GetComponent<Collider>();

            col.isTrigger = true;
            if(col.attachedRigidbody != null)
            {
                col.attachedRigidbody.detectCollisions = false;
                col.attachedRigidbody.velocity = Vector3.zero;
            }
        }

        public bool UpdateLerp()
        {
            progress = Mathf.MoveTowards(progress,1,rate * Time.deltaTime);
            transform.position = Vector3.Lerp(start, TargetPos, progress);
            if(progress == 1)
            {
                return true;
            }

            return false;
        }

        public void DisableObj()
        {
            col.isTrigger = false;
            if (col.attachedRigidbody != null)
            {
                col.attachedRigidbody.detectCollisions = true;
            }
        }
    }


    public int TrashCapacity = 10;
    public string TargetTag = "Trash";

    private PickupObject pickupObject = null;
    private int trashCurrent = 0;

    private List<ObjectLerpHolder> lerpHolders = new List<ObjectLerpHolder>();

    // Start is called before the first frame update
    void Start()
    {
        pickupObject = FindObjectOfType<PickupObject>();
        taskRunning = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(taskRunning && other.tag == TargetTag)
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

    private void Update()
    {
        if(lerpHolders.Count > 0)
        {
            for(int i = 0; i < lerpHolders.Count; i++)
            {
                if(lerpHolders[i].UpdateLerp())
                {
                    lerpHolders[i].DisableObj();
                    lerpHolders.RemoveAt(i);
                    i--;
                    continue;
                }
            }
        }
    }

    private void DisableTrashInteraction(GameObject trash)
    {
        trash.gameObject.layer = gameObject.layer;
        trash.tag = "Untagged";

        //Force drop later.
        if (pickupObject.IsHoldingObject())
        {
            pickupObject.PickUpControl();
        }

        lerpHolders.Add(new ObjectLerpHolder(trash.transform, transform.position));
    }
}
