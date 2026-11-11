using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 30f;
    [SerializeField] private Vector3 _raycastOffset = new Vector3(0, 1f, 0);
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TMP_Text interactionText;

    private IInteractable currentInteractable;
    private Outline currentOutline;
    private Camera playerCamera;

    // Check if a player camera is assigned and check if an interaction prompt is assigned. If not, log an error.
    private void Awake()
    {
        playerCamera = Camera.main;

        if (interactionPrompt == null)
        {
            Debug.LogError("Interaction prompt is not assigned.", this);
        }
        else
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void Update()
    {
        // Update the focus on interactable objects and handle interaction input
        UpdateFocus();
        Ray a = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(a.origin, a.direction * _castDistance, Color.red);

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            currentInteractable != null &&
            currentInteractable.CanInteract())
        {
            currentInteractable.Interact(this);
        }
    }

    private void UpdateFocus()
    {
        IInteractable interactable = null;
        Ray ray = playerCamera != null
            ? new Ray(playerCamera.transform.position, playerCamera.transform.forward)
            : new Ray(transform.position + _raycastOffset, transform.forward);

        RaycastHit[] hits = Physics.RaycastAll(ray, _castDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            IInteractable hitInteractable = hit.collider.GetComponentInParent<IInteractable>();
            if (hitInteractable != null)
            {
                interactable = hitInteractable;
                break;
            }
        }

        if (currentInteractable == interactable)
        {
            return;
        }

        if (currentOutline != null)
        {
            currentOutline.enabled = false;
        }

        currentInteractable = interactable;
        currentOutline = null;

        if (currentInteractable != null && currentInteractable.ShouldShowOutline())
        {
            MonoBehaviour interactableObject = currentInteractable as MonoBehaviour;
            if (interactableObject != null)
            {
                currentOutline = interactableObject.GetComponent<Outline>();
                if (currentOutline == null)
                {
                    currentOutline = interactableObject.GetComponentInChildren<Outline>();
                }

                if (currentOutline == null)
                {
                    currentOutline = interactableObject.gameObject.AddComponent<Outline>();
                }

                currentOutline.OutlineMode = Outline.Mode.OutlineVisible;
                currentOutline.OutlineColor = Color.white;
                currentOutline.OutlineWidth = 2f;
                currentOutline.enabled = true;
            }
        }

        if (interactionPrompt != null)
        {
            bool canInteract = currentInteractable != null
                && currentInteractable.CanInteract();

            interactionPrompt.SetActive(canInteract);

            if (canInteract && interactionText != null)
            {
                interactionText.text = currentInteractable.PromptMessage;
            }
        }
    }
}
