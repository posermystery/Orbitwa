using UnityEngine;

public class Orbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float radius = 2f;
    public float rotationSpeed = 180f; // Degrees per second
    public bool isClockwise = true;

    // To visualize the orbit in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
