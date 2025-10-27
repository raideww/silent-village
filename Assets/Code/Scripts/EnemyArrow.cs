using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 10f;           // damage dealt to player
    public float lifetime = 5f;          // destroy arrow after this time

    private void Start()
    {
        // Destroy the arrow automatically after lifetime seconds
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if arrow hits player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject); // Destroy arrow on hit
        }
        // Optional: destroy arrow if it hits walls or ground
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
