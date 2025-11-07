using UnityEngine;

public class NianShieldOrb : MonoBehaviour
{
    public NianBossShieldController shieldController;
    public int orbID;

    void Start()
    {
        // 2D RIGIDBODY - FIXED
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0f; // 2D version of useGravity = false
        }
    }

    public void Initialize(NianBossShieldController controller, int id)
    {
        shieldController = controller;
        orbID = id;
    }

    public void OnOrbDestroyed()
    {
        if (shieldController != null)
        {
            shieldController.OrbDestroyed(orbID);
        }
        Destroy(gameObject);
    }

    // 2D COLLISION - FIXED
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerWeapon") || other.CompareTag("Player"))
        {
            OnOrbDestroyed();
        }
    }
}