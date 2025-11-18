using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    public float damage = 100.0f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
        collision.gameObject.layer == LayerMask.NameToLayer("Ground")) Destroy(gameObject);
    }
}
