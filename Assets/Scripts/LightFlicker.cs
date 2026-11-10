
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float minIntensity = 0.05f;
    [SerializeField] private float maxIntensity = 0.7f;
    [SerializeField] private float flickerSpeed = 0.09f;

    [Header("Sound Settings")]
    [SerializeField] private PropSFX lightFlickerSFX;

    private Light flickerLight;
    private float originalIntensity;

    private void Awake()
    {
        flickerLight = GetComponent<Light>();
        originalIntensity = flickerLight.intensity;

        if (lightFlickerSFX == null)
            lightFlickerSFX = GetComponent<PropSFX>();
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void Start()
    {
        // Handle the initial state as well as future state changes.
        if (GameManager.Instance != null)
            HandleGameStateChanged(GameManager.Instance.CurrentState);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
            StartFlicker();
        else
            StopFlicker();
    }

    private void StartFlicker()
    {
        CancelInvoke(nameof(Flicker));

        InvokeRepeating(
            nameof(Flicker),
            0f,
            Mathf.Max(0.02f, flickerSpeed));

        if (lightFlickerSFX != null)
            lightFlickerSFX.PlaySFX();
    }

    private void StopFlicker()
    {
        CancelInvoke(nameof(Flicker));

        if (flickerLight != null)
            flickerLight.intensity = originalIntensity;

        if (lightFlickerSFX != null)
            lightFlickerSFX.StopSFX();
    }

    private void Flicker()
    {
        if (flickerLight == null) return;

        flickerLight.intensity =
            Random.Range(minIntensity, maxIntensity);
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        StopFlicker();
    }
}
