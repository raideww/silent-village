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
        // Safe tag checks: CompareTag throws if the tag doesn't exist in Project Settings.
        bool isPlayer = false;
        try
        {
            isPlayer = collision.CompareTag("Player");
        }
        catch (UnityException)
        {
            Debug.LogWarning("Tag 'Player' is not defined in Project Settings -> Tags and Layers. Please add it to avoid this warning.");
        }

        if (isPlayer)
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(10f); // damage value adjustable
            }

            // Optional: destroy or "stick" arrow
            Destroy(gameObject);
            return;
        }

        // Ground check: try tag first (safe), then fall back to a layer name check if tag missing
        bool isGround = false;
        try
        {
            isGround = collision.CompareTag("Ladder");
        }
        catch (UnityException)
        {
            // Tag missing; we'll try layer fallback below and warn the user.
            Debug.LogWarning("Tag 'Ground' is not defined in Project Settings -> Tags and Layers. Falling back to layer check.");
        }

        if (!isGround)
        {
            int groundLayer = LayerMask.NameToLayer("Ladder");
            if (groundLayer != -1)
            {
                isGround = (collision.gameObject.layer == groundLayer);
            }
        }

        if (isGround)
        {
            // Stick into ground (optional)
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic; // modern replacement for isKinematic = true
            }
            Destroy(gameObject, 2f);
        }
    }
}
