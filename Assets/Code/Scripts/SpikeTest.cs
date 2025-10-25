using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public int damage = 20;           // How much damage spikes deal
    public float damageCooldown = 1f; // Time before spikes can hurt again
    private bool canDamage = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                StartCoroutine(DamageCooldown());
            }
        }
    }

    System.Collections.IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamage = true;
    }
}