using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Speed")]
    public float startSpeed = 6f;
    public float maxSpeed = 14f;
    public float speedIncreaseRate = 0.15f;

    [Header("Spawn Timing")]
    public float initialDelay = 1.5f;
    public float minInterval = 0.6f;
    public float maxInterval = 2.0f;

    [Header("World")]
    public float spawnX = 12f;
    public float destroyX = -14f;
    public float groundSurfaceY = -4f;

    private float currentSpeed;
    private float nextSpawnTime;
    private List<GameObject> obstacles = new List<GameObject>();
    private Sprite whiteSprite;
    private int obstacleCount = 0;

    void Start()
    {
        currentSpeed = startSpeed;
        nextSpawnTime = Time.time + initialDelay;
        whiteSprite = CreateWhiteSprite();
    }

    void Update()
    {
        // Stop everything when player is dead/gone
        if (player == null || !player.gameObject.activeSelf) return;

        // Gradually increase speed
        currentSpeed = Mathf.Min(maxSpeed, currentSpeed + speedIncreaseRate * Time.deltaTime);

        // Spawn timer
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            float t = Mathf.Clamp01((currentSpeed - startSpeed) / (maxSpeed - startSpeed));
            float interval = Mathf.Lerp(maxInterval, minInterval, t);
            nextSpawnTime = Time.time + Random.Range(interval * 0.7f, interval * 1.3f);
        }

        // Move all obstacles left & cleanup
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            if (obstacles[i] == null)
            {
                obstacles.RemoveAt(i);
                continue;
            }

            obstacles[i].transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

            if (obstacles[i].transform.position.x < destroyX)
            {
                Destroy(obstacles[i]);
                obstacles.RemoveAt(i);
            }
        }
    }

    private void SpawnObstacle()
    {
        obstacleCount++;

        // After a while, start mixing in harder obstacles
        bool isAerial = obstacleCount > 8 && Random.value < 0.25f;
        bool isImpossible = obstacleCount > 12 && Random.value < 0.2f;

        GameObject obs = new GameObject("Obstacle");
        obs.tag = "Obstacle";

        SpriteRenderer sr = obs.AddComponent<SpriteRenderer>();
        sr.sprite = whiteSprite;
        sr.sortingOrder = 2;

        float width, height;
        Vector3 pos;

        if (isImpossible)
        {
            // WALL — impossible to jump over normally, forces the player to think differently
            width = 0.6f;
            height = 6f;
            pos = new Vector3(spawnX, groundSurfaceY + height / 2f, 0f);
            sr.color = new Color(0.85f, 0.1f, 0.1f); // Red wall
        }
        else if (isAerial)
        {
            // Flying obstacle at mid-height
            width = 1.5f;
            height = 0.5f;
            float flyY = Random.Range(-1.5f, -0.5f);
            pos = new Vector3(spawnX, flyY, 0f);
            sr.color = new Color(0.9f, 0.35f, 0.1f); // Orange bird
        }
        else
        {
            // Normal ground cactus
            width = Random.Range(0.4f, 0.8f);
            height = Random.Range(0.8f, 2.2f);
            pos = new Vector3(spawnX, groundSurfaceY + height / 2f, 0f);
            sr.color = new Color(0.2f, 0.65f, 0.2f); // Green cactus
        }

        obs.transform.position = pos;
        obs.transform.localScale = new Vector3(width, height, 1f);

        BoxCollider2D col = obs.AddComponent<BoxCollider2D>();

        Rigidbody2D rb = obs.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        obstacles.Add(obs);
    }

    private Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
    }
}
