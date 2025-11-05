using UnityEngine;

public class ShieldOrb2D : MonoBehaviour
{
    public BossShieldController2D shieldController;
    public int orbID;

    void Start()
    {
        // 2D Physics setup
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true; // Important for 2D!
        }
    }

    public void Initialize(BossShieldController2D controller, int id)
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

    // 2D Collision
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerWeapon") || other.CompareTag("Player"))
        {
            OnOrbDestroyed();
        }
    }
}