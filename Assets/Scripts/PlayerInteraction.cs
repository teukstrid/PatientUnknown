using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableComponent _currentInteractable;

    [SerializeField] private TMP_Text interactionPrompt;

    private void Start()
    {
        if (interactionPrompt == null)
        {
            Debug.LogError("Assign the interaction prompt text to PlayerInteraction.", this);
            return;
        }

        interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_currentInteractable != null &&
            UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            _currentInteractable.Interact();
        }
    }

    public void SetCurrentInteractable(InteractableComponent component)
    {
        if (_currentInteractable == component)
        {
            return;
        }

        _currentInteractable = component;

        if (interactionPrompt != null)
        {
            interactionPrompt.text = "Press E to interact";
            interactionPrompt.gameObject.SetActive(_currentInteractable != null);
        }
    }
}