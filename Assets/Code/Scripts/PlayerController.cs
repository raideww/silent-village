
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = false;


    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (Math.Sign(playerMovement.MoveValue) == 1 && !isFacingRight)
        {
            spriteRenderer.flipX = isFacingRight = true;
        }
        else if (Math.Sign(playerMovement.MoveValue) == -1 && isFacingRight)
        {
            spriteRenderer.flipX = isFacingRight = false;
        }
    }

    public void TakeDamage(float value)
    {
        playerHealth.TakeDamage(value);
        if (playerHealth.Health == 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}