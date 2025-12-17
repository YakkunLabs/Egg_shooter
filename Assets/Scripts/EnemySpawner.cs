using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject enemyPrefab; // The enemy blueprint
    public Transform[] spawnPoints; // List of places where they can appear
    
    public float timeBetweenSpawns = 3f; // Spawn every 3 seconds
    private float timer;

    void Update()
    {
        // Count down
        timer += Time.deltaTime;

        // If time is up...
        if (timer >= timeBetweenSpawns)
        {
            SpawnEnemy();
            timer = 0; // Reset timer
        }
    }

    void SpawnEnemy()
    {
        // 1. Pick a random number between 0 and the number of spawn points
        int randomIndex = Random.Range(0, spawnPoints.Length);
        
        // 2. Get that specific spawn point
        Transform mySpawnPoint = spawnPoints[randomIndex];

        // 3. Create the enemy at that position
        Instantiate(enemyPrefab, mySpawnPoint.position, Quaternion.identity);
    }
}