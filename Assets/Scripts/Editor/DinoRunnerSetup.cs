using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DinoRunnerSetup : EditorWindow
{
    [MenuItem("Orbitwa/Setup Dino Runner (Level 5)")]
    public static void Setup()
    {
        // 1. Ensure "Obstacle" tag exists
        EnsureTag("Obstacle");

        // 2. Create or find the white square sprite asset
        Sprite whiteSquare = GetOrCreateWhiteSprite();

        // 3. EventSystem (for touch/pointer input on mobile)
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            System.Type inputModule = System.Type.GetType(
                "UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModule != null)
                es.AddComponent(inputModule);
            else
                es.AddComponent<StandaloneInputModule>();
        }

        // 4. Configure camera explicitly
        if (Camera.main != null)
        {
            Camera.main.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
            Camera.main.orthographic = true;
            Camera.main.orthographicSize = 5f;
            Camera.main.transform.position = new Vector3(0f, 0f, -10f);
        }

        // 5. Ground — long dark rectangle at the bottom
        GameObject ground = new GameObject("Ground");
        SpriteRenderer groundSR = ground.AddComponent<SpriteRenderer>();
        groundSR.sprite = whiteSquare;
        groundSR.color = new Color(0.25f, 0.25f, 0.3f);
        groundSR.sortingOrder = 0;
        ground.transform.position = new Vector3(0f, -4.5f, 0f);
        ground.transform.localScale = new Vector3(30f, 1f, 1f);
        ground.AddComponent<BoxCollider2D>();

        // 6. Ground top line (thin decorative line for visual clarity)
        GameObject groundLine = new GameObject("GroundLine");
        SpriteRenderer lineSR = groundLine.AddComponent<SpriteRenderer>();
        lineSR.sprite = whiteSquare;
        lineSR.color = new Color(0.45f, 0.45f, 0.5f);
        lineSR.sortingOrder = 1;
        groundLine.transform.position = new Vector3(0f, -3.97f, 0f);
        groundLine.transform.localScale = new Vector3(30f, 0.06f, 1f);

        // 7. Player — cyan square sitting on the ground
        GameObject player = new GameObject("DinoPlayer");
        SpriteRenderer playerSR = player.AddComponent<SpriteRenderer>();
        playerSR.sprite = whiteSquare;
        playerSR.color = Color.cyan;
        playerSR.sortingOrder = 5;
        // X=-3 keeps player clearly in view. Ground surface Y=-4, player height=0.8 so bottom at -4.0
        player.transform.position = new Vector3(-3f, -3.6f, 0f);
        player.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        player.AddComponent<BoxCollider2D>();

        Rigidbody2D playerRb = player.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 3f;
        playerRb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        DinoRunner runner = player.AddComponent<DinoRunner>();

        // 8. Obstacle Spawner
        GameObject spawnerObj = new GameObject("ObstacleSpawner");
        ObstacleSpawner spawner = spawnerObj.AddComponent<ObstacleSpawner>();
        spawner.player = player.transform;

        // 9. Canvas + Score Text (top-right, like Chrome Dino)
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(canvas.transform, false);

        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.text = "00000";
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 48;
        scoreText.alignment = TextAnchor.UpperRight;
        scoreText.color = Color.white;

        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(1, 1);
        scoreRect.anchorMax = new Vector2(1, 1);
        scoreRect.pivot = new Vector2(1, 1);
        scoreRect.anchoredPosition = new Vector2(-30, -20);
        scoreRect.sizeDelta = new Vector2(300, 60);

        // Wire score text to the runner
        runner.scoreText = scoreText;

        // Select player so user can add audio clips easily
        Selection.activeGameObject = player;

        Debug.Log("<color=green><b>Success:</b> Dino Runner (Level 5) generated! Add your GameManager prefab to complete the scene.</color>");
    }

    // ─── Helper: Ensure a tag exists in the project ───
    private static void EnsureTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return; // Already exists
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();

        Debug.Log("<color=yellow>Tag '" + tag + "' created.</color>");
    }

    // ─── Helper: Create a simple white square sprite asset ───
    private static Sprite GetOrCreateWhiteSprite()
    {
        string dir = "Assets/Sprites";
        string path = dir + "/WhiteSquare.png";

        // Return existing if already created
        Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null) return existing;

        // Create the Sprites folder if needed
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets", "Sprites");

        // Generate a 4x4 white texture
        Texture2D tex = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();

        // Save as PNG
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        // Configure as Sprite
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 4;
        importer.filterMode = FilterMode.Point;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
