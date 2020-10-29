using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishStack : MonoBehaviour
{
    [Tooltip("The empty transforms of each plate in the stack.")]
    public Transform[] PlateStackPoints;
    public SnapPickupInArea PickupConroller;

    [HideInInspector]
    public List<GameObject> stackedPlates = new List<GameObject>();
    public bool Done { get {return done;} }

    private int stackIndex = 0;
    private bool done = false;

    public void IncreaseStack()
    {
        if (done) return;

        stackIndex++;
        if (PickupConroller.SnappingObject != null) stackedPlates.Add(PickupConroller.SnappingObject);

        if (stackIndex < PlateStackPoints.Length)
        {
            PickupConroller.TargetTransform = PlateStackPoints[stackIndex];
            
        }
        else
        {
            done = true;
            stackedPlates.Reverse(); // Reverse list so the top one is first.
        }
    }
}
