using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 30f;
    [SerializeField] private Vector3 _raycastOffset = new Vector3(0, 1f, 0);
    [SerializeField] private TMP_Text interactionPrompt;

    private IInteractable currentInteractable;
    private Outline currentOutline;
    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            Debug.LogError("No camera tagged MainCamera was found.", this);
        }

        if (interactionPrompt == null)
        {
            Debug.LogError("Interaction prompt is not assigned.", this);
        }
        else
        {
            interactionPrompt.enabled = true;
            interactionPrompt.text = "Press E";
            interactionPrompt.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
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
        // Find the closest interactable object in front of the player within the cast distance
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

        // early out if the interactable hasn't changed
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

        // outline logic
        if (currentInteractable != null)
        {
            MonoBehaviour interactableObject = currentInteractable as MonoBehaviour;
            currentOutline = interactableObject.GetComponent<Outline>();

            // create default outline if interactable wants to and don't have an outline already
            if (currentOutline == null && currentInteractable.ShouldShowOutline())
            {
                currentOutline = interactableObject.gameObject.AddComponent<Outline>();
                currentOutline.OutlineMode = Outline.Mode.OutlineVisible;
                currentOutline.OutlineColor = Color.yellow;
                currentOutline.OutlineWidth = 2f;
                currentOutline.enabled = true;
            }

        }

        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(currentInteractable != null);
        }
    }
}
