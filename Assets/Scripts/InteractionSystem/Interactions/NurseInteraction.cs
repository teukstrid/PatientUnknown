
using UnityEngine;

public class NurseInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator npcAnimator;

    private bool isCollapsed = false;

    public bool CanInteract()
    {
        return !isCollapsed;
    }

    public bool Interact(Interactor interactor)
    {
        PlayerInventory inventory =
            interactor.GetComponent<PlayerInventory>();

        if (inventory == null || !inventory.HasSyringe)
        {
            Debug.Log("You need a syringe to interact with this NPC.");
            return false;
        }

        if (inventory.UseSyringe())
        {
            npcAnimator.SetTrigger("Collapse");
            isCollapsed = true;

            Debug.Log("NPC collapsed!");
            return true;
        }

        return false;
    }
}
