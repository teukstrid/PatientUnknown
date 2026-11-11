using UnityEngine;

public class DoorInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptMessage = "Open Door";
    public string PromptMessage => _promptMessage;
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private PropSFX doorSFX;

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
        StartCoroutine(OpenDoorWithSound());

        return true;
    }

    private System.Collections.IEnumerator OpenDoorWithSound()
    {
        doorSFX.PlaySFX();

        // Wait for the silence at the beginning of the clip.
        yield return new WaitForSeconds(0.6f);

        // Start the door animation.
        doorAnimator.SetBool("isOpen", true);
    }
}