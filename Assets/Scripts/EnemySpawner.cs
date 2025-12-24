using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] enemyPrefabs; // CHANGED: Now a list!
    public Transform[] spawnPoints; 
    
    public float timeBetweenSpawns = 3f; 
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSpawns)
        {
            SpawnEnemy();
            timer = 0; 
        }
    }

    void SpawnEnemy()
    {
        // 1. Pick a random spawn point
        int randomPoint = Random.Range(0, spawnPoints.Length);
        Transform mySpawnPoint = spawnPoints[randomPoint];

        // 2. Pick a random enemy type (Zombie or Sniper)
        int randomEnemy = Random.Range(0, enemyPrefabs.Length);
        GameObject chosenEnemy = enemyPrefabs[randomEnemy];

        // 3. Spawn
        Instantiate(chosenEnemy, mySpawnPoint.position, Quaternion.identity);
    }
}