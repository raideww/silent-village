using UnityEngine;
using UnityEngine.UI;

public class EnemyArcher : MonoBehaviour
{
    public Transform[] patrolPoints; // Points to patrol between
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float detectionRange = 5f;
    public int maxHP = 100;
    public Image hpBar; // UI Image for displaying HP above enemy

    private int currentPatrolIndex = 0;
    private Transform player;
    private int currentHP;
    private bool isChasing = false;
    private Rigidbody2D rb;
    [Header("Contact Damage")]
    public int contactDamage = 10; // Damage applied when touching the player
    public float damageCooldown = 1.0f; // Seconds between repeated damage while touching
    private float lastDamageTime = -999f;

    void Start()
    {
        currentHP = maxHP;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("EnemyArcher: No GameObject with tag 'Player' found.");
        }
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Prevent spinning
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        UpdateHPBar();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Chase the player
            ChasePlayer();
        }
        else
        {
            // Patrol within limited area
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        else
        {
            MoveTowards(targetPoint.position, patrolSpeed);
        }
    }

    void ChasePlayer()
    {
        MoveTowards(player.position, chaseSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        // Only move horizontally to avoid vertical flying
        Vector3 delta = target - transform.position;
        float dirX = Mathf.Sign(delta.x);
        float moveX = dirX * speed;
        if (rb != null)
        {
            // Preserve current vertical velocity (gravity, jumps, etc.) and only set horizontal speed
            rb.linearVelocity = new Vector2(moveX, rb.linearVelocity.y);
        }
        // Optional: Add sprite flipping or animation here
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"EnemyArcher: Collided with Player ({collision.gameObject.name}). Attempting to apply damage.");
            TryDealContactDamage(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            // Take damage from player's attack
            Debug.Log($"EnemyArcher: Hit by Projectile ({collision.gameObject.name}). Taking damage.");
            TakeDamage(20);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Optional: add logic for enemy collision
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDealContactDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"EnemyArcher: TriggerEnter with Player ({other.gameObject.name}). Attempting to apply damage.");
            TryDealContactDamage(other.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryDealContactDamage(other.gameObject);
        }
    }

    private void TryDealContactDamage(GameObject playerObj)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;
        lastDamageTime = Time.time;
        Debug.Log($"EnemyArcher: Dealing {contactDamage} contact damage to {playerObj.name}.");
        playerObj.SendMessage("TakeDamage", contactDamage, SendMessageOptions.DontRequireReceiver);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
        UpdateHPBar();
    }

    void UpdateHPBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHP / maxHP;
        }
    }

}
