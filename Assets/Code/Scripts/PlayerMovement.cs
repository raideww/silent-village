using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum MovementType
{
    Walking,
    Sprinting,
    Crouching,
    Climbing
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float walkingSpeedMax = 10.0f;
    public float sprintingSpeedMax = 20.0f;
    public float crouchingSpeedMax = 5.0f;
    public float climbingSpeedMax = 10.0f;

    [Header("Forces")]
    public float walkingForce = 5.0f;
    public float sprintingForce = 20.0f;
    public float crouchingForce = 2.0f;
    public float jumpForce = 500.0f;
    public float climbingForce = 5.0f;

    [Header("Timing")]
    public float accelTime = 0.12f;      
    public float stopTime = 0.08f;

    [Header("Drag")]
    public float dragAir = 0.05f;
    public float dragMoving = 6f;
    public float dragIdle = 16f;
    public float dragClimbing = 6f;

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
    private bool onLadder = false;
    private float gravityScaleInitial;
    private bool tryingToClimb = false;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction jumpAction;
    private InputAction climbAction;
    private InputAction verticalAction;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerStamina playerStamina;

    public float MoveValue
    {
        get { return moveValue; }
    }

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("move");
        sprintAction = InputSystem.actions.FindAction("sprint");
        crouchAction = InputSystem.actions.FindAction("crouch");
        jumpAction = InputSystem.actions.FindAction("jump");
        climbAction = InputSystem.actions.FindAction("climb");
        verticalAction = InputSystem.actions.FindAction("vertical");
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStamina = GetComponent<PlayerStamina>();
        gravityScaleInitial = rb.gravityScale;
    }

    void Update()
    {
        moveValue = moveAction.ReadValue<float>();

        // Climbing
        if (climbAction.WasPressedThisFrame())
        {
            tryingToClimb = true;
        }
        else if (climbAction.WasReleasedThisFrame())
        {
            tryingToClimb = false;
        }
        if (tryingToClimb && currentMovementType != MovementType.Climbing && onLadder)
        {
            ChangeMovementType(MovementType.Climbing);
        }
        if (currentMovementType == MovementType.Climbing && !onLadder)
        {
            ChangeMovementType(MovementType.Walking);
        }

        // Sprinting
        if (sprintAction.WasPressedThisFrame())
        {
            ChangeMovementType(MovementType.Sprinting);
        }
        else if (sprintAction.WasReleasedThisFrame() && currentMovementType == MovementType.Sprinting)
        {
            ChangeMovementType(MovementType.Walking);
        }

        // Crouching
        if (currentMovementType != MovementType.Climbing && crouchAction.WasPressedThisFrame())
        {
            ChangeMovementType(MovementType.Crouching);
        }
        else if (crouchAction.WasReleasedThisFrame() && currentMovementType == MovementType.Crouching)
        {
            ChangeMovementType(MovementType.Walking);
        }
        
        // Jump
        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        

        if (tryingToUncrouch && currentMovementType == MovementType.Crouching) TryToUncrouch();
        if (currentMovementType == MovementType.Sprinting && playerStamina.Stamina == 0)
        {
            ChangeMovementType(MovementType.Walking);
        }


        if (!GroundBelow() && currentMovementType != MovementType.Climbing)
        {
            rb.linearDamping = dragAir;
        }
        else if (currentMovementType == MovementType.Climbing)
        {
            rb.linearDamping = dragClimbing;    
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

        if (currentMovementType == MovementType.Climbing)
        {
            float verticalValue = verticalAction.ReadValue<float>();

            rb.AddForceY(ForceNeeded(climbingSpeedMax, climbingForce, verticalValue));
            rb.AddForceX(ForceNeeded(climbingSpeedMax, climbingForce, moveValue));
        }
        else
        {
            float maxSpeed, maxForce;
            switch (currentMovementType)
            {
                case MovementType.Sprinting: maxSpeed = sprintingSpeedMax; maxForce = sprintingForce; break;
                case MovementType.Crouching: maxSpeed = crouchingSpeedMax; maxForce = crouchingForce; break;
                default: maxSpeed = walkingSpeedMax; maxForce = walkingForce; break;
            }

            rb.AddForceX(ForceNeeded(maxSpeed, maxForce, moveValue));
        }
    }
    
    float ForceNeeded(float maxSpeed, float maxForce, float inputValue)
    {
        float target = inputValue * maxSpeed;
        float tau = (Mathf.Abs(moveValue) > 0) ? Mathf.Max(0.01f, accelTime)
                                               : Mathf.Max(0.01f, stopTime);
        float a = (target - rb.linearVelocityX) / tau;
        float forceNeeded = Mathf.Clamp(a * rb.mass, -maxForce, maxForce);
        return forceNeeded;
    }

    void StartSprinting()
    {
        playerStamina.StartDraining();
    }
    void EndSprinting()
    {
        playerStamina.EndDraining();
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

    void StartClimbing()
    {
        rb.gravityScale = 0;
    }
    void EndClimbing()
    {
        rb.gravityScale = gravityScaleInitial;
    }

    void Jump()
    {
        if (GroundBelow())
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        }
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
                    if (GroundAbove())
                    {
                        tryingToUncrouch = true;
                        return;
                    }
                    EndCrouching();
                    break;
                case MovementType.Climbing:
                    if (tryingToClimb && onLadder) return;
                    EndClimbing();
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
                case MovementType.Climbing:
                    StartClimbing();
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadder = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireCube(ceilingCheck.position, ceilingCheckSize);
    }
}
