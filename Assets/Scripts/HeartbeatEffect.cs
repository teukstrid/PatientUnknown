
using UnityEngine;

public class HeartbeatEffect : MonoBehaviour
{
    [Header("Heartbeat Groups")]
    [SerializeField] private CanvasGroup noticingGroup;
    [SerializeField] private CanvasGroup chaseGroup;

    [Header("Noticing Pulse")]
    [SerializeField] private float noticingSpeed = 1.8f;
    [SerializeField] private float noticingMinAlpha = 0.15f;
    [SerializeField] private float noticingMaxAlpha = 0.45f;

    [Header("Chase Pulse")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseMinAlpha = 0.35f;
    [SerializeField] private float chaseMaxAlpha = 1f;

    private bool isNoticing;
    private bool isChasing;

    private void Awake()
    {
        SetGroupAlpha(noticingGroup, 0f);
        SetGroupAlpha(chaseGroup, 0f);
    }

    private void Update()
    {
        if (isNoticing)
        {
            float pulse =
                (Mathf.Sin(Time.time * noticingSpeed) + 1f) / 2f;

            SetGroupAlpha(noticingGroup, Mathf.Lerp(
                noticingMinAlpha, noticingMaxAlpha, pulse));
        }

        if (isChasing)
        {
            float pulse =
                (Mathf.Sin(Time.time * chaseSpeed) + 1f) / 2f;

            SetGroupAlpha(chaseGroup, Mathf.Lerp(
                chaseMinAlpha, chaseMaxAlpha, pulse));
        }
    }

    public void SetNoticing(bool active)
    {
        isNoticing = active;

        if (active)
        {
            isChasing = false;
            SetGroupAlpha(chaseGroup, 0f);
        }

        if (!active)
            SetGroupAlpha(noticingGroup, 0f);
    }

    public void SetChasing(bool active)
    {
        isChasing = active;

        if (active)
        {
            isNoticing = false;
            SetGroupAlpha(noticingGroup, 0f);
        }

        if (!active)
            SetGroupAlpha(chaseGroup, 0f);
    }

    private void SetGroupAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
            group.alpha = alpha;
    }
}