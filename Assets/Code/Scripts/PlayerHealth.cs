using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public Image hpBar; // optional UI image assigned in inspector

    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    // This method will be called by EnemyArcher via SendMessage or directly if you use GetComponent
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"PlayerHealth: Took {damage} damage. Current HP = {currentHP}");
        if (currentHP <= 0)
        {
            Die();
        }
        UpdateHPBar();
    }

    void Die()
    {
        Debug.Log("PlayerHealth: Player died.");
        // TODO: add death handling (disable input, play animation, respawn, etc.)
        // For now, just destroy the GameObject
        // Destroy(gameObject);
    }

    void UpdateHPBar()
    {
        if (hpBar != null && maxHP > 0)
        {
            hpBar.fillAmount = (float)currentHP / maxHP;
        }
    }
}
