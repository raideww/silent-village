using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;


    private Vector2 moveValue;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private Vector3 previousScale;
    private SpriteRenderer spriteRenderer;
    private Renderer rend;
    

    public float moveSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float crouchSpeed = 2.5f;
    public Vector3 crouchScale = new Vector3(1.0f, 0.7f, 1.0f);
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
        sprintAction = InputSystem.actions.FindAction("sprint");
        crouchAction = InputSystem.actions.FindAction("crouch");
        previousScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        if (crouchAction.WasPressedThisFrame())
        {
            // Getting previous scale to take into account when calculating height compensation
            // because when player's size changes it changes in center which leaves player off the ground
            // thats why we should compensate this change
            Vector2 beforeCrouchSize = rend.bounds.size;

            // Changing player's size
            previousScale = transform.localScale;
            transform.localScale = Vector3.Scale(transform.localScale, crouchScale);
            isCrouching = true;

            // Calculating height compensation by getting the difference of player's height before changing size and after
            // and dividing it by 2 because we only need to account space under the player
            Vector2 afterCrouchSize = rend.bounds.size;
            float heightCompensation = (beforeCrouchSize.y - afterCrouchSize.y) / 2;
            Vector2 compensatedPosition = transform.localPosition - new Vector3(0, heightCompensation, 0);
            transform.localPosition = compensatedPosition;
        }
        else if (crouchAction.WasReleasedThisFrame())
        {
            Vector2 beforeCrouchSize = rend.bounds.size;

            transform.localScale = previousScale;
            isCrouching = false;

            Vector2 afterCrouchSize = rend.bounds.size;
            float heightCompensation = (beforeCrouchSize.y - afterCrouchSize.y) / 2;
            Vector2 compensatedPosition = transform.localPosition - new Vector3(0, heightCompensation, 0);
            transform.localPosition = compensatedPosition;
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
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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

        if (isCrouching)
        {
            rb.linearVelocityX = moveValue.x * crouchSpeed;
        }
        else if (isSprinting)
        {
            rb.linearVelocityX = moveValue.x * sprintSpeed;
        }
        else
        {
            rb.linearVelocityX = moveValue.x * moveSpeed;
        }

    }

    void OnDrawGizmosSelected()
    {
        // Just to visualize the ground check in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}