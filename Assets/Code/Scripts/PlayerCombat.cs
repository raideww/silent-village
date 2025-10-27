using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputAction attackAction;
    private Animator animator;

    // Attack
    private float holdingTime;
    private float chargedHoldingTime = .2f;
    private bool isHolding = false;
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
            if (!isHolding)
            {
                StartAttack();
            }
        }
        if (attackAction.WasReleasedThisFrame())
        {
            if (isHolding)
            {
                EndAttack();
            }
        }
    }

    void StartAttack()
    {
        isHolding = true;
        ResetAnimTriggers();
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
            animator.ResetTrigger("chargedAttackHold");
        }
        isHolding = false;
        isCharged = false;
    }

    void HoldAttack()
    {
        holdingTime += Time.deltaTime;
        if (holdingTime >= chargedHoldingTime && !isCharged)
        {
            isCharged = true;
            animator.SetTrigger("chargedAttackHold");
        }
    }
    
    void ResetAnimTriggers()
    {
        animator.ResetTrigger("attack");
        animator.ResetTrigger("chargedAttack");
        animator.ResetTrigger("chargedAttackHold");
    }
}
