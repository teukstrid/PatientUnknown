
using System.Collections;
using UnityEngine;

public class NurseAudio : MonoBehaviour
{
    public enum AmbientMode
    {
        PlayOnceAtGameStart,
        RepeatAtIntervals
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource movementSource;
    [SerializeField] private AudioSource voiceSource;

    [Header("Footstep SFX")]
    [SerializeField] private AudioClip walkingFootstepSFX;
    [SerializeField] private AudioClip chasingFootstepSFX;

    [Header("Ambient VO")]
    [SerializeField] private AudioClip ambientVO;
    [SerializeField] private AmbientMode ambientMode;
    [SerializeField] private float minAmbientInterval = 15f;
    [SerializeField] private float maxAmbientInterval = 30f;

    [Header("Event VO / SFX")]
    [SerializeField] private AudioClip investigationVO;
    [SerializeField] private AudioClip detectionVO;
    [SerializeField] private AudioClip catchVO;
    [SerializeField] private AudioClip collapseSFX;

    private bool hasStartedAmbient;
    private bool hasPlayedInvestigation;
    private bool hasPlayedDetection;
    private bool hasPlayedCatch;
    private bool isChasing;

    private Coroutine ambientCoroutine;

    private void Awake()
    {
        if (movementSource != null)
        {
            movementSource.playOnAwake = false;
            movementSource.loop = false;
        }

        if (voiceSource != null)
        {
            voiceSource.playOnAwake = false;
            voiceSource.loop = false;
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;

        StopAllCoroutines();

        if (movementSource != null)
            movementSource.Stop();

        if (voiceSource != null)
            voiceSource.Stop();
    }

    // Pausing sounds and stopping the nurse from moving when the state has changed from
    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            if (voiceSource != null)
                voiceSource.UnPause();

            StartAmbientVO();
        }
        else if (state == GameState.Paused)
        {
            StopMovement();

            if (voiceSource != null)
                voiceSource.Pause();
        }
        else
        {
            StopMovement();
            StopAmbientVO();

            if (voiceSource != null)
                voiceSource.Stop();

            hasStartedAmbient = false;
        }
    }

    // FOOTSTEPS

    // Called by Animation Events on the nurse's walk animations.
    public void Footstep()
    {
        if (movementSource == null)
            return;

        AudioClip clip = isChasing
            ? chasingFootstepSFX
            : walkingFootstepSFX;

        if (clip != null)
            movementSource.PlayOneShot(clip);
    }

    public void SetWalking()
    {
        isChasing = false;
    }

    public void SetChasing()
    {
        isChasing = true;
    }

    public void StopMovement()
    {
        isChasing = false;
    }

    // AMBIENT VO

    private void StartAmbientVO()
    {
        if (hasStartedAmbient || ambientVO == null || voiceSource == null)
            return;

        hasStartedAmbient = true;

        if (ambientMode == AmbientMode.PlayOnceAtGameStart)
        {
            voiceSource.PlayOneShot(ambientVO);
        }
        else
        {
            ambientCoroutine = StartCoroutine(PlayAmbientRepeatedly());
        }
    }

    private IEnumerator PlayAmbientRepeatedly()
    {
        while (true)
        {
            if (ambientVO != null && voiceSource != null &&
                !voiceSource.isPlaying)
            {
                voiceSource.PlayOneShot(ambientVO);

                yield return new WaitForSeconds(ambientVO.length);
            }

            float interval = Random.Range(
                minAmbientInterval,
                Mathf.Max(minAmbientInterval, maxAmbientInterval));

            yield return new WaitForSeconds(interval);
        }
    }

    private void StopAmbientVO()
    {
        if (ambientCoroutine != null)
        {
            StopCoroutine(ambientCoroutine);
            ambientCoroutine = null;
        }
    }

    // ONE-SHOT EVENTS

    public void PlayInvestigation()
    {
        if (hasPlayedInvestigation)
            return;

        hasPlayedInvestigation = true;
        PlayVoice(investigationVO);
    }

    public void PlayDetection()
    {
        if (hasPlayedDetection)
            return;

        hasPlayedDetection = true;
        PlayVoice(detectionVO);
    }

    public void PlayCatch()
    {
        if (hasPlayedCatch)
            return;

        hasPlayedCatch = true;
        StopMovement();
        PlayVoice(catchVO);
    }

    public void PlayCollapse()
    {
        Debug.Log($"Clip: {collapseSFX}, Source: {movementSource}, Volume: {movementSource?.volume}");

        if (collapseSFX != null && movementSource != null)
        {
            movementSource.PlayOneShot(collapseSFX);
        }
    }

    private void PlayVoice(AudioClip clip)
    {
        if (clip != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(clip);
        }
    }
}