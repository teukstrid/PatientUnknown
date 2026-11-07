using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;
    public AudioClip pauseMusic;

    [SerializeField, Min(0f)] private float transitionDuration = 0.5f;

    private Coroutine _musicTransition;
    private float _musicVolume;

    private void Awake()
    {
        if (musicSource == null)
        {
            Debug.LogError("Music AudioSource is not assigned.", this);
            return;
        }

        _musicVolume = musicSource.volume;
    }

    public void PlayMusic(AudioClip track, bool restart = false)
    {
        if (track == null)
        {
            Debug.LogError("Cannot play music because the track is not assigned.", this);
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("Cannot play music because the AudioSource is not assigned.", this);
            return;
        }

        // Don't restart the same track unless requested.
        if (musicSource.clip == track && musicSource.isPlaying && !restart && _musicTransition == null)
        {
            return;
        }

        StartMusicTransition(track);
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            Debug.LogError("Cannot stop music because the AudioSource is not assigned.", this);
            return;
        }

        StartMusicTransition(null);
    }

    private void StartMusicTransition(AudioClip track)
    {
        if (_musicTransition != null)
        {
            StopCoroutine(_musicTransition);
        }

        _musicTransition = StartCoroutine(ChangeMusic(track));
    }

    private System.Collections.IEnumerator ChangeMusic(AudioClip track)
    {
        yield return null;

        if (musicSource.isPlaying)
        {
            yield return FadeVolume(0f);
            musicSource.Stop();
        }

        if (track != null)
        {
            musicSource.clip = track;
            musicSource.loop = true;
            musicSource.time = 0f;
            musicSource.volume = 0f;
            musicSource.Play();
            yield return FadeVolume(_musicVolume);
        }

        _musicTransition = null;
    }

    private System.Collections.IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, Mathf.Clamp01(elapsed / transitionDuration));
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}