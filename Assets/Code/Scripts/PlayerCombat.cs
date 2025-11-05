using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

enum Weapon
{
    Sword,
    Cannon
}

public class PlayerCombat : MonoBehaviour
{
    private InputAction attackAction;
    private Animator animator;

    // Attack
    private float holdingTime;
    private float chargedHoldingTime = .2f;
    private bool isHolding = false;
    private bool isCharged = false;

    // Cannon
    private Weapon currentWeapon = Weapon.Cannon;
    public GameObject weaponCannon;


    void Awake()
    {
        animator = GetComponent<Animator>();
        attackAction = InputSystem.actions.FindAction("attack");
    }

    void Update()
    {
        if (currentWeapon == Weapon.Sword)
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
        else if (currentWeapon == Weapon.Cannon)
        {
            if (attackAction.WasPressedThisFrame())
            {
                CannonScript cannon = weaponCannon.GetComponent<CannonScript>();
                cannon.Attack();
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
