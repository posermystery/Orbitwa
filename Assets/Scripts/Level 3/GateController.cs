using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour
{
    [Header("Gate Settings")]
    public Transform gateWall; // Assign the wall that acts as the gate
    public float openDistance = 5f; // How high the gate should lift
    public float openSpeed = 2f; // How fast it opens
    
    [Header("Audio")]
    public AudioClip gateOpenSound;
    private AudioSource audioSource;
    private bool isGateOpen = false;

    [Header("Button Press Visuals")]
    public float pressDownDistance = 0.2f; // Kitna niche jayega button
    private SpriteRenderer originalSr;
    private Transform visualTransform;
    private int playersOnButton = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Visual vs Physics Separation Trick
        // We will create a child object to hold the visual sprite, so we can move it down
        // without moving the physical BoxCollider2D! This 100% prevents all physics flickering.
        originalSr = GetComponent<SpriteRenderer>();
        if (originalSr != null)
        {
            GameObject visualObj = new GameObject("ButtonVisual");
            visualTransform = visualObj.transform;
            visualTransform.SetParent(transform);
            visualTransform.localPosition = Vector3.zero;
            visualTransform.localRotation = Quaternion.identity;
            visualTransform.localScale = Vector3.one;

            SpriteRenderer newSr = visualObj.AddComponent<SpriteRenderer>();
            newSr.sprite = originalSr.sprite;
            newSr.color = originalSr.color;
            newSr.sortingLayerID = originalSr.sortingLayerID;
            newSr.sortingOrder = originalSr.sortingOrder;

            // Hide the original sprite so only the moving child is seen
            originalSr.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersOnButton++;
            if (playersOnButton == 1)
            {
                // Only move the visual part down!
                if (visualTransform != null)
                {
                    visualTransform.localPosition = new Vector3(0, -pressDownDistance, 0);
                }
            }

            if (!isGateOpen)
            {
                isGateOpen = true;
                if (gateOpenSound != null) audioSource.PlayOneShot(gateOpenSound);
                
                if (gateWall != null)
                {
                    StartCoroutine(OpenGate());
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersOnButton--;
            if (playersOnButton <= 0)
            {
                playersOnButton = 0;
                // Move the visual part back up!
                if (visualTransform != null)
                {
                    visualTransform.localPosition = Vector3.zero;
                }
            }
        }
    }

    private IEnumerator OpenGate()
    {
        Vector3 targetPosition = gateWall.position + Vector3.up * openDistance;
        
        while (Vector3.Distance(gateWall.position, targetPosition) > 0.01f)
        {
            gateWall.position = Vector3.MoveTowards(gateWall.position, targetPosition, openSpeed * Time.deltaTime);
            yield return null;
        }
        
        gateWall.position = targetPosition;
    }
}
