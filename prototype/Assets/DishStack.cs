using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishStack : MonoBehaviour
{
    [Tooltip("The empty transforms of each plate in the stack.")]
    public Transform BaseTransform;
    public float SpaceBetweenPlates = 0.03125f;
    public SnapPickupInArea PickupConroller;
    public GameObject PlateIndicator;

    [HideInInspector]
    public List<GameObject> stackedPlates = new List<GameObject>();

    private int stackIndex = 0;
    private float BaseY = 0;
    

    private void Start()
    {
        BaseY = BaseTransform.position.y;
        if (BaseTransform == null) BaseTransform = GetComponentInChildren<Transform>();
    }

    public void IncreaseStack()
    {
        if(stackIndex == 0)
        {
            //Destory indicator
            if(PlateIndicator != null)
            {
                Destroy(PlateIndicator);
            }
        }

        stackIndex++;
        if (PickupConroller.SnappingObject != null) stackedPlates.Add(PickupConroller.SnappingObject);

        PickupConroller.transform.position = BaseTransform.position;
        BaseTransform.position = new Vector3(BaseTransform.position.x,BaseY + (SpaceBetweenPlates * stackedPlates.Count), BaseTransform.position.z);
    }

    public GameObject PopDish()
    {
        if (stackedPlates == null || stackedPlates.Count == 0) return null;

        GameObject obj = stackedPlates[stackedPlates.Count - 1];
        stackedPlates.RemoveAt(stackedPlates.Count - 1);
        BaseTransform.position = new Vector3(BaseTransform.position.x, BaseY + (SpaceBetweenPlates * stackedPlates.Count), BaseTransform.position.z);

        return obj;
    }

    public bool HasDishes()
    {
        return (stackedPlates != null && stackedPlates.Count > 0);
    }
}
