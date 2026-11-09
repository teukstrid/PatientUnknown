using UnityEngine;

public class SyringePickup : MonoBehaviour, IInteractable
{
    // Allows the player to interact with the syringe
    public bool CanInteract()
    {
        return true;
    }

    // Interaction logic for picking up the syringe
    // Checks if the player has a PlayerInventory component and adds the syringe to it
    public bool Interact(Interactor interactor)
    {
        PlayerInventory inventory =
         interactor.GetComponent<PlayerInventory>();

    if (inventory == null)
    {
        Debug.LogWarning("PlayerInventory not found!");
         return false;
     }

    if (inventory.AddSyringe())
     {
        Destroy(gameObject);
         Debug.Log("Syringe picked up!");
         return true;
     }

        Debug.Log("Already carrying a syringe.");
        return false;
     }
}
