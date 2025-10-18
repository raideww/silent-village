using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction climbAction;
    private InputAction healAction;
    private InputAction goDownAction;


    private Vector2 moveValue;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isSprinting = false;
    private bool isCrouching = false;
    private bool tryingToUncrouch = false;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Renderer rend;
    

    public float moveSpeed = 5.0f;
    public float crouchSpeed = 2.5f;
    public Vector2 crouchScale = new Vector2(1.0f, 0.7f);
    public float jumpSpeed = 10.0f;

    // Crouch
    private Vector2 originalSpriteSize;
    private Vector2 originalBoxColliderSize;
    private Vector2 originalBoxColliderOffset;
    public Transform ceilingCheck;
    public Vector2 ceilingCheckSize = new Vector2(0.7f, 0.4f);

    // Climb
    private bool onLadder = false;
    private bool isClimbing = false;
    private float originalGravityScale;
    private bool isGoingDown = false;
    public float climbSpeed = 2.5f;

    // Sprint
    public float sprintSpeed = 10.0f;

    // Stamina
    public RectTransform staminaBarRectTransform;
    public RectTransform staminaRectTransform;
    public float staminaMaxValue = 5.0f;
    public float staminaRegenPerSecond = 1.0f;
    public float staminaDrainPerSecond = 1.0f;

    private float staminaValue;
    private float staminaWidth;

    // Health
    public RectTransform healthBarRectTransform;
    public RectTransform healthRectTransform;
    public float healthMaxValue = 100.0f;
    private float healthValue;
    private float healthWidth;

    // Heal Potion
    public int healPotionAmount = 1;
    public float healPotionValue = 50.0f;


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
        climbAction = InputSystem.actions.FindAction("climb");
        healAction = InputSystem.actions.FindAction("heal");
        goDownAction = InputSystem.actions.FindAction("go down");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rend = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        staminaValue = staminaMaxValue;
        staminaWidth = staminaBarRectTransform.sizeDelta.x;
        UpdateStaminaBar();
        healthValue = healthMaxValue;
        healthWidth = healthBarRectTransform.sizeDelta.x;
        UpdateHealthBar();
    }

    private void Update()
    {
        moveValue = moveAction.ReadValue<Vector2>();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        if (healAction.WasPressedThisFrame())
        {
            if (healPotionAmount > 0)
            {
                healPotionAmount -= 1;
                Heal(healPotionValue);
            }
        }

        if (onLadder && goDownAction.WasPerformedThisFrame())
        {
            GoDownInit();
            isGoingDown = true;
        }
        else if (climbAction.WasReleasedThisFrame())
        {
            GoDownInit(reverse: true);
            isGoingDown = false;
        }


        if (onLadder && climbAction.WasPressedThisFrame())
        {
            ClimbInit();
            isClimbing = true;
        }
        else if (climbAction.WasReleasedThisFrame())
        {
            ClimbInit(reverse: true);
            isClimbing = false;
        }

        if (crouchAction.WasPressedThisFrame())
        {
            if (!isCrouching)
            {
                Crouch();
            }
        }
        else if (crouchAction.WasReleasedThisFrame())
        {
            if (isCrouching)
            {
                Crouch(reverse: true);
            }
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

        if (isGoingDown && onLadder)
        {
            rb.linearVelocityY = -climbSpeed;
        }

        if (isClimbing && onLadder)
        {
            rb.linearVelocityY = climbSpeed;
        }

        if (tryingToUncrouch && !GroundAbove())
        {
            Crouch(reverse: true);
        }

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
        else if (isSprinting && staminaValue > 0)
        {
            rb.linearVelocityX = moveValue.x * sprintSpeed;
            staminaValue -= staminaDrainPerSecond * Time.deltaTime;
            if (staminaValue < 0)
            {
                staminaValue = 0;
            }
            UpdateStaminaBar();

        }
        else
        {
            rb.linearVelocityX = moveValue.x * moveSpeed;
        }
        
        if (!isSprinting && staminaValue < staminaMaxValue)
        {
            staminaValue += staminaRegenPerSecond * Time.deltaTime;
            if (staminaValue > staminaMaxValue)
            {
                staminaValue = staminaMaxValue;
            }
            UpdateStaminaBar();
        }
    }

    void Crouch(bool reverse = false)
    {
        if (!reverse)
        {
            // Getting previous scale to take into account when calculating height compensation
            // because when player's size changes it changes in center which leaves player off the ground
            // thats why we should compensate this change
            Vector2 beforeCrouchSize = rend.bounds.size;

            // Saving original values to go back to them after crouch
            originalSpriteSize = spriteRenderer.size;
            originalBoxColliderSize = boxCollider.size;
            originalBoxColliderOffset = boxCollider.offset;

            // Changing player status
            isCrouching = true;
            tryingToUncrouch = false;

            // To crouch the character we need to change these components
            // 1) sprite size to show that our character is crouched
            //    and in future may be replaced to another sprite
            // 2) box collider to match the rendered sprite size on the screen
            spriteRenderer.size = Vector2.Scale(spriteRenderer.size, crouchScale);
            boxCollider.size = Vector2.Scale(boxCollider.size, crouchScale);
            boxCollider.offset = Vector2.Scale(boxCollider.offset, crouchScale);

            // Calculating height compensation by getting the difference of player's height before changing size and after
            // and dividing it by 2 because we only need to account space under the player
            Vector2 afterCrouchSize = rend.bounds.size;
            float heightCompensation = (beforeCrouchSize.y - afterCrouchSize.y) / 2;
            Vector2 compensatedPosition = transform.localPosition - new Vector3(0, heightCompensation, 0);
            transform.localPosition = compensatedPosition;

            // Also crouching affects crucial gameobjects like: "Ground Check" and "Ceiling Check"
            // So we also have to compensate their position too
            groundCheck.transform.position += new Vector3(0f, heightCompensation);
            ceilingCheck.transform.position += new Vector3(0f, heightCompensation);
        }
        else
        {
            if (GroundAbove())
            {
                tryingToUncrouch = true;
                return;
            }
            Vector2 beforeCrouchSize = rend.bounds.size;

            spriteRenderer.size = originalSpriteSize;
            boxCollider.size = originalBoxColliderSize;
            boxCollider.offset = originalBoxColliderOffset;
            isCrouching = false;
            tryingToUncrouch = false;

            Vector2 afterCrouchSize = rend.bounds.size;
            float heightCompensation = (beforeCrouchSize.y - afterCrouchSize.y) / 2;
            Vector2 compensatedPosition = transform.localPosition - new Vector3(0, heightCompensation, 0);
            transform.localPosition = compensatedPosition;

            groundCheck.transform.position += new Vector3(0f, heightCompensation);
            ceilingCheck.transform.position += new Vector3(0f, heightCompensation);
        }

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
            ClimbInit(reverse: true);
        }
    }

    void ClimbInit(bool reverse = false)
    {
        if (!reverse)
        {
            if (!isClimbing)
            {
                originalGravityScale = rb.gravityScale;
                rb.gravityScale = 0;
            }
        }
        else
        {
            if (isClimbing)
            {
                rb.gravityScale = originalGravityScale;
            }
        }

    }

    void GoDownInit(bool reverse = true)
    {
        if (!reverse)
        {
            if (!isClimbing)
            {
                originalGravityScale = rb.gravityScale;
                rb.gravityScale = 0;
            }
        }
        else
        {
            if (isClimbing)
            {
                rb.gravityScale = originalGravityScale;
            }
        }
    }

    void UpdateStaminaBar()
    {
        float newWidth = (staminaValue / staminaMaxValue) * staminaWidth;
        staminaRectTransform.sizeDelta = new Vector2(newWidth, staminaRectTransform.sizeDelta.y);
    }

    void UpdateHealthBar()
    {
        float newWidth = (healthValue / healthMaxValue) * healthWidth;
        healthRectTransform.sizeDelta = new Vector2(newWidth, healthRectTransform.sizeDelta.y);
    }

    public void TakeDamage(float value)
    {
        healthValue -= value;
        if (healthValue <= 0)
        {
            healthValue = 0;
            Die();
        }
        UpdateHealthBar();
    }

    void Heal(float value)
    {
        healthValue += value;
        if (healthValue > healthMaxValue)
        {
            healthValue = healthMaxValue;
        }
        UpdateHealthBar();
    }
    
    void Die()
    {
        Debug.Log("Player died!");
    }

    void OnDrawGizmosSelected()
    {
        // Just to visualize the ground check in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireCube(ceilingCheck.position, ceilingCheckSize);
    }
}