using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestServerSimulator : MonoBehaviour
{
    private List<NetworkPuppet> allPuppets = new List<NetworkPuppet>();
    public float commandInterval = 2.0f; // Give new orders every 2 seconds

    void Start()
    {
        // Wait a split second for Spawner to finish, then find all puppets
        StartCoroutine(FindAndCommandRoutine());
    }

    IEnumerator FindAndCommandRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // Find everyone with the Puppet script
        NetworkPuppet[] found = FindObjectsOfType<NetworkPuppet>();
        allPuppets.AddRange(found);

        Debug.Log($"[Test Server] Found {allPuppets.Count} puppets. Starting simulation...");

        // Loop forever giving commands
        while (true)
        {
            foreach (NetworkPuppet puppet in allPuppets)
            {
                GiveRandomCommand(puppet);
            }

            yield return new WaitForSeconds(commandInterval);
        }
    }

    void GiveRandomCommand(NetworkPuppet puppet)
    {
        // 1. Move Command
        // Pick a random spot within 5 meters
        Vector3 randomOffset = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        Vector3 newPos = puppet.transform.position + randomOffset;
        
        // Look at the new spot
        Quaternion newRot = Quaternion.LookRotation(randomOffset);

        puppet.Server_MoveTo(newPos, newRot);

        // 2. Shoot Command (50% chance)
        if (Random.value > 0.5f)
        {
            puppet.Server_Shoot();
            Debug.Log(puppet.name + " is shooting!");
        }
    }
}