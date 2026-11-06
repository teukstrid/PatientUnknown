
using UnityEngine;

public class ElevatorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator elevatorAnimator;
    [SerializeField] private bool opensDoors;

    public bool CanInteract()
    {
        return true;
    }

    public bool Interact(Interactor interactor)
    {
        elevatorAnimator.SetBool("isOpen", opensDoors);
        return true;
    }
}
