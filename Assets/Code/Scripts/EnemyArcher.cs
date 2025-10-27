using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;
    public float fireRate = 0.5f;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 7f;

    [Header("Archer Settings")]
    public bool isFacingRight = true;

    private float nextFireTime = 0f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Fire automatically if player is within range
        if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
        {
            // Flip archer based on player position
            isFacingRight = (player.position.x > transform.position.x);

            ShootArrow();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        // Flip arrow if facing left
        if (!isFacingRight)
        {
            Vector3 scale = arrow.transform.localScale;
            scale.x *= -1;
            arrow.transform.localScale = scale;
        }

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = (isFacingRight ? firePoint.right : -firePoint.right) * arrowSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
