using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

enum MovementType
{
    Walking,
    Sprinting,
    Crouching,
    Climbing,
    Idle,
    Jumping
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
    public float dashForce = 500.0f;

    [Header("Cooldown")]
    public float dashCooldown = 3.0f;

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
    private float verticalValue;
    private bool tryingToUncrouch = false;
    private bool onLadder = false;
    private float gravityScaleInitial;
    private bool tryingToClimb = false;
    private bool isFacingRight = true;
    private float dashCooldownRemained = 0;
    private bool isGoingUp = false;
    private bool isFallingDown = false;

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction jumpAction;
    private InputAction climbAction;
    private InputAction verticalAction;
    private InputAction dashAction;

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
        dashAction = InputSystem.actions.FindAction("dash");
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStamina = GetComponent<PlayerStamina>();
        gravityScaleInitial = rb.gravityScale;
    }

    void Update()
    {
        dashCooldownRemained = Mathf.Max(0, dashCooldownRemained - Time.deltaTime);
        moveValue = moveAction.ReadValue<float>();

        if (Math.Sign(moveValue) == 1 && !isFacingRight) isFacingRight = true;
        else if (Math.Sign(moveValue) == -1 && isFacingRight) isFacingRight = false;

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
            ChangeMovementType(MovementType.Jumping);
        }

        // Dash
        if (dashAction.WasPressedThisFrame())
        {
            if (dashCooldownRemained == 0) Dash();
        }

        // Wallking & Idle
        if (currentMovementType == MovementType.Walking && moveValue == 0)
        {
            ChangeMovementType(MovementType.Idle);
        }
        if (currentMovementType == MovementType.Idle && moveValue != 0)
        {
            ChangeMovementType(MovementType.Walking);
        }

        if (rb.linearVelocityY > 0 && !isGoingUp)
        {
            isGoingUp = true;
            animator.SetBool("isGoingUp", true);
        }
        else if (rb.linearVelocityY < 0 && isGoingUp && !isFallingDown)
        {
            isGoingUp = false;
            isFallingDown = true;
            animator.SetBool("isGoingUp", false);
            animator.SetBool("isFallingDown", true);
        }
        else if (rb.linearVelocityY == 0 && (isGoingUp || isFallingDown))
        {
            isGoingUp = false;
            isFallingDown = false;
            animator.SetBool("isGoingUp", false);
            animator.SetBool("isFallingDown", false);
            ChangeMovementType(MovementType.Walking);
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
            verticalValue = verticalAction.ReadValue<float>();

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

    void StartWalking()
    {
        animator.SetBool("isWalking", true);
    }
    void EndWalking()
    {
        animator.SetBool("isWalking", false);
    }
    void StartSprinting()
    {
        animator.SetBool("isSprinting", true);
        playerStamina.StartDraining();
    }
    void EndSprinting()
    {
        animator.SetBool("isSprinting", false);
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
    void StartJumping()
    {
        animator.SetTrigger("jump");
        Jump();
    }
    void EndJumping()
    {
        animator.SetTrigger("land");
    }
    void Jump()
    {
        rb.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    void Dash()
    {
        int dashDirection = isFacingRight ? 1 : -1;
        rb.AddForceX(dashForce * dashDirection, ForceMode2D.Impulse);
        dashCooldownRemained = dashCooldown;
    }

    void ChangeMovementType(MovementType newMovementType)
    {
        if (newMovementType != currentMovementType)
        {
            if (newMovementType == MovementType.Jumping && !GroundBelow()) return;

            // Ending Previous Movement
            switch (currentMovementType)
            {
                case MovementType.Walking:
                    EndWalking();
                    break;
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
                case MovementType.Jumping:
                    if (isFallingDown || isGoingUp) return;
                    EndJumping();
                    break;
            }
            
            // Starting New Movement
            switch (newMovementType)
            {
                case MovementType.Walking:
                    StartWalking();
                    break;
                case MovementType.Sprinting:
                    StartSprinting();
                    break;
                case MovementType.Crouching:
                    StartCrouching();
                    break;
                case MovementType.Climbing:
                    StartClimbing();
                    break;
                case MovementType.Jumping:
                    StartJumping();
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
