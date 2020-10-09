using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float speed = 12f;
    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded = false;

    private Camera childCamera;

    private Rigidbody heldObject = null;
    private Collider heldObjectCollider = null;
    private float distenceFromHeldObject = 0;
    private Vector3 heldObjectVelocity = new Vector3();

    private void Start()
    {
        childCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position,groundDistance,groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xMov + transform.forward * zMov;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.E))
        {
            PickupObject();
        }        
    }

    RaycastHit[] hits;

    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            Vector3 targetPoint = childCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            targetPoint += childCamera.transform.forward * distenceFromHeldObject;

            Vector3 move = Vector3.MoveTowards(heldObject.transform.position, targetPoint, 50 * Time.deltaTime);

            Vector3 center = heldObjectCollider.bounds.center + move;
            Vector3 extents = heldObjectCollider.bounds.extents;

            hits = Physics.BoxCastAll(center, extents, childCamera.transform.forward, Quaternion.identity, distenceFromHeldObject,LayerMask.GetMask("Ground"));

            bool hasHit = false;

            foreach(RaycastHit hit in hits)
            {
                if(hit.collider != null)
                {
                    hasHit = true;
                    break;
                }
            }

            if(!hasHit)
            {
                heldObject.MovePosition(move);
                heldObjectVelocity = move - heldObject.transform.position;
            }
        }
    }

    void PickupObject()
    {
        if (heldObject == null)
        {
            RaycastHit hit;
            //Check if object in way of raycast
            if(Physics.Raycast(new Ray(childCamera.transform.position,childCamera.transform.forward),out hit,500))
            {
                if ((LayerMask.GetMask("Pickupable") & (1 << hit.collider.gameObject.layer)) > 0)
                {
                    heldObject = hit.collider.gameObject.GetComponent<Rigidbody>();
                    heldObjectCollider =  heldObject.gameObject.GetComponent<Collider>();
                    heldObject.useGravity = false;
                    heldObjectVelocity = Vector3.zero;

                    distenceFromHeldObject = hit.distance;

                    distenceFromHeldObject = Mathf.Max(2.5f,distenceFromHeldObject);
                }
            }
        }
        else
        {

            //heldObject.velocity = Vector3.zero;
            heldObject.useGravity = true;
            heldObject.velocity = heldObjectVelocity;
            heldObjectCollider = null;
            heldObject = null;
        }
    }
}
