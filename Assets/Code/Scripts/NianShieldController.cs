using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossShieldController2D : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldObject; // Your circle sprite
    public float shieldDownTime = 10f;

    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public float detectionRange = 30f;
    public int numberOfOrbs = 4;

    [Header("Spawn Settings")]
    public float spawnDistance = 8f;

    private bool isShieldActive = true;
    private bool playerInRange = false;
    private List<GameObject> currentOrbs = new List<GameObject>();
    private int orbsDestroyed = 0;
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetupShield();
        SetShield(true);
    }

    void SetupShield()
    {
        if (shieldObject != null)
        {
            // Reset shield position and scale
            shieldObject.transform.localPosition = Vector3.zero;
            shieldObject.transform.localScale = Vector3.one; // Important for 2D!

            // Make sure the shield has proper 2D components
            CircleCollider2D shieldCollider = shieldObject.GetComponent<CircleCollider2D>();
            if (shieldCollider == null)
            {
                shieldCollider = shieldObject.AddComponent<CircleCollider2D>();
                shieldCollider.isTrigger = true; // So player can pass through
            }
        }
    }

    void Update()
    {
        CheckPlayerDistance();
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
            ShieldOrb2D orbScript = orb.GetComponent<ShieldOrb2D>();

            if (orbScript != null)
            {
                orbScript.Initialize(this, i);
            }

            currentOrbs.Add(orb);
        }

        Debug.Log($"{numberOfOrbs} shield orbs spawned! Destroy them!");
    }

    Vector2 CalculateOrbSpawnPosition(int orbIndex)
    {
        // Calculate positions in a semicircle on the LEFT side (2D)
        float angle = (orbIndex * 90f) + 135f; // Left side angles
        Vector2 spawnDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        Vector2 spawnPos = (Vector2)transform.position + (spawnDir * spawnDistance);

        // Add some random variation
        spawnPos += Random.insideUnitCircle * 2f;

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

        Debug.Log("SHIELD BROKEN! Boss vulnerable for " + shieldDownTime + " seconds!");
        StartCoroutine(ReactivateShield());
    }

    IEnumerator ReactivateShield()
    {
        yield return new WaitForSeconds(shieldDownTime);

        isShieldActive = true;
        SetShield(true);
        Debug.Log("Shield reactivated!");

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

    // 2D Gizmos
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Orb spawn positions
        Gizmos.color = Color.red;
        for (int i = 0; i < numberOfOrbs; i++)
        {
            Vector2 spawnPos = CalculateOrbSpawnPosition(i);
            Gizmos.DrawWireSphere(spawnPos, 0.5f);
        }
    }
}