using UnityEngine;
using TMPro;

public class ItemPickupVisual : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    [Header("UI")]
    public GameObject pickupText; // Drag your World Space Canvas text here
    public float showDistance = 3.0f;

    private Vector3 startPos;
    private Transform player;

    void Start()
    {
        startPos = transform.position;
        
        // Find player once (optimization)
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        
        if (pickupText != null) pickupText.SetActive(false);
    }

    void Update()
    {
        // ------------------------------------------------------------------
        // ✅ 1. ANIMATION FIX (Spin 360 in World Space)
        // ------------------------------------------------------------------
        // We use 'Space.World' so it spins around the global UP axis (Skyward),
        // regardless of how the weapon model is tilted/rotated locally.
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        
        // Bobbing Logic (Float Up/Down)
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        
        // Apply position but keep X and Z absolute (prevents drifting)
        transform.position = new Vector3(startPos.x, newY, startPos.z);


        // ------------------------------------------------------------------
        // 2. UI PROMPT (Show "Press F")
        // ------------------------------------------------------------------
        if (player == null)
        {
             // Try finding player again if we missed them at start
             GameObject p = GameObject.FindWithTag("Player");
             if (p != null) player = p.transform;
             return;
        }

        if (pickupText != null)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            bool isClose = (dist <= showDistance);

            if (pickupText.activeSelf != isClose)
                pickupText.SetActive(isClose);

            // Make text look at camera (Billboarding)
            if (isClose && Camera.main != null)
            {
                pickupText.transform.LookAt(Camera.main.transform);
                pickupText.transform.Rotate(0, 180, 0); // Flip to face correct way
            }
        }
    }
}