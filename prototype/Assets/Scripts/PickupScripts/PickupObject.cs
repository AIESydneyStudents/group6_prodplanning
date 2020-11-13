using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    GameObject pickedObject;
    Rigidbody pickedObjectRb;
    GameObject pickedObjectParent = null;
    Vector3 pickedObjectRotation;

    InspectionEvent pickedObjectInspectEvent = null;

    RigidbodyConstraints freezeRotation = RigidbodyConstraints.FreezeRotation;
    public PickupTextPrompt pickupAbleText;

    public LayerMask PickableObjectLayer;
    public LayerMask InteractableLayer;

    public F_PlayerMovement playerMovement;

    public Material SelectedGlowMaterial;

    bool smoothMove = false;
    bool focusing = false;
    bool fixedRotation = false;
    bool objectZoomed = false;
    bool isObjectDesiredInspection = false; // Does the object have a force rotation script?

    public float ArmLength = 4f; // Ray cast length for pickup
    [SerializeField] float pickUpSmooth = 20f; // Lerp smooth value
    [SerializeField] float carrySmooth = 4f; // Lerp smooth value
    [SerializeField] float throwStrength = 500f; // Throwing object

    public float PickedRotationSpeed = 4f; // Player rotation's of object during inspection
    [SerializeField] float zoomSmooth = 110f; // Lerp smooth value

    public GameObject ObjectDestination;
    public Vector3 defaultLocalObjectHolderPos = new Vector3(0, 0, 1.2f);
    Vector3 zoomedLocalObjectHolderPos;

    public bool IsHoldingObject() { return pickedObject != null; }

    private bool hasInteracted = false;
    private GameObject highlightingObject;

    void Start()
    {
        //ObjectDestination = GameObject.FindGameObjectWithTag("ObjectHolder");
        playerMovement = FindObjectOfType<F_PlayerMovement>();

        zoomedLocalObjectHolderPos = new Vector3(0, 0, defaultLocalObjectHolderPos.z / 2);
    }

    void Update()
    {
        UniversalStaticFunctions.lol();
        RaycastObjectUpdate();

        if (Input.GetMouseButtonDown(0) && playerMovement.IsGameplay) PickUpControl();

        if (Input.GetMouseButtonUp(0) && playerMovement.IsGameplay)
        {
            PickUpControl();
            hasInteracted = false;
        }

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
    }

    void FixedUpdate() // FixedUpdate fixes jitter issue.
    {
        if (smoothMove) // When the object is first picked up..
        {
            pickedObject.transform.position = Vector3.MoveTowards(pickedObject.transform.position,
                ObjectDestination.transform.position, Time.fixedDeltaTime * pickUpSmooth);

            if (pickedObject.transform.position == ObjectDestination.transform.position)
            {
                pickedObject.transform.parent = ObjectDestination.transform;

                smoothMove = false;
                focusing = true;
            }
        }

        if (!objectHolderPosSet) SmoothMoveObjectHolderUpdate();

        if (focusing) // Object has been picked up and is now focusing on its position.
        {
            pickedObject.transform.position = Vector3.Lerp(pickedObject.transform.position, ObjectDestination.transform.position,
                Time.fixedDeltaTime * carrySmooth);

            bool check = false;
            if (fixedRotation)
            {
                if (isObjectDesiredInspection)
                {
                    pickedObjectInspectEvent.IsInspected = true;
                    if (!pickedObjectInspectEvent.PositionLocked)
                        check = true;
                }

                if (!check)
                {
                    //pickedObject.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.fixedDeltaTime * 150f);
                    //float rotX = Input.GetAxis("Mouse X") * PickedRotationSpeed * Mathf.Deg2Rad;
                    //float rotZ = Input.GetAxis("Mouse Y") * PickedRotationSpeed * Mathf.Deg2Rad;

                    //pickedObject.transform.RotateAround(Vector3.down, -rotX * Time.fixedDeltaTime);
                    //pickedObject.transform.RotateAround(Vector3.right, rotZ * Time.fixedDeltaTime);

                    //pickedObject.transform.rotation = Quaternion.Lerp(pickedObject.transform.rotation,
                    //    Quaternion.Euler(pickedObject.transform.rotation.eulerAngles.x + Input.GetAxis("Vertical"),
                    //    pickedObject.transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal"),
                    //    pickedObject.transform.rotation.eulerAngles.y - pickedObject.transform.rotation.eulerAngles.x), PickedRotationSpeed);

                    //pickedObjectRotation = pickedObject.transform.eulerAngles;

                    //Vector3 v3 = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 1f);
                    //Quaternion qTo = Quaternion.LookRotation(v3);

                    //pickedObject.transform.rotation = Quaternion.Lerp(pickedObject.transform.rotation,
                    //    qTo, PickedRotationSpeed * Time.fixedDeltaTime);


                    pickedObject.transform.rotation *= Quaternion.Euler(Input.GetAxis("Mouse X"), 1f, Input.GetAxis("Mouse Y"));
                }
            }
            else
            {
                if (isObjectDesiredInspection)
                {
                    pickedObjectInspectEvent.IsInspected = false;
                    if (!pickedObject.GetComponent<InspectionEvent>().PositionLocked) // So that this doesn't conflict with the lerping in InspectionEvent
                    {
                        pickedObjectRotation = pickedObjectInspectEvent.transform.eulerAngles;
                        pickedObject.transform.eulerAngles = pickedObjectRotation; // So that the object doesn't spin, spin and spin.
                    }
                }
                else
                    pickedObject.transform.eulerAngles = pickedObjectRotation; // So that the object doesn't spin, spin and spin.
            }

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
                if (InteractableLayer == (InteractableLayer | (1 << hit.collider.gameObject.layer)))
                { 
                    Interactable interact = hit.collider.gameObject.GetComponent<Interactable>();
                    if (!hasInteracted && interact != null)
                    {
                        hasInteracted = true;
                        interact.OnInteract.Invoke();
                    }
                }
                else
                {
                    pickedObject = hit.collider.gameObject;
                    hasInteracted = true;

                    smoothMove = true;

                    pickedObjectRb = hit.collider.attachedRigidbody;
                    pickedObjectRb.useGravity = false;

                    if (pickedObject.transform.parent != null)
                        pickedObjectParent = pickedObject.transform.parent.gameObject;
                    else
                        pickedObjectParent = null;

                    pickedObjectRotation = pickedObject.transform.eulerAngles;

                    if (pickedObject.GetComponent<InspectionEvent>() != null)
                    {
                        pickedObjectInspectEvent = pickedObject.GetComponent<InspectionEvent>();
                        isObjectDesiredInspection = true;
                    }
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

            pickedObjectRb.constraints = RigidbodyConstraints.None;
            pickedObjectRb.useGravity = true;
            pickedObjectRb = null;

            isObjectDesiredInspection = false;
            if(pickedObjectInspectEvent != null)
            {
                pickedObjectInspectEvent.PositionLocked = false;
                pickedObjectInspectEvent.IsInspected = false;

                pickedObjectInspectEvent = null;
            }

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
            ObjectDestination.transform.localPosition, Time.fixedDeltaTime * zoomSmooth);

        //ObjectDestination.transform.localPosition = setObjectHolderLerpPos;

        if (ObjectDestination.transform.localPosition == setObjectHolderLerpPos) objectHolderPosSet = true;
    }

    void RaycastObjectUpdate()
    {
        RaycastHit hit;
        if (playerMovement.PlayerCurrentState == F_PlayerMovement.PlayerState.Gameplay && 
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, ArmLength, PickableObjectLayer))
        {
            if (InteractableLayer == (InteractableLayer | (1 << hit.collider.gameObject.layer)))
                pickupAbleText.Text.text = "Press 'Left Mouse' to interact with this object";
            else
                pickupAbleText.Text.text = "Hold 'Left Mouse' to pickup this object";

            if (IsHoldingObject())
            {
                ResetHighlightedObject();
                ToggleDisplayText(false);
            }
            else
            {
                ToggleDisplayText(true);
                SetHighlightedObject(hit.collider.gameObject);
            }
        }
        else
        {
            ResetHighlightedObject();
            ToggleDisplayText(false);
        }
    }

    private MeshRenderer highlightRenderer;
    private Material highlightOriginalMaterial;

    void SetHighlightedObject(GameObject obj)
    {
        //Reset the highlighting to disable it on other objects
        if (highlightingObject != obj)
        {
            ResetHighlightedObject();

            highlightingObject = obj;
            highlightRenderer = obj.GetComponent<MeshRenderer>();
            highlightOriginalMaterial = highlightRenderer.material;

            SelectedGlowMaterial.CopyPropertiesFromMaterial(highlightOriginalMaterial);

            highlightRenderer.material = SelectedGlowMaterial;
        }
    }

    void ResetHighlightedObject()
    {
        if(highlightingObject != null)
        {
            highlightRenderer.material = highlightOriginalMaterial;
        }

        highlightingObject          = null;
        highlightRenderer           = null;
        highlightOriginalMaterial   = null;
    }

    void ToggleDisplayText(bool toggle)
    {
        if (pickupAbleText != null)
        {
            if (toggle)
            {
                pickupAbleText.EnableText();
            }
            else
            {
                pickupAbleText.DisableText();
            }
        }

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward);
    //}
}
