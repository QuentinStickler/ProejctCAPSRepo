using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{

    [SerializeField] new Transform camera = null;   //new
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float gravity = -25f;
    public float walkSpeed = 5f;
    CharacterController controller;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    float cameraPitch = 0.0f;
    float velocityY = 0f;
    float jumpHeight = 1.5f;

    float sprintSpeed = 10f;
    public float speed;
    float crouchSpeed = 3f;

    bool isGrounded;
    bool isCrouching;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundLayer;


    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;
    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    CapsuleCollider playerCollider;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    [Client]
    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorMode();
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                CmdMove();
            }
        }
        else
        {
            camera.gameObject.SetActive(false);
        }
    }

    [Command]
    private void CmdMove()
    {
        RpcMove();
    }
    [ClientRpc]
    private void RpcMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && velocityY < 0)
        {
            velocityY = -2f;
        }
        CameraUpdate();
        Move();
    }
    void CameraUpdate()
    {
        //vertikaler und horizontaler Maus Input
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), (Input.GetAxis("Mouse Y")));
        //Kamerabewegung dabei smoother machen
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
        //Vertikaler Drehbereich
        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        //Nur bis nach oben und nach unten gucken
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        camera.localEulerAngles = Vector3.right * cameraPitch;

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

    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);
        //transofrm it based off the player transform and scale it by movement speed
        Vector3 move = transform.TransformVector(input) * speed;
        if (controller.isGrounded)
        {
            velocityY = 0f;
        }
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            speed = sprintSpeed;
        }
        if (!Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            speed = walkSpeed;
        }
        if (Input.GetKey(KeyCode.Mouse4))
        {
            controller.height = 1.5f;
            isCrouching = true;
            speed = crouchSpeed;
        }
        if (!Input.GetKey(KeyCode.Mouse4))
        {
            controller.height = 2.0f;
            isCrouching = false;
        }

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            //Formel für Velocity nach unten
            velocityY = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocityY += gravity * Time.deltaTime;
        move.y = velocityY;
        //Bewegung in alle richtungen so schnell wie speed ist
        //Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed + Vector3.up * velocityY;
        controller.Move(move * Time.deltaTime);
    }
}