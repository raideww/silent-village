using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    public float damage = 100.0f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Projectile")) Destroy(gameObject);
        
    }
}
