using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator doorAnimator;

    private bool hasOpened = false;

    public bool CanInteract()
    {
        return !hasOpened;
    }

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