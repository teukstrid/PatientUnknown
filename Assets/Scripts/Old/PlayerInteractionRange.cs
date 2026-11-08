using UnityEngine;
using System.Collections.Generic;

public class PlayerInteractionRange : MonoBehaviour
{
    public List<InteractableComponent> nearbyInteractables =
    new List<InteractableComponent>();

    void OnTriggerEnter(Collider other)
    {
        InteractableComponent component =
        other.GetComponentInParent<InteractableComponent>();

        if (component != null &&
            !nearbyInteractables.Contains(component))
        {
            nearbyInteractables.Add(component);
            Debug.Log("Interactable entered range: " + component.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        InteractableComponent component =
        other.GetComponentInParent<InteractableComponent>();

        if (component != null)
        {
            nearbyInteractables.Remove(component);
            Debug.Log("Interactable left range: " + component.name);
        }
    }
}
