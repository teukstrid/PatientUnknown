using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip victoryMusic;
    public AudioClip pauseMusic;
    
    [Header("Gameplay Ambience")]
    public AudioSource ambienceSource;
    public AudioClip gameplayAmbience;
    public AudioClip chaseAmbience;
    public AudioClip gameplayMusic;
    private bool ambiencePaused;
    private bool isChasing;

    [SerializeField, Min(0f)] private float transitionDuration = 0.5f;

    private Coroutine _musicTransition;
    private float _musicVolume;

    [Header("Sound Effects")]
    public AudioSource sfxSource;

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

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

        public void PlayOrResumeAmbience()
    {
        AudioClip clip = isChasing ? chaseAmbience : gameplayAmbience;

        if (ambienceSource == null || clip == null)
        {
            Debug.LogError("Ambience source or selected clip is not assigned.", this);
            return;
        }

        ambienceSource.clip = clip;
        ambienceSource.loop = true;

        if (ambiencePaused)
        {
            ambienceSource.UnPause();
            ambiencePaused = false;
        }
        else if (!ambienceSource.isPlaying)
        {
            ambienceSource.Play();
        }
    }

    public void PauseAmbience()
    {
        if (ambienceSource != null && ambienceSource.isPlaying)
        {
            ambienceSource.Pause();
            ambiencePaused = true;
        }
    }

    public void StopAmbience()
    {
        if (ambienceSource != null)
        {
            ambienceSource.Stop();
        }

        ambiencePaused = false;
    }

    public void SetChaseAmbience(bool chasing)
    {
        isChasing = chasing;
        AudioClip clip = isChasing ? chaseAmbience : gameplayAmbience;

        if (ambienceSource == null || clip == null)
        {
            Debug.LogError("Ambience source or selected clip is not assigned.", this);
            return;
        }

        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.Play();
        ambiencePaused = false;
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