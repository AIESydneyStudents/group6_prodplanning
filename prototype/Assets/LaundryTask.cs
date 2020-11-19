using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LaundryTask : MonoBehaviour
{
    public UnityEvent OnPickedUp;

   public void PickedUp()
    {
        OnPickedUp.Invoke();

    }
}
