using System;
using Unity.VisualScripting;
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
    public float walkingForce = 5.0f;
    public float walkingSpeedMax = 10.0f;
    public float sprintingForce = 20.0f;
    public float sprintingSpeedMax = 20.0f;
    public float crouchingForce = 2.0f;
    public float crouchingSpeedMax = 5.0f;

    [Header("Timing")]
    public float accelTime = 0.12f;      
    public float stopTime = 0.08f;

    [Header("Drag")]
    public float dragAir = 0.05f;
    public float dragMoving = 6f;
    public float dragIdle = 16f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Ceiling Check")]
    public Transform ceilingCheck;
    public Vector2 ceilingCheckSize = new Vector2(0.7f, 0.4f);

    [Header("Ground Layer")]
    public LayerMask groundLayer;


    private MovementType currentMovementType = MovementType.Walking;
    private float moveValue;
    private bool tryingToUncrouch = false;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        sprintAction = InputSystem.actions.FindAction("sprint");
        crouchAction = InputSystem.actions.FindAction("crouch");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (sprintAction.WasPressedThisFrame())
        {
            ChangeMovementType(MovementType.Sprinting);
        }
        else if (sprintAction.WasReleasedThisFrame() && currentMovementType == MovementType.Sprinting)
        {
            ChangeMovementType(MovementType.Walking);
        }
        if (crouchAction.WasPressedThisFrame())
        {
            ChangeMovementType(MovementType.Crouching);
        }
        else if (crouchAction.WasReleasedThisFrame() && currentMovementType == MovementType.Crouching)
        {
            ChangeMovementType(MovementType.Walking);
        }
    }

    void FixedUpdate()
    {
        if (tryingToUncrouch && currentMovementType == MovementType.Crouching) TryToUncrouch();

        moveValue = moveAction.ReadValue<float>();
        bool grounded = GroundBelow();

        if (!grounded)
        {
            rb.linearDamping = dragAir;
        }
        else
        {
            if (moveValue == 0 || Math.Sign(moveValue) != Math.Sign(rb.linearVelocityX))
            {
                rb.linearDamping = dragIdle;
            }
            else
            {
                rb.linearDamping = dragMoving;
            }
        }

        float maxSpeed, maxForce;
        switch (currentMovementType)
        {
            case MovementType.Sprinting: maxSpeed = sprintingSpeedMax; maxForce = sprintingForce; break;
            case MovementType.Crouching: maxSpeed = crouchingSpeedMax; maxForce = crouchingForce; break;
            default:                     maxSpeed = walkingSpeedMax;   maxForce = walkingForce;   break;
        }

        float target = moveValue * maxSpeed;

        // Быстрее выходим на target при нажатии, быстрее останавливаемся при отпускании
        float tau = (Mathf.Abs(moveValue) > 0) ? Mathf.Max(0.01f, accelTime)
                                               : Mathf.Max(0.01f, stopTime);

        // Ускорение к цели
        float a = (target - rb.linearVelocityX) / tau;
        float forceNeeded = Mathf.Clamp(a * rb.mass, -maxForce, maxForce);

        rb.AddForceX(forceNeeded);
    }

    void StartSprinting()
    {

    }
    void EndSprinting()
    {
        
    }

    void StartCrouching()
    {
        tryingToUncrouch = false;
        animator.SetBool("isCrouching", true);
        Vector3 newPosition = transform.position;
        newPosition.y -= 0.3f;
        transform.position = newPosition;
    }
    void EndCrouching()
    {   
        animator.SetBool("isCrouching", false);
        Vector3 newPosition = transform.position;
        newPosition.y += 0.3f;
        transform.position = newPosition;
    }
    void TryToUncrouch()
    {
        ChangeMovementType(MovementType.Walking);
    }

    void ChangeMovementType(MovementType newMovementType)
    {
        if (newMovementType != currentMovementType)
        {
            // Ending Previous Movement
            switch (currentMovementType)
            {
                case MovementType.Sprinting:
                    EndSprinting();
                    break;
                case MovementType.Crouching:
                    Debug.Log(GroundAbove());
                    if (GroundAbove())
                    {
                        tryingToUncrouch = true;
                        return;
                    }
                    EndCrouching();
                    break;
            }
            
            // Starting New Movement
            switch (newMovementType)
            {
                case MovementType.Sprinting:
                    StartSprinting();
                    break;
                case MovementType.Crouching:
                    StartCrouching();
                    break;
            }

            currentMovementType = newMovementType;
        }
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
