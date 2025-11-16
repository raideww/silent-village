using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform startPoint;
    
    private Vector2 respawnPoint;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (!startPoint) respawnPoint = new Vector2(0, 0);
        else respawnPoint = startPoint.position;

        transform.position = respawnPoint;

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth.Health == 0)
        {
            Die();
        }
    }

    void Die()
    {
        transform.position = respawnPoint;
        playerHealth.ResetHealth();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("RespawnPoint"))
        {
            respawnPoint = collision.transform.position;
        }
    }
}
