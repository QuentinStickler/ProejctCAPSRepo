using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : NetworkBehaviour
{
    float yVelocity = 0f;
    [Range(-5f,-25f)]
    public float gravity = -15f;
    //the speed of the player movement
    public float movementSpeed;
    //jump speed
    [Range(5f,15f)]
    public float jumpSpeed = 9f;
    bool isCrouching;
    private float sprintingSpeed = 6f;
    public float walkingSpeed = 4f;
    public float shootingSpeed = 3.25f;
    public float crouchingSpeed = 2f;
    public float aimingSpeed = 2.75f;
    public bool isAiming;
    public bool isShooting;


    public Transform cameraTransform;
    public GameObject gun;
    float pitch = 0f;
    [Range(1f,90f)]
    public float maxPitch = 85f;
    [Range(-1f, -90f)]
    public float minPitch = -85f;
    [Range(0.5f, 5f)]
    public float mouseSensitivity = 2f;
    CharacterController cc;

    private void Start()
    {
        if (!isLocalPlayer) { return; }
        cc = GetComponent<CharacterController>();
        //shoot = GetComponent<PlayerShoot>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) { return; }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorMode();
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Look();
                Move();
            }  
    }

    public void SetAiming (bool aiming)
    {
        isAiming = aiming;
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    void Look()
    {
        //get the mouse inpuit axis values
        float xInput = Input.GetAxis("Mouse X") * mouseSensitivity;
        float yInput = Input.GetAxis("Mouse Y") * mouseSensitivity;
        //turn the whole object based on the x input
        transform.Rotate(0, xInput, 0);
        //now add on y input to pitch, and clamp it
        pitch -= yInput;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        //create the local rotation value for the camera and set it
        Quaternion rot = Quaternion.Euler(pitch, 0, 0);
        cameraTransform.localRotation = rot;
    }

    void Move()
    {
        //update speed based onn the input
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);
        //transofrm it based off the player transform and scale it by movement speed
        //is it on the ground
        if (cc.isGrounded)
        {
            yVelocity = 0;
            //check for jump here
            if (Input.GetButtonDown("Jump"))
            {
                yVelocity = jumpSpeed;
            }
        }
        if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) && !isCrouching && !isAiming) && !isShooting)
        {
            movementSpeed = sprintingSpeed;
        }
        if ((!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.W)) && !isCrouching && !isShooting)
        {
            movementSpeed = walkingSpeed;
        }
        if (isShooting)
        {
            movementSpeed = shootingSpeed;
        }
        if (isAiming)
        {
            movementSpeed = aimingSpeed;
        }
        if (Input.GetKey(KeyCode.Mouse4))
        {
            cc.height = 1.5f;
            isCrouching = true;
            movementSpeed = crouchingSpeed;
        }
        if (!Input.GetKey(KeyCode.Mouse4))
        {
            cc.height = 2.0f;
            isCrouching = false;
        }
        Vector3 move = transform.TransformVector(input) * movementSpeed;

        //now add the gravity to the yvelocity
        yVelocity += gravity * Time.deltaTime;
        move.y = yVelocity;
        //and finally move
        cc.Move(move * Time.deltaTime);
    }
}