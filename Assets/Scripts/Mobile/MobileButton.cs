using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Helper component for mobile buttons that need PointerDown/PointerUp events
/// Attach this to buttons and it will call MobileInputManager methods
/// </summary>
public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType
    {
        Shoot,
        Jump,
        Reload,
        Scope
    }

    public ButtonType buttonType;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInputManager.Instance == null) return;

        switch (buttonType)
        {
            case ButtonType.Shoot:
                MobileInputManager.Instance.OnShootDown();
                break;
            case ButtonType.Jump:
                MobileInputManager.Instance.OnJumpDown();
                break;
            case ButtonType.Reload:
                MobileInputManager.Instance.OnReloadPress();
                break;
            case ButtonType.Scope:
                MobileInputManager.Instance.OnScopeToggle();
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (MobileInputManager.Instance == null) return;

        switch (buttonType)
        {
            case ButtonType.Shoot:
                MobileInputManager.Instance.OnShootUp();
                break;
            case ButtonType.Jump:
                MobileInputManager.Instance.OnJumpUp();
                break;
            // Reload and Scope don't need PointerUp
        }
    }
}
