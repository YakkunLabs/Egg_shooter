using UnityEngine;

public class BulletVisual : MonoBehaviour
{
    public float speed = 50f;
    public float lifeTime = 2f;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Auto-delete after 2 seconds
    }

    void Update()
    {
        // Move forward locally
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}