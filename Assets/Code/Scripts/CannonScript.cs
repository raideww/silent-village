using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [Header("Ball")]
    public float ballSpeed = 5.0f;
    public float ballSpeedCharged = 20.0f;
    public float ballDamage = 100.0f;
    public float ballDamageCharged = 200.0f;

    [Header("Elements")]
    public Transform firePoint;
    public GameObject cannonBallPrefab;
    public Rigidbody2D playerRb;

    public void Attack(float chargePower = 0)
    {
        Vector3 spawnPoint = firePoint.transform.position + firePoint.rotation * Vector3.up;
        GameObject ball = Instantiate(cannonBallPrefab, spawnPoint, firePoint.rotation);

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        CannonBallScript cannonBallScript = ball.GetComponent<CannonBallScript>();

        float ballSpeedValue = (ballSpeedCharged - ballSpeed) * chargePower + ballSpeed;
        float ballDamageValue = ballDamage + (ballDamageCharged - ballDamage) * chargePower;

        ballRb.AddForce(firePoint.rotation * (Vector3.up * ballSpeedValue), ForceMode2D.Impulse);
        ballRb.linearVelocity = ballRb.linearVelocity + playerRb.linearVelocity;
        cannonBallScript.damage = ballDamageValue;
    }
}
