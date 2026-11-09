using UnityEngine;

public class CreepyPatient : MonoBehaviour
{
    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip laughter;

    // When the player enters the collider, play laughter SFX
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayLaughter();
        }
    }

    private void PlayLaughter()
    {
        if (audioSource != null && laughter != null)
        {
            audioSource.PlayOneShot(laughter);
        }
    }
}
