using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;

    private bool hasOpened = false;

    public bool CanInteract()
    {
        return !hasOpened;
    }

    // When the player interacts with the door, play open animation
    public bool Interact(Interactor interactor)
    {
        if (hasOpened)
        {
            return false;
        }

        hasOpened = true;
        doorAnimator.SetBool("isOpen", true);

        return true;
    }
}