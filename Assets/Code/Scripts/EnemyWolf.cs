using UnityEngine;

public class EnemyWolf : MonoBehaviour
{

    public float speed = 1.0f;
    public float chaseSpeed = 3.0f;
    public float detectionRange = 10.0f;
    public float range = 2.0f;
    public int damage = 10;
    public Transform player;
    private Rigidbody2D rb;
    private Vector2 startingPoint;
    private bool movingRight = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPoint = transform.position;

        // Optional: automatically find player by tag
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Chase player
            ChasePlayer();
        }
        else
        {
            // Patrol automatically
            Patrol();
        }
    }
    void Patrol()
    {
        float move = (movingRight ? 1 : -1) * speed;
        rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

        if (movingRight && transform.position.x > startingPoint.x + range)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && transform.position.x < startingPoint.x - range)
        {
            movingRight = true;
            Flip();
        }
    }
    void ChasePlayer()
    {
        if (player == null) return;

        // Move towards the player
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        // Flip to face player
        if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
        {
            movingRight = !movingRight;
            Flip();
        }
    }
    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private void OnDrawGizmosSelected()
    {
        // Visualize detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
