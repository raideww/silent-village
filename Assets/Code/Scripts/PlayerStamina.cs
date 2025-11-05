using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] private float staminaMaxValue = 5.0f;
    [SerializeField] private float staminaRegenPerSecond = 1.0f;
    [SerializeField] private float staminaDrainPerSecond = 1.0f;

    [Header("Stamina Bar Elements")]
    [SerializeField] private RectTransform staminaBarRectTransform;
    [SerializeField] private RectTransform staminaRectTransform;
    
    private float staminaValue;
    private float staminaWidth;
    private bool isDraining = false;

    public float Stamina
    {
        get { return staminaValue; }
    }

    void Awake()
    {
        staminaValue = staminaMaxValue;
        staminaWidth = staminaBarRectTransform.sizeDelta.x;
        UpdateStaminaBar();
    }

    void FixedUpdate()
    {
        if (isDraining)
        {
            Drain();
        }
        else if (!isDraining && staminaValue < staminaMaxValue)
        {
            Regen();
        }
    }

    public void StartDraining()
    {
        isDraining = true;
    }
    public void EndDraining()
    {
        isDraining = false;
    }
    
    void Drain()
    {
        staminaValue -= staminaDrainPerSecond * Time.deltaTime;
        if (staminaValue < 0)
        {
            staminaValue = 0;
            EndDraining();
        }
        UpdateStaminaBar();
    }

    void Regen()
    {
        staminaValue += staminaRegenPerSecond * Time.deltaTime;
        if (staminaValue > staminaMaxValue)
        {
            staminaValue = staminaMaxValue;
        }
        UpdateStaminaBar();
    }
    
    // Stamina Bar
    void UpdateStaminaBar()
    {
        float newWidth = (staminaValue / staminaMaxValue) * staminaWidth;
        staminaRectTransform.sizeDelta = new Vector2(newWidth, staminaRectTransform.sizeDelta.y);
    }
}
