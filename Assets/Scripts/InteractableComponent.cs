using UnityEngine;
using UnityEngine.Events;

public class InteractableComponent : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract = new UnityEvent();

    public void Interact()
    {
        onInteract?.Invoke();
    }
}
