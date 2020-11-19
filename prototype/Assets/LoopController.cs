using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoopController : MonoBehaviour
{
    [Range(0f,1f)]
    public float SaturationLevel = 1.0f;
    public UnityEvent OnLoopStart;

}
