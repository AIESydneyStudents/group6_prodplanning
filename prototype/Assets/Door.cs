using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool Open = false;
    public float TargetOpenRotationY;

    private float initRotationY;

    private void Start()
    {
        initRotationY = transform.rotation.y;
    }

    public void ToggleDoor()
    {
        Open = !Open;
        Moving = true;
    }

    private bool Moving = false;

    public void Update()
    {
        //Rotate the door if need to
        if(Moving)
        {
            float targetDir = Open ? TargetOpenRotationY : initRotationY;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler( new Vector3(0, targetDir, 0)),25 * Time.deltaTime);
            if(transform.rotation == Quaternion.Euler(new Vector3(0, targetDir, 0)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, targetDir, 0));
                Moving = false;
            }

        }
    }
}
