using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    struct AudioObject
    {
        string Name;
        AudioSource AudioSource;
    }
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] List<AudioObject> MusicObjectList = new List<AudioObject>();
    [SerializeField] private AudioSource convoMusic;
    [SerializeField] private AudioSource yellowMusic;
    [SerializeField] private AudioSource blueMusic;
    [SerializeField] private AudioSource redMusic;
    [SerializeField] private AudioSource greenMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource tentacleSmack;
    [SerializeField] private AudioSource playerHit;
    [SerializeField] private AudioSource laughTrack;

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
        // start all then pause all but convo music since thats what game starts on
        StartAll();
        PauseAll();
        convoMusic.UnPause();
    }

    public void StartAll()
    {
        convoMusic.Play();
        yellowMusic.Play();
        blueMusic.Play();
        redMusic.Play();
        greenMusic.Play();
    }

    public void PauseAll()
    {
        convoMusic.Pause();
        yellowMusic.Pause();
        blueMusic.Pause();
        redMusic.Pause();
        greenMusic.Pause();
    }

    public void ConvoMusic()
    {
        PauseAll();
        convoMusic.UnPause();
    }

    public void RedMusic()
    {
        PauseAll();
        redMusic.UnPause();
    }

    public void YellowMuisc()
    {
        PauseAll();
        yellowMusic.UnPause();
    }

    public void GreenMusic()
    {
        PauseAll();
        greenMusic.UnPause();
    }

    public void BlueMuisc()
    {
        PauseAll();
        blueMusic.UnPause();
    }

    public void HitNoise()
    {
        tentacleSmack.PlayOneShot(tentacleSmack.clip);
    }

    public void PlayerHitNoise()
    {
        playerHit.PlayOneShot(playerHit.clip);
    }

    public void LaughTrack()
    {
        laughTrack.PlayOneShot(laughTrack.clip);
    }
}