using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    GameObject pickedObject;
    Rigidbody pickedObjectRb;
    GameObject pickedObjectParent = null;
    Vector3 pickedObjectRotation;

    RigidbodyConstraints freezeRotation = RigidbodyConstraints.FreezeRotation;
    public GameObject pickupAbleText;

    public LayerMask PickableObjectLayer;
    public LayerMask InteractableLayer;

    public F_PlayerMovement playerMovement;

    bool smoothMove = false;
    bool focusing = false;
    bool fixedRotation = false;
    bool objectZoomed = false;

    public float ArmLength = 4f;
    [SerializeField] float throwStrength = 500f;
    [SerializeField] float pickUpSmooth = 20f;
    [SerializeField] float carrySmooth = 4f;
    [SerializeField] float zoomSmooth = 110f;

    public GameObject ObjectDestination;
    Vector3 defaultLocalObjectHolderPos = new Vector3(0, 0, 1.5f);
    Vector3 zoomedLocalObjectHolderPos = new Vector3(0, 0, 0.5f);

    public bool IsHoldingObject() { return pickedObject != null; }

    void Start()
    {
        //ObjectDestination = GameObject.FindGameObjectWithTag("ObjectHolder");
        playerMovement = FindObjectOfType<F_PlayerMovement>();
    }

    void Update()
    {
        RaycastObjectUpdate();

        if (Input.GetKeyDown(KeyCode.E)) PickUpControl();

        if (pickedObject == null) return; // Won't continue past this point if there's no object.

        if (Input.GetKeyDown(KeyCode.R))
        {
            fixedRotation = true;
            playerMovement.LockPlayerMovement(true);

            ZoomInObject();

            pickedObjectRb.constraints = freezeRotation;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            fixedRotation = false;
            playerMovement.LockPlayerMovement(false);

            ZoomOutObject();

            pickedObjectRb.constraints = RigidbodyConstraints.None;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Rigidbody objRb = pickedObjectRb;
            PickUpControl();

            objRb.AddForce(playerMovement.playerCamera.transform.forward * throwStrength);
        }

        if (smoothMove) // When the object is first picked up..
        {
            pickedObject.transform.position = Vector3.MoveTowards(pickedObject.transform.position,
                ObjectDestination.transform.position, Time.deltaTime * pickUpSmooth);

            if (pickedObject.transform.position == ObjectDestination.transform.position)
            {
                pickedObject.transform.parent = ObjectDestination.transform;

                smoothMove = false;
                focusing = true;
            }
        }

        if (!objectHolderPosSet) SmoothMoveObjectHolderUpdate();
    }

    void FixedUpdate() // FixedUpdate fixes jitter issue.
    {
        if (focusing) // Object has been picked up and is now focusing on its position.
        {
            pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, ObjectDestination.transform.position,
                Time.deltaTime * carrySmooth);

            if (fixedRotation)
            {
                //pickedObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.fixedDeltaTime * 150f);
                float rotX = Input.GetAxis("Mouse X") * 5 * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * 5 * Mathf.Deg2Rad;

                pickedObject.transform.RotateAround(Vector3.up, -rotX);
                pickedObject.transform.RotateAround(Vector3.right, rotY);

                pickedObjectRotation = pickedObject.transform.eulerAngles;
            }
            else
                pickedObject.transform.eulerAngles = pickedObjectRotation;// So that the object doesn't spin, spin and spin.

            if (pickedObjectRb.velocity != Vector3.zero) pickedObjectRb.velocity = Vector3.zero; // Seemed to fix the rest of the issues lol
        }
    }

    public void PickUpControl()
    {
        if (pickedObject == null)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, ArmLength, PickableObjectLayer))
            { // Raycast from camera perspective, this may need to be edited if multiple cameras are used.
                if(InteractableLayer == (InteractableLayer | (1 << hit.collider.gameObject.layer)))
                {
                    Interactable interact = hit.collider.gameObject.GetComponent<Interactable>();
                    if(interact != null)
                    {
                        interact.OnInteract.Invoke();
                    }
                }
                else
                {
                    pickedObject = hit.collider.gameObject;
                    smoothMove = true;

                    pickedObjectRb = pickedObject.GetComponent<Rigidbody>();
                    pickedObjectRb.useGravity = false;

                    if (pickedObject.transform.parent != null)
                        pickedObjectParent = pickedObject.transform.parent.gameObject;
                    else
                        pickedObjectParent = null;

                    pickedObjectRotation = pickedObject.transform.eulerAngles;
                }
            }
        }
        else // Reset everything.
        {
            if (pickedObjectParent != null)
                pickedObject.transform.parent = pickedObjectParent.transform;
            else
                pickedObject.transform.parent = null;

            pickedObjectParent = null;
            pickedObject = null;

            pickedObjectRb.useGravity = true;
            pickedObjectRb = null;

            smoothMove = false;
            focusing = false;
            fixedRotation = false;

            playerMovement.LockPlayerMovement(false);
            ObjectDestination.transform.localPosition = defaultLocalObjectHolderPos;
        }
    }

    void ZoomInObject()
    {
        objectZoomed = true;
        setObjectHolderLerpPos = zoomedLocalObjectHolderPos;

        objectHolderPosSet = false;
    }

    void ZoomOutObject()
    {
        objectZoomed = false;
        setObjectHolderLerpPos = defaultLocalObjectHolderPos;

        objectHolderPosSet = false;
    }

    Vector3 setObjectHolderLerpPos;
    bool objectHolderPosSet = true;
    void SmoothMoveObjectHolderUpdate()
    {
        ObjectDestination.transform.localPosition = Vector3.Lerp(setObjectHolderLerpPos,
            ObjectDestination.transform.localPosition, Time.deltaTime * zoomSmooth);

        //ObjectDestination.transform.localPosition = setObjectHolderLerpPos;

        if (ObjectDestination.transform.localPosition == setObjectHolderLerpPos) objectHolderPosSet = true;
    }

    void RaycastObjectUpdate() // DIRTY DIRTY DIRTY
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, ArmLength, PickableObjectLayer))
        {
            if (InteractableLayer == (InteractableLayer | (1 << hit.collider.gameObject.layer)))
            {
                // show object is interactable
            }
            else
            {
                if (IsHoldingObject()) ToggleDisplayText(false);
                else ToggleDisplayText(true);
            }
        }
        else
        {
            ToggleDisplayText(false);
        }
    }

    void ToggleDisplayText(bool toggle)
    {
        if(pickupAbleText != null)
            pickupAbleText.SetActive(toggle);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward);
    }
}
