
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PropSFX : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField] private AudioClip sfxClip;
    [SerializeField] private bool loopForever = false;
    [SerializeField] private bool playOnAwake = false;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = loopForever;
        audioSource.clip = sfxClip;
    }

    private void Start()
    {
        if (playOnAwake && sfxClip != null)
        {
            PlaySFX();
        }
    }

    public void PlaySFX()
    {
        if (sfxClip == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: No SFX clip assigned.", this);
            return;
        }

        if (loopForever)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.PlayOneShot(sfxClip);
        }
    }

    public void StopSFX()
    {
        AudioSource source = GetComponent<AudioSource>();

        if (source != null)
            source.Stop();
    }
}
