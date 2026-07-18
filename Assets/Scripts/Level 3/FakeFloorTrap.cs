using UnityEngine;

public class FakeFloorTrap : MonoBehaviour
{
    public GameObject floorToDisappear;
    public AudioClip fallSound;
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
            if (fallSound != null) audioSource.PlayOneShot(fallSound);
            
            if (floorToDisappear != null)
            {
                floorToDisappear.SetActive(false); // Zameen gayab!
            }
        }
    }
}
