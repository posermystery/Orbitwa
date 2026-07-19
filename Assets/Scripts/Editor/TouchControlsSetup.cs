using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchControlsSetup : EditorWindow
{
    [MenuItem("Orbitwa/Setup Touch Controls (Level 3)")]
    public static void SetupTouchControls()
    {
        // 1. Create Event System if not exists
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            System.Type inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
                eventSystem.AddComponent(inputModuleType);
            else
                eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 2. Find or Create Canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("TouchCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Always on top

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 3. Create LEFT button (Bottom-Left)
        GameObject leftBtn = CreateTouchButton(canvas.transform, "LeftButton", "◀", 
            new Vector2(0, 0), new Vector2(0, 0), // anchors bottom-left
            new Vector2(120, 50), new Vector2(120, 120),
            TouchButton.ButtonType.Left);

        // 4. Create RIGHT button (Next to Left)
        GameObject rightBtn = CreateTouchButton(canvas.transform, "RightButton", "▶",
            new Vector2(0, 0), new Vector2(0, 0), // anchors bottom-left
            new Vector2(270, 50), new Vector2(120, 120),
            TouchButton.ButtonType.Right);

        // 5. Create JUMP button (Bottom-Right)
        GameObject jumpBtn = CreateTouchButton(canvas.transform, "JumpButton", "▲",
            new Vector2(1, 0), new Vector2(1, 0), // anchors bottom-right
            new Vector2(-120, 50), new Vector2(140, 140),
            TouchButton.ButtonType.Jump);

        Debug.Log("<color=green><b>Success:</b> Touch Controls (Left, Right, Jump) created! They work on both PC (click) and Android (tap).</color>");
    }

    private static GameObject CreateTouchButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size,
        TouchButton.ButtonType type)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);

        // RectTransform
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = size;

        // Semi-transparent background
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.35f); // White, 35% opacity

        // Button component (just for visual press feedback)
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        // Change pressed color to be more visible
        ColorBlock colors = btn.colors;
        colors.pressedColor = new Color(1f, 1f, 1f, 0.7f);
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.45f);
        btn.colors = colors;

        // TouchButton script
        TouchButton touchBtn = btnObj.AddComponent<TouchButton>();
        touchBtn.buttonType = type;

        // Label Text
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(btnObj.transform, false);

        Text text = textObj.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 60;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        return btnObj;
    }
}
