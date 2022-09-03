using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedPlayerMovement : MonoBehaviour
{

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public Animator anim;
    public float walkSpeed = 5;
    public float runSpeed = 8;
    public float crouchSpeed = 3;
    public float jumpForce = 5;
    public float gravity = -9.8f;
    public float crouchHeight = 1.4f;
    public float standHeight = 2;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;
    private Vector3 moveDirection;
    private CharacterController characterController;
    private bool playerGrounded;
    private bool running;
    private bool crouching;
    [HideInInspector]
    public Vector3 move;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();  
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(runKey))
        {
            running = true;
        }
        else
        {
            running = false;
        }


        if(Input.GetKeyDown(crouchKey))
        {
            crouching = !crouching;
            if(crouching)
            {
                characterController.height = crouchHeight;
                characterController.center.Set(characterController.center.x, crouchHeight / 2, characterController.center.z);
                groundCheck.position = transform.position + -transform.up * crouchHeight / 2;
            }
            else
            {
                characterController.height = standHeight;
                characterController.center.Set(characterController.center.x, standHeight / 2, characterController.center.z);
                groundCheck.position = transform.position + -transform.up * standHeight / 2;
            }
        }

        playerGrounded =  Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (playerGrounded && moveDirection.y < 0)
        {
            moveDirection.y = 0f;
        }



        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        anim.SetBool("Crouching", crouching);
        anim.SetFloat("Horizontal", move.x);
        anim.SetFloat("Vertical", move.z);
        move = transform.TransformDirection(move);
        if(running)
        {
            characterController.Move(move * Time.deltaTime * runSpeed);
        }
        else
        {
            characterController.Move(move * Time.deltaTime * walkSpeed);
        }


        if(Input.GetKeyDown(jumpKey) && playerGrounded)
        {
            moveDirection.y += Mathf.Sqrt(jumpForce * -3.0f * gravity);
        }

        moveDirection.y += gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
    }
}
