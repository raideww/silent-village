using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NianBossShieldController : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldObject; // Your 2D sprite circle
    public float shieldDownTime = 10f;

    [Header("Orb Spawn Settings")]
    public GameObject orbPrefab;
    public float detectionRange = 30f;
    public int numberOfOrbs = 4;

    [Header("Spawn Area Range")]
    public float minXSpawn = -8f;
    public float maxXSpawn = -2f;
    public float ySpawnHeight = 2f;

    [Header("Boss Physics")]
    public bool usePhysicsMovement = false;

    private bool isShieldActive = true;
    private bool playerInRange = false;
    private List<GameObject> currentOrbs = new List<GameObject>();
    private int orbsDestroyed = 0;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        // Setup boss physics for future movement
        if (rb != null)
        {
            if (usePhysicsMovement)
            {
                // For physics-based movement
                rb.isKinematic = false;
                rb.gravityScale = 0f; // No gravity for boss
                rb.linearDamping = 3f; // Some drag to prevent sliding
            }
            else
            {
                // For transform-based movement (recommended for bosses)
                rb.isKinematic = true;
            }
        }

        SetupShield();
        SetShield(true);
    }

    void SetupShield()
    {
        if (shieldObject != null)
        {
            // Reset shield position and scale
            shieldObject.transform.localPosition = Vector3.zero;
            shieldObject.transform.localScale = Vector3.one;

            // Shield should only have a collider, NO Rigidbody2D
            CircleCollider2D shieldCollider = shieldObject.GetComponent<CircleCollider2D>();
            if (shieldCollider == null)
            {
                shieldCollider = shieldObject.AddComponent<CircleCollider2D>();
                shieldCollider.isTrigger = true;
            }

            // Remove any Rigidbody2D that might be on the shield
            Rigidbody2D shieldRb = shieldObject.GetComponent<Rigidbody2D>();
            if (shieldRb != null)
            {
                Debug.LogWarning("Removing Rigidbody2D from shield - it should be on the boss parent!");
                Destroy(shieldRb);
            }
        }
    }

    void Update()
    {
        CheckPlayerDistance();

        // Future movement can be added here
        // if (isShieldActive) MoveBoss();
    }

    void CheckPlayerDistance()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && !playerInRange && isShieldActive)
        {
            playerInRange = true;
            SpawnOrbs();
        }
        else if (distanceToPlayer > detectionRange && playerInRange)
        {
            playerInRange = false;
            CleanupOrbs();
        }
    }

    void SpawnOrbs()
    {
        orbsDestroyed = 0;
        currentOrbs.Clear();

        for (int i = 0; i < numberOfOrbs; i++)
        {
            Vector2 spawnPos = CalculateOrbSpawnPosition(i);

            GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            NianShieldOrb orbScript = orb.GetComponent<NianShieldOrb>();

            if (orbScript != null)
            {
                orbScript.Initialize(this, i);
            }

            currentOrbs.Add(orb);
        }

        Debug.Log($"{numberOfOrbs} shield orbs spawned!");
    }

    Vector2 CalculateOrbSpawnPosition(int orbIndex)
    {
        float xSpawn = Mathf.Lerp(minXSpawn, maxXSpawn, (float)orbIndex / (numberOfOrbs - 1));

        float randomOffset = Random.Range(-0.5f, 0.5f);
        xSpawn += randomOffset;

        xSpawn = Mathf.Clamp(xSpawn, minXSpawn, maxXSpawn);

        Vector2 spawnPos = new Vector2(
            transform.position.x + xSpawn,
            transform.position.y + ySpawnHeight
        );

        return spawnPos;
    }

    public void OrbDestroyed(int orbID)
    {
        orbsDestroyed++;
        Debug.Log($"Orb {orbID} destroyed! {orbsDestroyed}/{numberOfOrbs}");

        if (orbsDestroyed >= numberOfOrbs)
        {
            BreakShield();
        }
    }

    void BreakShield()
    {
        isShieldActive = false;
        SetShield(false);
        CleanupOrbs();

        Debug.Log("NIAN SHIELD BROKEN! Vulnerable for " + shieldDownTime + " seconds!");
        StartCoroutine(ReactivateShield());
    }

    IEnumerator ReactivateShield()
    {
        yield return new WaitForSeconds(shieldDownTime);

        isShieldActive = true;
        SetShield(true);
        Debug.Log("Nian shield reactivated!");

        if (playerInRange)
        {
            SpawnOrbs();
        }
    }

    void CleanupOrbs()
    {
        foreach (GameObject orb in currentOrbs)
        {
            if (orb != null)
                Destroy(orb);
        }
        currentOrbs.Clear();
    }

    void SetShield(bool active)
    {
        if (shieldObject != null)
        {
            shieldObject.SetActive(active);
        }
    }

    public bool IsShieldActive
    {
        get { return isShieldActive; }
    }

    // Example method for future boss movement
    void MoveBoss()
    {
        // You can add your boss movement logic here later
        // Example: transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
    }
}