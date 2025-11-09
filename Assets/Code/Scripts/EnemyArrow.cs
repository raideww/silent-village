using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    public float lifetime = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Only rotate if moving (so it doesn't spin when stuck)
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(10f); // damage value adjustable
            }

            // Optional: destroy or "stick" arrow
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            // Stick into ground (optional)
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // ✅ modern replacement for isKinematic = true
            Destroy(gameObject, 2f);
        }
    }
}
