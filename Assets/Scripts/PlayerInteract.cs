using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableComponent myComponent;

    [SerializeField] private TMP_Text interactionPrompt;

    private void Start()
    {
        if (interactionPrompt == null)
        {
            Debug.LogError("Assign the interaction prompt text to PlayerInteraction.", this);
            enabled = false;
            return;
        }

        interactionPrompt.enabled = true;
        interactionPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (myComponent != null &&
        myComponent.CanInteract &&
        UnityEngine.InputSystem.Keyboard.current != null &&
        UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            myComponent.Interact();
        }
    }

    public void SetCurrentInteractable(InteractableComponent component)
    {
        if (interactionPrompt == null)
        {
            Debug.LogError("Interaction prompt is not assigned!", this);
            return;
        }

        if (myComponent == component)
        {
            return;
        }

        myComponent = component;
        interactionPrompt.text = "Press E to interact";
        interactionPrompt.gameObject.SetActive(component != null);
    }
}