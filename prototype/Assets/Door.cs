using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool Open = false;
    public float TargetOpenRotationY;

    private float initRotationY;

#if UNITY_EDITOR // So we can skip tasks >:>
    private Collider col;
#endif

    private void Start()
    {
        initRotationY = transform.rotation.eulerAngles.y;

        #if UNITY_EDITOR // So we can skip tasks >:>
            col = GetComponent<Collider>();
        #endif
    }

public void ToggleDoor()
    {
        Open = !Open;
        Moving = true;
    }

    private bool Moving = false;

    public void Update()
    {
        #if UNITY_EDITOR // So we can skip tasks >:>
            if(Input.GetKey(KeyCode.LeftControl))
            {
                col.isTrigger = true;
            }
            else
            {
                col.isTrigger = false;
            }
        #endif

        //Rotate the door if need to
        if (Moving)
        {
            float targetDir = Open ? TargetOpenRotationY : initRotationY;
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler( new Vector3(0, targetDir, 0)),5f * Time.deltaTime);
            if(transform.rotation == Quaternion.Euler(new Vector3(0, targetDir, 0)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, targetDir, 0));
                Moving = false;
            }

        }
    }
}
