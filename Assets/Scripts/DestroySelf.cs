using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public float lifetime = 2f; // Disappear after 2 seconds

    void Start()
    {
        // This command tells Unity to delete this object after 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}