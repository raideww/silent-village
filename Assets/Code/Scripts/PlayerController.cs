using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;


    private Vector2 moveValue;
    private Rigidbody2D rb;
    private bool isGrounded;

    public float moveSpeed = 5.0f;
    public float jumpSpeed = 10.0f;

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
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
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
        rb.linearVelocityX = moveValue.x * moveSpeed;
    }

    void OnDrawGizmosSelected()
    {
        // Just to visualize the ground check in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}