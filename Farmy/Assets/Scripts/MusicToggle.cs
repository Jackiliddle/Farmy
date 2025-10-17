using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    private AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    public void ToggleMusic()
    {
        if (music.isPlaying)
            music.Pause();
        else
            music.Play();
    }
}