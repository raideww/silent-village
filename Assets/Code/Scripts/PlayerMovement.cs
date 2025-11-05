using System;
using UnityEngine;
using UnityEngine.InputSystem;

enum MovementType
{
    Walking,
    Sprinting,
    Crouching
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float walkingForce = 20.0f;
    public float walkingSpeedMax = 10.0f;

    [Header("Drag")]
    public float dragAir = 0.05f;
    public float dragMoving = 0.2f;
    public float dragIdle = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public Vector2 ceilingCheckSize = new Vector2(0.7f, 0.4f);

    [Header("Ground Layer")]
    public LayerMask groundLayer;


    private MovementType currentMovementType = MovementType.Walking;
    private Vector2 moveValue;

    private InputAction moveAction;
    private InputAction sprintAction;
    private Rigidbody2D rb;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        moveValue = moveAction.ReadValue<Vector2>();
        if (!GroundBelow())
        {
            rb.linearDamping = dragAir;
        }
        else
        {
            if (moveValue == Vector2.zero || Math.Sign(moveValue.x) != Math.Sign(rb.linearVelocityX))
            {
                rb.linearDamping = dragIdle;
            }
            else
            {
                rb.linearDamping = dragMoving;
            }
        }


        if (currentMovementType == MovementType.Walking)
        {
            if (Math.Abs(rb.linearVelocityX) < walkingSpeedMax)
            {
                rb.AddForce(walkingForce * moveValue, ForceMode2D.Force);
            }
        }
        else if (currentMovementType == MovementType.Sprinting)
        {

        }
        else if (currentMovementType == MovementType.Crouching)
        {

        }

        Debug.Log(rb.linearVelocity);
    }

    bool GroundBelow()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    bool GroundAbove()
    {
        return Physics2D.OverlapBox(ceilingCheck.position, ceilingCheckSize, 0f, groundLayer);
    }
}
