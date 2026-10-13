using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource convoMusic;
    [SerializeField] private AudioSource yellowMusic;
    [SerializeField] private AudioSource blueMusic;
    [SerializeField] private AudioSource redMusic;
    [SerializeField] private AudioSource greenMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource tentacleSmack;
    [SerializeField] private AudioSource playerHit;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If a duplicate exists, destroy this GameObject to enforce the single instance
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        convoMusic.Play();
    }

    public void StopAll()
    {
        convoMusic.Stop();
        yellowMusic.Stop();
        blueMusic.Stop();
        redMusic.Stop();
        greenMusic.Stop();
    }

    public void ConvoMusic()
    {
        convoMusic.Play();
        yellowMusic.Stop();
        blueMusic.Stop();
        redMusic.Stop();
        greenMusic.Stop();
    }

    public void RedMusic()
    {
        convoMusic.Stop();
        yellowMusic.Stop();
        blueMusic.Stop();
        redMusic.Play();
        greenMusic.Stop();
    }

    public void YellowMuisc()
    {
        convoMusic.Stop();
        yellowMusic.Play();
        blueMusic.Stop();
        redMusic.Stop();
        greenMusic.Stop();
    }

    public void GreenMusic()
    {
        convoMusic.Stop();
        yellowMusic.Stop();
        blueMusic.Stop();
        redMusic.Stop();
        greenMusic.Play();
    }

    public void BlueMuisc()
    {
        convoMusic.Stop();
        yellowMusic.Stop();
        blueMusic.Play();
        redMusic.Stop();
        greenMusic.Stop();
    }

    public void HitNoise()
    {
        tentacleSmack.PlayOneShot(tentacleSmack.clip);
    }

    public void PlayerHitNoise()
    {
        playerHit.PlayOneShot(playerHit.clip);
    }
}