using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerPlayer : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float jumpForce = 8f;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    [Header("Audio")]
    public AudioClip jumpSound;
    [Range(0f, 1f)] public float jumpVolume = 0.1f;
    
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isDead = false;
    private bool isGrounded = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        // Check if grounded (safely and ignoring self)
        isGrounded = false;
        int layerMask = groundLayer.value == 0 ? ~0 : groundLayer.value; // If Nothing is selected, use Everything

        if (groundCheck != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, layerMask);
            foreach (var col in colliders)
            {
                if (col.gameObject != gameObject && !col.isTrigger)
                {
                    isGrounded = true;
                    break;
                }
            }
        }
        else
        {
            // Agar player ne groundCheck transform assign nahi kiya hai, toh automatically player ke theek niche check karo
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 1.5f, layerMask);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject != gameObject && !hit.collider.isTrigger)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        float horizontalInput = 0f;
        bool jumpPressed = false;

        // --- PC CONTROLS ---
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) jumpPressed = true;
        }

        // --- MOBILE ON-SCREEN BUTTON CONTROLS ---
        if (TouchButton.touchHorizontal != 0f) horizontalInput = TouchButton.touchHorizontal;
        if (TouchButton.touchJumpPressed) jumpPressed = true;

        // Move Player
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Jump Player
        if (jumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (jumpSound != null) audioSource.PlayOneShot(jumpSound, jumpVolume);
        }
        
        // Flip Sprite
        if (horizontalInput != 0)
        {
            float originalScaleX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector3(originalScaleX * Mathf.Sign(horizontalInput), transform.localScale.y, transform.localScale.z);
        }

        // Fall out of world death
        if (transform.position.y < -10f)
        {
            Die("You fell into the abyss.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("DeathTrap"))
        {
            Die("Wow, you stepped on a spike. Very original.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("DeathTrap"))
        {
            Die("Wow, you stepped on a spike. Very original.");
        }
    }

    public void Die(string reason)
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true; // freeze
        gameObject.GetComponent<SpriteRenderer>().enabled = false; // Hide player

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver(reason);
        }
    }
}
