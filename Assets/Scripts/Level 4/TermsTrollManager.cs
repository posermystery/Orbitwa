using UnityEngine;
using UnityEngine.UI;

public class TermsTrollManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainTermsPanel;
    public GameObject scrollContentPanel; // The long text panel that opens

    [Header("UI Controls")]
    public Button readMoreButton;
    public Button acceptButton;
    public Button declineButton;
    public Toggle agreeToggle;

    [Header("Audio")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // UI Setup
        mainTermsPanel.SetActive(true);
        if (scrollContentPanel != null) scrollContentPanel.SetActive(false); // Hidden initially

        // Button Listeners
        if (readMoreButton != null) readMoreButton.onClick.AddListener(ClickReadMore);
        if (acceptButton != null) acceptButton.onClick.AddListener(ClickAccept);
        if (declineButton != null) declineButton.onClick.AddListener(ClickDecline);
    }

    private void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void ClickReadMore()
    {
        PlayClickSound();
        // Open the massive scroll view
        if (scrollContentPanel != null) scrollContentPanel.SetActive(true);
        // Hide the Read More button itself so they have to scroll
        if (readMoreButton != null) readMoreButton.gameObject.SetActive(false);
    }

    public void ClickAccept()
    {
        PlayClickSound();

        // The Ultimate Check
        if (agreeToggle != null && agreeToggle.isOn)
        {
            // They actually ticked the hidden box! They win!
            mainTermsPanel.SetActive(false);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WinLevel();
            }
        }
        else
        {
            // Blind Accept Trap!
            mainTermsPanel.SetActive(false); // Hide UI to show death
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver("Did you even read the contract?! Clause 1: You die.");
            }
        }
    }

    public void ClickDecline()
    {
        PlayClickSound();
        
        // Decline Trap!
        mainTermsPanel.SetActive(false); // Hide UI to show death
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver("You refused the terms? Fine. Penalty: Immediate Death.");
        }
    }
}
