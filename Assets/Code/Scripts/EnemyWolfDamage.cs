using UnityEngine;

public class WolfDamage : MonoBehaviour
{
    public float damageAmount = 20f;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.2f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (player != null && playerRb != null)
            {
                // Apply damage
                player.TakeDamage(damageAmount);

                // Apply knockback
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerRb.linearVelocity = Vector2.zero; // reset current velocity
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                // Optional: you can make player temporarily invincible or disable movement
                // for knockbackDuration seconds

                Debug.Log("Wolf attacked the player with knockback!");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (player != null && playerRb != null)
            {
                // Apply damage
                player.TakeDamage(damageAmount);

                // Apply knockback
                Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                Debug.Log("Wolf attacked the player with knockback!");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }
}
