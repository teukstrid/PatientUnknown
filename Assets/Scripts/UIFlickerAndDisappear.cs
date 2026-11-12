using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFlickerAndDisappear : MonoBehaviour
{
    [SerializeField, Min(0f)] private float flickerDuration = 1.5f;
    [SerializeField, Min(0.01f)] private float flickerInterval = 0.07f;
    [SerializeField, Min(0f)] private float fadeDuration = 0.4f;

    private CanvasGroup canvasGroup;
    private float startingAlpha;
    private Coroutine animation;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        startingAlpha = canvasGroup.alpha;
    }

    private void OnEnable()
    {
        canvasGroup.alpha = startingAlpha;
        animation = StartCoroutine(FlickerThenDisappear());
    }

    private void OnDisable()
    {
        if (animation != null)
        {
            StopCoroutine(animation);
            animation = null;
        }
    }

    private IEnumerator FlickerThenDisappear()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            canvasGroup.alpha = Random.Range(startingAlpha * 0.15f, startingAlpha);
            yield return new WaitForSecondsRealtime(flickerInterval);
            elapsed += flickerInterval;
        }

        float fromAlpha = canvasGroup.alpha;
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}