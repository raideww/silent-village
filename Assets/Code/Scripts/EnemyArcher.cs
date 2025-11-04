using UnityEngine;

public class Archer : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowSpeed = 10f;
    public float fireRate = 1f;

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

        if (distanceToPlayer <= detectionRange && Time.time >= nextFireTime)
        {
            isFacingRight = (player.position.x > transform.position.x);
            ShootArrow();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f; // ensures arrow arcs down

            // Calculate direction to player
            Vector2 direction = (player.position - firePoint.position).normalized;

            // Add upward curve (arc)
            Vector2 launchDir = new Vector2(direction.x, direction.y + 0.5f).normalized;

            rb.linearVelocity = launchDir * arrowSpeed;

            // Flip arrow if facing left
            if (!isFacingRight)
            {
                Vector3 scale = arrow.transform.localScale;
                scale.x *= -1;
                arrow.transform.localScale = scale;
            }

            // Rotate arrow toward its velocity direction (so it "flies" naturally)
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
