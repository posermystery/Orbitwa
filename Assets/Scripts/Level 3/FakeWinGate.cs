using UnityEngine;
using System.Collections;

public class FakeWinGate : MonoBehaviour
{
    [Header("Fake Win Settings")]
    public AudioClip victoryFanfare; // Plays fake victory sound first
    public AudioClip recordScratch;  // Then abruptly cuts to record scratch
    public AudioClip laughTrack;     // And plays a laugh track or death sound

    private AudioSource audioSource;
    private bool isTriggered = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;
            InstantFakeWinSequence(collision.gameObject);
        }
    }

    private void InstantFakeWinSequence(GameObject player)
    {
        // Stop player from moving
        PlatformerPlayer pController = player.GetComponent<PlatformerPlayer>();
        if (pController != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }

        // Ruin their dreams instantly
        if (recordScratch != null) audioSource.PlayOneShot(recordScratch);
        if (laughTrack != null) audioSource.PlayOneShot(laughTrack);

        // Kill them instantly with a polished taunt
        if (pController != null)
        {
            pController.Die("Did you REALLY think it would be that easy?! 🤡 This is a troll game, you absolute clown.");
        }
    }
}
