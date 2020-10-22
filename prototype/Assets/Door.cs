using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    bool Open = false;
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ToggleDoor()
    {
        if(Open)
        {
            animator.Play("ani_door_close");
        }
        else
        {
            animator.Play("ani_door_open");
        }

        Open = !Open;
    }
}
