using UnityEngine;

public class EnemyTestSpawner : MonoBehaviour
{
    [Header("Test Settings")]
    public GameObject networkPlayerPrefab; // Drag your "Enemy" prefab here
    public int numberOfEnemies = 5;       // How many to spawn
    public float spacing = 2.0f;          // Distance between them

    void Start()
    {
        if (networkPlayerPrefab == null)
        {
            Debug.LogError("Forgot to assign the Prefab!");
            return;
        }

        SpawnTestEnemies();
    }

    void SpawnTestEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 1. Calculate Position (Line them up)
            Vector3 spawnPos = new Vector3(i * spacing, 0, 0);
            
            // 2. Spawn the "Puppet"
            GameObject newEnemy = Instantiate(networkPlayerPrefab, spawnPos, Quaternion.identity);
            newEnemy.name = "Test_Enemy_" + i;

            // 3. Get the Setup Script
            NetworkPlayerSetup setupScript = newEnemy.GetComponent<NetworkPlayerSetup>();

            if (setupScript != null)
            {
                // 4. PICK RANDOM LOOKS
                // We pick a random skin ID and random gun ID based on what is available in the script
                int randomSkin = Random.Range(0, setupScript.availableSkins.Length);
                int randomGun = Random.Range(0, setupScript.availableGuns.Length);

                // 5. COMMAND THE PUPPET
                setupScript.UpdateVisuals(randomSkin, randomGun, 0);

                // Add a text label above them (Optional, for debugging)
                Debug.Log($"Spawned Enemy {i}: Skin {randomSkin}, Gun {randomGun}");
            }
        }
    }
}