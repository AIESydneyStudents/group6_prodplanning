using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomManager : MonoBehaviour
{
    public UnityEvent OnRoomComplete;
    public int CompletionThreshhold = 5;

    private int currentCompletion;
    private bool hasFinished;

    private void Update()
    {
        if(!hasFinished)
        {
            if(currentCompletion >= CompletionThreshhold)
            {
                OnRoomComplete.Invoke();
            }
        }
    }

    public void AddComplete(int amount)
    {
        currentCompletion += amount;
    }
}
