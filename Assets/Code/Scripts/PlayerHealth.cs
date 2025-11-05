using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float healthMaxValue = 100.0f;

    [Header("Heal Potion")]
    [SerializeField] private int healPotionAmount = 1;
    [SerializeField] private float healPotionValue = 50.0f;

    [Header("Health Bar Elements")]
    [SerializeField] private RectTransform healthBarRectTransform;
    [SerializeField] private RectTransform healthRectTransform;
    
    private float healthValue;
    private float healthWidth;
    private InputAction healAction;

    public float Health
    {
        get { return healthValue; }
    }

    void Awake()
    {
        healAction = InputSystem.actions.FindAction("heal");
        healthValue = healthMaxValue;
        healthWidth = healthBarRectTransform.sizeDelta.x;
        UpdateHealthBar();
    }

    void Update()
    {
        if (healAction.WasPressedThisFrame())
        {
            if (healPotionAmount > 0)
            {
                healPotionAmount -= 1;
                Heal(healPotionValue);
            }
        }
    }

    public void TakeDamage(float value)
    {
        healthValue -= value;
        if (healthValue <= 0)
        {
            healthValue = 0;
        }
        UpdateHealthBar();
    }

    void Heal(float value)
    {
        healthValue += value;
        if (healthValue > healthMaxValue)
        {
            healthValue = healthMaxValue;
        }
        UpdateHealthBar();
    }

    // Health Bar
    void UpdateHealthBar()
    {
        float newWidth = (healthValue / healthMaxValue) * healthWidth;
        healthRectTransform.sizeDelta = new Vector2(newWidth, healthRectTransform.sizeDelta.y);
    }
}
