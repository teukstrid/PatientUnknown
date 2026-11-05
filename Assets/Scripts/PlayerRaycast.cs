
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField, Min(0f)] private float interactDistance = 3f;
    [SerializeField] private PlayerInteraction playerInteract;

    private ItemHighlight _currentItemHighlight;

    private void Awake()
    {
        if (playerCamera == null || playerInteract == null)
        {
            Debug.LogError("Assign the camera and PlayerInteraction references.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        CheckForInteractable();
    }

    private void CheckForInteractable()
    {
        InteractableComponent detectedInteractable = null;
        ItemHighlight detectedHighlight = null;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out RaycastHit hit,
            interactDistance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            InteractableComponent hitInteractable =
                hit.collider.GetComponentInParent<InteractableComponent>();

            if (hitInteractable != null)
            {
                detectedInteractable = hitInteractable;
                detectedHighlight = hit.collider.GetComponentInParent<ItemHighlight>();
            }
        }

        if (detectedHighlight != _currentItemHighlight)
        {
            if (_currentItemHighlight != null)
            {
                _currentItemHighlight.SetHighlighted(false);
            }

            _currentItemHighlight = detectedHighlight;

            if (_currentItemHighlight != null)
            {
                _currentItemHighlight.SetHighlighted(true);
            }
        }

        playerInteract.SetCurrentInteractable(detectedInteractable);
    }

    private void OnDisable()
    {
        if (_currentItemHighlight != null)
        {
            _currentItemHighlight.SetHighlighted(false);
            _currentItemHighlight = null;
        }

        if (playerInteract != null)
        {
            playerInteract.SetCurrentInteractable(null);
        }
    }
}
