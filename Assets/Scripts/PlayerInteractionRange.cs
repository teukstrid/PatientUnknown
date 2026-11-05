using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionRange : MonoBehaviour
{
    private readonly Dictionary<InteractableComponent, HashSet<Collider>> _collidersByInteractable =
        new Dictionary<InteractableComponent, HashSet<Collider>>();
    private readonly Dictionary<Collider, InteractableComponent> _interactableByCollider =
        new Dictionary<Collider, InteractableComponent>();

    private void OnTriggerEnter(Collider other)
    {
        InteractableComponent interactable = other.GetComponentInParent<InteractableComponent>();
        if (interactable == null || _interactableByCollider.ContainsKey(other))
        {
            return;
        }

        if (!_collidersByInteractable.TryGetValue(interactable, out HashSet<Collider> colliders))
        {
            colliders = new HashSet<Collider>();
            _collidersByInteractable.Add(interactable, colliders);
        }

        colliders.Add(other);
        _interactableByCollider.Add(other, interactable);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactableByCollider.TryGetValue(other, out InteractableComponent interactable))
        {
            _interactableByCollider.Remove(other);
            HashSet<Collider> colliders = _collidersByInteractable[interactable];
            colliders.Remove(other);

            if (colliders.Count == 0)
            {
                _collidersByInteractable.Remove(interactable);
            }
        }
    }

    public bool IsInRange(InteractableComponent interactable)
    {
        return interactable != null && _collidersByInteractable.ContainsKey(interactable);
    }

    private void OnDisable()
    {
        _collidersByInteractable.Clear();
        _interactableByCollider.Clear();
    }
}