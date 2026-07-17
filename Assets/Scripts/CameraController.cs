using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    public float yOffset = 3f; // Positive value means camera is higher than the player, keeping player at the bottom

    [Header("Follow Settings")]
    public float smoothTime = 0.3f;
    
    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player != null)
        {
            // We only want to track the Y position, X stays fixed (e.g., at 0).
            // Z stays the same as current camera Z (usually -10).
            Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + yOffset, transform.position.z);

            // Smoothly move the camera towards the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}
