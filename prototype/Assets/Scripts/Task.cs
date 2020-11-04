using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Task : MonoBehaviour
{
    public UnityEvent OnTaskFinished;
    public UnityEvent OnTaskProgressed;

    protected bool taskFinished = false;
    protected bool taskRunning = false;

    protected void TaskFinished()
    {
        taskFinished = true;
        taskRunning = false;
        OnTaskFinished.Invoke();
    }
}
