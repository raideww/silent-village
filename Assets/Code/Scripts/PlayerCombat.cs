using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputAction attackAction;
    private Animator animator;

    // Attack
    private bool isHolding = false;
    private float holdingTime;
    private float chargedHoldingTime = 1.0f;
    private bool isCharged = false;
    

    void Awake()
    {
        animator = GetComponent<Animator>();
        attackAction = InputSystem.actions.FindAction("attack");
    }

    void Update()
    {
        if (isHolding)
        {
            HoldAttack();
        }

        if (attackAction.WasPressedThisFrame())
        {
            StartAttack();
        }
        if (attackAction.WasReleasedThisFrame())
        {
            EndAttack();
        }
    }

    void StartAttack()
    {
        isHolding = true;
    }

    void EndAttack()
    {
        if (!isCharged)
        {
            animator.SetTrigger("attack");
        }
        else
        {
            animator.SetTrigger("chargedAttack");
        }
        isHolding = false;
    }
    
    void HoldAttack()
    {
        holdingTime += Time.deltaTime;
        if (holdingTime >= chargedHoldingTime)
        {
            isCharged = true;
            animator.SetTrigger("chargedAttackHold");
        }
    }
}
