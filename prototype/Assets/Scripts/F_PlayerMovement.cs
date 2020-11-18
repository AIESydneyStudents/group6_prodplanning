using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_PlayerMovement : MonoBehaviour
{
    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera; // Acutal camera
    public CinemachineVirtualCamera VirtualCamera; // Cinamachine cam
    public float lookSpeed = 2.0f;
    public float lookXLimit = 20.0f;

    CharacterController characterController;
    [HideInInspector]
    public Vector3 Velocity = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    public bool IsGameplay { get {return playerState == PlayerState.Gameplay;} }

    public enum PlayerState
    {
        Gameplay,
        Cutscene
    }

    public PlayerState PlayerCurrentState { get {return playerState; } }
    private PlayerState playerState = PlayerState.Gameplay;
    private CinemachineVirtualCamera otherCamera;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ChangeState(PlayerState newState)
    {
        playerState = newState;
    }

    void Update()
    {
        //Run the correct state for the player
        switch(playerState)
        {
            case PlayerState.Gameplay: GameplayState(); break;
            case PlayerState.Cutscene: CutsceneState(); break;
        }
    }

    public void GameplayState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            canMove = false;
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            canMove = true;
        }

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            Velocity = (forward * curSpeedX) + (right * curSpeedY);
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        Velocity.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(Velocity * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            VirtualCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }

    public void CutsceneState()
    {
        Velocity.x = 0;
        Velocity.z = 0;
        //Cursor.lockState = CursorLockMode.None;
        canMove = false;
        

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        Velocity.y -= gravity * Time.deltaTime;

        if(teleMoving)
        {
            Velocity = portalVelocity;
            teleMovingTimer -= Time.deltaTime;
            if(teleMovingTimer <= 0)
            {
                teleMoving = false;
                playerState = PlayerState.Gameplay;
            }
        }

        // Move the controller
        characterController.Move(Velocity * Time.deltaTime);
    }

    public void ChangePerspective(CinemachineVirtualCamera  newCamera) // Used for some tasks
    {
        if(newCamera == null)
        {
            if (otherCamera != null) otherCamera.Priority = 0;
            VirtualCamera.m_Priority = 10;
            VirtualCamera.enabled = true;
            playerState = PlayerState.Gameplay;
            otherCamera = null;

            Cursor.lockState = CursorLockMode.Locked;
            canMove = true;

            return;
        }

        if (newCamera != otherCamera)
        {
            newCamera.Priority = 10;
            VirtualCamera.m_Priority = 0;
            VirtualCamera.enabled = false;
        }
        else
        {
            if(otherCamera != null) otherCamera.Priority = 0;
            VirtualCamera.m_Priority = 10;
            VirtualCamera.enabled = true;
        }

        otherCamera = newCamera;
        playerState = PlayerState.Cutscene;
    }

    private Vector3 portalVelocity;
    private bool  teleMoving = false;
    private float teleMovingTimer = 0;

    public void HasTeleported()
    {
        portalVelocity = Velocity;
        playerState = PlayerState.Cutscene;
        teleMoving = true;
        teleMovingTimer = 0.25f;
    }

    public void LockPlayerMovement(bool _lock) { canMove = !_lock; }
}