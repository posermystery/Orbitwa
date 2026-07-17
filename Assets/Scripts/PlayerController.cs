using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpSpeed = 15f;

    [Header("State")]
    public bool isOrbiting = false;
    public Orbit currentOrbit;

    private float currentAngle = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // If we start attached to an orbit
        if (currentOrbit != null)
        {
            AttachToOrbit(currentOrbit);
        }
    }

    void Update()
    {
        if (isOrbiting && currentOrbit != null)
        {
            // Calculate new angle based on orbit speed and direction
            float directionMult = currentOrbit.isClockwise ? -1f : 1f;
            currentAngle += currentOrbit.rotationSpeed * directionMult * Time.deltaTime;
            
            // Keep angle between 0 and 360 to prevent large float values over time
            currentAngle %= 360f;

            // Calculate new position using trigonometry
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 newPos = new Vector2(
                Mathf.Cos(rad) * currentOrbit.radius,
                Mathf.Sin(rad) * currentOrbit.radius
            );

            // Move the player to the orbit position
            transform.position = (Vector2)currentOrbit.transform.position + newPos;

            // Check for tap/click to jump
            if (Input.GetMouseButtonDown(0))
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        isOrbiting = false;
        
        // Calculate tangent direction
        // If clockwise, tangent is -90 degrees from current angle. If counter-clockwise, it's +90 degrees.
        float tangentOffset = currentOrbit.isClockwise ? -90f : 90f;
        float tangentAngleRad = (currentAngle + tangentOffset) * Mathf.Deg2Rad;

        Vector2 jumpDirection = new Vector2(
            Mathf.Cos(tangentAngleRad),
            Mathf.Sin(tangentAngleRad)
        );

        // Apply velocity to the Rigidbody
        rb.linearVelocity = jumpDirection.normalized * jumpSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOrbiting && collision.CompareTag("Orbit"))
        {
            Orbit orbit = collision.GetComponent<Orbit>();
            if (orbit != null)
            {
                AttachToOrbit(orbit);
            }
        }
    }

    private void AttachToOrbit(Orbit orbit)
    {
        currentOrbit = orbit;
        isOrbiting = true;
        
        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Calculate the angle we hit the orbit at so we attach smoothly without snapping
        Vector2 directionFromCenter = transform.position - orbit.transform.position;
        currentAngle = Mathf.Atan2(directionFromCenter.y, directionFromCenter.x) * Mathf.Rad2Deg;
    }
}
