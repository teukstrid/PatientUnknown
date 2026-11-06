using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private PlayerInteractionRange interactionRange;
    [SerializeField] private PlayerInteract playerInteract;

    private InteractableComponent currentInteractable;
    private ItemHighlight currentItemHighlight;

    void Update()
    {
        Debug.LogError("RAYCAST RUNNING");
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        InteractableComponent detectedInteractable = null;
        ItemHighlight detectedHighlight = null;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit,
            interactDistance))
        {
            detectedInteractable =
                hit.collider.GetComponentInParent<InteractableComponent>();

            detectedHighlight =
                hit.collider.GetComponentInParent<ItemHighlight>();

            // Reject objects outside interaction range.
            if (detectedInteractable != null &&
                !interactionRange.nearbyInteractables.Contains(detectedInteractable))
            {
                detectedInteractable = null;
                detectedHighlight = null;
            }
        }

        // Update highlighting.
        if (detectedHighlight != currentItemHighlight)
        {
            if (currentItemHighlight != null)
                currentItemHighlight.SetHighlighted(false);

            currentItemHighlight = detectedHighlight;

            if (currentItemHighlight != null)
                currentItemHighlight.SetHighlighted(true);
        }

        currentInteractable = detectedInteractable;
        playerInteract.SetCurrentInteractable(currentInteractable);
    }
}
