using UnityEngine;

public class MenuPlayerRotator : MonoBehaviour
{
    public float rotationSpeed = 5f;

    // This function runs automatically when you click and drag on this object
    void OnMouseDrag()
    {
        // Get how fast the mouse is moving left/right
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;

        // Rotate the player around the Y-Axis (Up arrow), inverted for natural feel
        // We use Space.World so he spins in place like a statue
        transform.Rotate(Vector3.up, -rotX, Space.World);
    }
}