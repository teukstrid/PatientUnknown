using UnityEngine;
using UnityEngine.Events;

public class InteractableComponent : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract = new UnityEvent();
    [SerializeField] private bool interactOnce;

    private bool hasInteracted;

    public bool CanInteract => !interactOnce || !hasInteracted;

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        onInteract?.Invoke();

        if (interactOnce)
        {
            hasInteracted = true;
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
