using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f; 

    void Update()
    {
        // Spin the box
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. Find the gun script on the Player's ACTIVE gun
            AdvancedGunSystem currentGun = other.GetComponentInChildren<AdvancedGunSystem>();

            if (currentGun != null)
            {
                // 2. Call the new "Refill" function
                currentGun.RefillAmmo();

                // 3. Delete the box
                Destroy(gameObject);
            }
        }
    }
}