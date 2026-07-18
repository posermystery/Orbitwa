using UnityEngine;
using System.Collections;

public class ButtonSpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public Transform spikeObject;
    public float popUpSpeed = 15f;
    public float popUpDistance = 1.5f;
    
    [Header("Audio")]
    public AudioClip trapSound;
    private AudioSource audioSource;

    private bool isTriggered = false;
    private Vector3 targetPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (spikeObject != null)
        {
            targetPosition = spikeObject.position + Vector3.up * popUpDistance;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player"))
        {
            isTriggered = true;
            if (trapSound != null) audioSource.PlayOneShot(trapSound);
            StartCoroutine(PopSpike());
        }
    }

    private IEnumerator PopSpike()
    {
        if (spikeObject == null) yield break;

        // Fast pop up
        while (Vector3.Distance(spikeObject.position, targetPosition) > 0.01f)
        {
            spikeObject.position = Vector3.MoveTowards(spikeObject.position, targetPosition, popUpSpeed * Time.deltaTime);
            yield return null;
        }

        spikeObject.position = targetPosition;
    }
}
