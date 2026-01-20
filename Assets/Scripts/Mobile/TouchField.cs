using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Invisible Touch Field for rotating the camera (Aiming)
/// </summary>
public class TouchField : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Vector2 TouchDist; // The amount we moved this frame
    [HideInInspector]
    public Vector2 PointerOld; // Where the finger was last frame
    [HideInInspector]
    protected int PointerId; // Which finger is touching meant for multi-touch
    [HideInInspector]
    public bool Pressed; // Is the user touching this area?

    void Update()
    {
        if (Pressed)
        {
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                // Fallback for mouse testing in Editor
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 1. Ignore if pointer is on the Left side of screen (Joystick Zone)
        // This prevents the camera from snapping when you try to move
        if (eventData.position.x < Screen.width / 2) return;

        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Logic handled in Update for smoother frames
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
