using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    private InteractableComponent myComponent;

    // Create a reference to a text object that display the interaction prompt
    // [SerielizeFiel] lets you assign the text object in the inspector despite being private
    [SerializeField] private TMP_Text interactionPrompt;

    void Start()
    {
        // At the start of the game, the interaction prompt is hidden
        interactionPrompt.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check if the player has an interactable object available and if the player press E
        if (myComponent != null &&
        UnityEngine.InputSystem.Keyboard.current != null &&
        UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            myComponent.Interact();
        }
    }

    public void SetCurrentInteractable(InteractableComponent component)
    {
        myComponent = component;

        if (interactionPrompt == null)
        {
            Debug.LogError("Interaction prompt is not assigned!");
            return;
        }

        interactionPrompt.text = "Press E to interact";
        interactionPrompt.gameObject.SetActive(component != null);

        Debug.Log("Prompt visibility: " + (component != null));
    }
}