using UnityEngine;
using UnityEngine.UI;

public class EnemyWolfHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 50f;
    private float currentHealth;
    public bool isInvulnerable = false; // For testing

    [Header("UI")]
    public Image healthFill; // Reference to the UI Image fill component

    private void Awake()
    {
        Debug.Log($"{gameObject.name} Awake - Setting up wolf health");
        currentHealth = maxHealth;
    }

    private void Start()
    {
        Debug.Log($"{gameObject.name} Start - Health: {currentHealth}/{maxHealth}");
        UpdateHealthBar();
    }

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} Enabled - Health: {currentHealth}/{maxHealth}");
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        // Stack trace will show us where the damage is coming from
        Debug.Log($"{gameObject.name} TakeDamage called with {damage} damage from:\n{new System.Diagnostics.StackTrace()}");

        if (isInvulnerable)
        {
            Debug.Log($"{gameObject.name} is invulnerable - ignoring damage");
            return;
        }

        if (damage <= 0)
        {
            Debug.LogWarning($"{gameObject.name} received invalid damage amount: {damage}");
            return;
        }

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} health reduced from {currentHealth + damage} to {currentHealth}");

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthBar();
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died. Final health: {currentHealth}");
        // You can play animation, drop loot, etc.
        Destroy(gameObject);
    }
}