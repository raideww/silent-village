using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public Transform firePoint;
    public float damage;
    public GameObject cannonBallPrefab;
    public float ballSpeed = 10.0f;

    public void Attack()
    {
        Vector3 spawnPoint = firePoint.transform.position + firePoint.rotation * Vector3.up;
        GameObject ball = Instantiate(cannonBallPrefab, spawnPoint, firePoint.rotation);
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        ballRb.linearVelocity = firePoint.rotation * (Vector3.up * ballSpeed);
    }
}
