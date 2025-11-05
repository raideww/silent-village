using NUnit.Framework;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    public Transform firePoint;
    public float damage;
    public GameObject cannonBallPrefab;
    public float ballSpeed = 5.0f;
    public float ballSpeedCharged = 20.0f;

    public void Attack(float chargePower = 0)
    {
        Vector3 spawnPoint = firePoint.transform.position + firePoint.rotation * Vector3.up;
        GameObject ball = Instantiate(cannonBallPrefab, spawnPoint, firePoint.rotation);
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        float ballSpeedValue = (ballSpeedCharged - ballSpeed) * chargePower + ballSpeed;
        ballRb.linearVelocity = firePoint.rotation * (Vector3.up * ballSpeedValue);
        Debug.Log(chargePower);
        Debug.Log(ballSpeedValue);
    }
}
