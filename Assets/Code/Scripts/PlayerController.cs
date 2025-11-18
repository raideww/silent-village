
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;


    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (Math.Sign(playerMovement.MoveValue) == 1 && !isFacingRight)
        {
            isFacingRight = true;
            spriteRenderer.flipX = !isFacingRight;
        }
        else if (Math.Sign(playerMovement.MoveValue) == -1 && isFacingRight)
        {
            isFacingRight = false;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // {
            // Debug.Log(collision.gameObject.name);
            // Destroy(collision.gameObject);
        // }

    }
}