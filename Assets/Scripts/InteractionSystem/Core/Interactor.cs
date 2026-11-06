using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private float _castDistance = 5f;
    [SerializeField] private Vector3 _raycastOffset = new Vector3(0, 1f, 0);



    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E was pressed!");

            if (DoInteractionTest(out IInteractable interactable))
            {
                Debug.Log("Interactable found!");

                if (interactable.CanInteract())
                {
                    Debug.Log("Can interact!");

                    interactable.Interact(this);
                }
                else
                {
                    Debug.Log("Cannot interact!");
                }
            }
            else
            {
                Debug.Log("No interactable found!");
            }
        }
    }
    

    private bool DoInteractionTest(out IInteractable interactable)
    {
        interactable = null;

        Ray ray = new Ray(
            transform.position + _raycastOffset,
            transform.forward
        );

        Debug.DrawRay(ray.origin, ray.direction * _castDistance, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, _castDistance))
        {
            Debug.Log("Raycast hit: " + hitInfo.collider.name);

            interactable = hitInfo.collider.GetComponentInParent<IInteractable>();

            if (interactable != null)
            {
                Debug.Log("IInteractable found on hit object!");
                return true;
            }

            Debug.Log("Hit object, but it has no IInteractable component.");
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
        }

        return false;
    }
}