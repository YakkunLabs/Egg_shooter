using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A Simple Virtual Joystick for Mobile Movement
/// </summary>
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("UI Reference")]
    public Image bgImage;  // The background circle
    public Image joystickImg; // The inner handle circle

    public Vector3 InputDirection { get; private set; }

    private void Start()
    {
        // Auto-get images if not assigned but on same object
        if (bgImage == null) bgImage = GetComponent<Image>();
        if (joystickImg == null && transform.childCount > 0) 
            joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData ped)
    {
        Vector2 pos;
        
        // Calculate the position of the touch relative to the background image
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bgImage.rectTransform, 
            ped.position, 
            ped.pressEventCamera, 
            out pos))
        {
            pos.x = (pos.x / bgImage.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImage.rectTransform.sizeDelta.y);

            // Normalize
            InputDirection = new Vector3(pos.x * 2 - 1, 0, pos.y * 2 - 1);
            InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

            // Move the visual handle
            if (joystickImg != null)
            {
                joystickImg.rectTransform.anchoredPosition = new Vector3(
                    InputDirection.x * (bgImage.rectTransform.sizeDelta.x / 3),
                    InputDirection.z * (bgImage.rectTransform.sizeDelta.y / 3));
            }
        }
    }

    public void OnPointerDown(PointerEventData ped)
    {
        OnDrag(ped);
    }

    public void OnPointerUp(PointerEventData ped)
    {
        // Reset joystick on release
        InputDirection = Vector3.zero;
        if (joystickImg != null)
            joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }
}
