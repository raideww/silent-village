using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;


    private Vector2 moveValue;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting;
    private SpriteRenderer spriteRenderer;

    public float moveSpeed = 5.0f;
    public float jumpSpeed = 10.0f;
    public float sprintSpeed = 10.0f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        jumpAction = InputSystem.actions.FindAction("jump");
        sprintAction = InputSystem.actions.FindAction("sprint");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        if (sprintAction.WasPerformedThisFrame())
        {
            isSprinting = true;
        }
        else if (sprintAction.WasCompletedThisFrame())
        {
            isSprinting = false;
        }
    }

    private void FixedUpdate()
    {
        Walking();
    }

    private void Jump()
    {   isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            rb.linearVelocityY = rb.linearVelocityY + jumpSpeed;
        }
    }

    private void Walking()
    {


        if (moveValue.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveValue.x < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (!isSprinting)
        {
            rb.linearVelocityX = moveValue.x * moveSpeed;
        }
        else
        {
            rb.linearVelocityX = moveValue.x * sprintSpeed;
        }

    }

    void OnDrawGizmosSelected()
    {
        // Just to visualize the ground check in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}