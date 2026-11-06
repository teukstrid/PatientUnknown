using UnityEngine;

public class SyringePickup : MonoBehaviour, IInteractable
{
    public bool CanInteract()
    {
        return true;
    }

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
