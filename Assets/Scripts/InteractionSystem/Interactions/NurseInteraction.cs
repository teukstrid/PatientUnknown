
using UnityEngine;

public class NurseInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptMessage = "Sedate nurse";
    public string PromptMessage => _promptMessage;
    [SerializeField] private Animator npcAnimator;

    private bool isCollapsed = false;

    public bool CanInteract()
    {
        return !isCollapsed;
    }

    // When the player interacts with the nurse, check if they have a syringe in their inventory
    // If they do, use the syringe and trigger the collapse animation and disable the nurse
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
