using UnityEngine;


public class SwordScript : MonoBehaviour
{
    [Header("Sword Damage")]
    public float swordDamage = 100.0f;
    public float swordDamageCharged = 200.0f;

    void OnCollisionEnter2D(Collision2D collision)
    {

    }
}
