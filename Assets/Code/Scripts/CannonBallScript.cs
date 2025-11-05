using UnityEngine;

public class CannonBallScript : MonoBehaviour
{
    public float damage = 100.0f;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
