using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource convoMusic;
    [SerializeField] AudioSource yellowMusic;
    [SerializeField] AudioSource blueMusic;
    [SerializeField] AudioSource redMusic;
    [SerializeField] AudioSource greenMusic;

    private void Start()
    {
        convoMusic.Play();
        Debug.Log(convoMusic.isPlaying);
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("blue music");
            BlueMuisc();
        }
    }
}