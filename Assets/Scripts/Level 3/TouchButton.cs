using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to a UI Button. While the button is held down, it sets a float/bool value.
/// PlatformerPlayer reads these static values for movement.
/// </summary>
public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Left, Right, Jump }
    public ButtonType buttonType;

    // Static values that PlatformerPlayer will read
    public static float touchHorizontal = 0f;
    public static bool touchJumpPressed = false;

    private bool isHeld = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        isHeld = true;

        if (buttonType == ButtonType.Jump)
        {
            touchJumpPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;

        if (buttonType == ButtonType.Left && touchHorizontal < 0f) touchHorizontal = 0f;
        if (buttonType == ButtonType.Right && touchHorizontal > 0f) touchHorizontal = 0f;
    }

    void Update()
    {
        if (isHeld)
        {
            if (buttonType == ButtonType.Left) touchHorizontal = -1f;
            if (buttonType == ButtonType.Right) touchHorizontal = 1f;
        }
    }

    void LateUpdate()
    {
        // Reset jump flag after one frame so it acts like "wasPressedThisFrame"
        if (buttonType == ButtonType.Jump)
        {
            touchJumpPressed = false;
        }
    }
}
