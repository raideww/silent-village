using NUnit.Framework;
using Unity.VisualScripting;
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
    private InputAction switchWeaponAction;
    private Animator animator;

    // Attack
    private float holdingTime;
    private float chargedHoldingTime = .2f;
    private bool isHolding = false;
    private bool isCharged = false;
    public float holdingTimeMax = 2.0f;

    // Cannon
    private Weapon currentWeapon = Weapon.Cannon;

    public GameObject weaponSword;
    public GameObject weaponCannon;
    private CannonScript cannonScript;


    void Awake()
    {
        animator = GetComponent<Animator>();
        attackAction = InputSystem.actions.FindAction("attack");
        switchWeaponAction = InputSystem.actions.FindAction("switch weapon");
        cannonScript = weaponCannon.GetComponent<CannonScript>();
    }

    void Update()
    {
        if (switchWeaponAction.WasPressedThisFrame())
        {
            SwitchWeapon();
        }

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

        if (currentWeapon == Weapon.Sword)
        {
            ResetAnimTriggers();
        }
    }

    void EndAttack()
    {
        if (!isCharged)
        {
            if (currentWeapon == Weapon.Sword)
            {
                animator.SetTrigger("attack");
            }
            else if (currentWeapon == Weapon.Cannon)
            {
                cannonScript.Attack();
            }
        }
        else
        {
            if (holdingTime > holdingTimeMax)
            {
                holdingTime = holdingTimeMax;
            }
            if (currentWeapon == Weapon.Sword)
            {
                animator.SetTrigger("chargedAttack");
                animator.ResetTrigger("chargedAttackHold");
            }
            else if (currentWeapon == Weapon.Cannon)
            {
                cannonScript.Attack(chargePower: holdingTime/holdingTimeMax);
            }
        }
        isHolding = false;
        isCharged = false;
        holdingTime = 0;
    }

    void HoldAttack()
    {
        holdingTime += Time.deltaTime;
        if (holdingTime >= chargedHoldingTime && !isCharged)
        {
            isCharged = true;
            if (currentWeapon == Weapon.Sword)
            {
                animator.SetTrigger("chargedAttackHold");
            }
        }
    }

    void ResetAnimTriggers()
    {
        animator.ResetTrigger("attack");
        animator.ResetTrigger("chargedAttack");
        animator.ResetTrigger("chargedAttackHold");
    }

    void SwitchWeapon()
    {
        if (isHolding)
        {
            EndAttack();
        }

        if (currentWeapon == Weapon.Sword)
        {
            // weaponSword.SetActive(false);
            weaponCannon.SetActive(true);
            currentWeapon = Weapon.Cannon;
        }
        else if (currentWeapon == Weapon.Cannon)
        {
            // weaponSword.SetActive(true);
            weaponCannon.SetActive(false);
            currentWeapon = Weapon.Sword;
        }
    }
}
