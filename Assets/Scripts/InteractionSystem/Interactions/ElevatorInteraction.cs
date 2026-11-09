
using UnityEngine;
using UnityEngine.Events;

public class ElevatorInteraction : MonoBehaviour, IInteractable
{
    private static readonly int ElevatorCloseState = Animator.StringToHash("Base Layer.ElevatorClose");

    [SerializeField] private Animator elevatorAnimator;
    [SerializeField] private bool opensDoors;
    private bool _closingForVictory;
    public MusicManager audioManager;
    public AudioClip doorOpenSFX;
    [SerializeField] private AudioClip doorCloseSFX;


    public bool CanInteract()
    {
        return !_closingForVictory;
    }

    // When the player interacts with the elevator
    // Check if an animator is assigned to the elevator, if not, log an error
    // Play the door open sound effect and opening animation
    public bool Interact(Interactor interactor)
    {
        if (elevatorAnimator == null)
        {
            Debug.LogError("Elevator Animator is not assigned.", this);
            return false;
        }
        audioManager.PlaySFX(doorOpenSFX);
        elevatorAnimator.SetBool("isOpen", opensDoors);

        if (!opensDoors)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("Cannot start the victory sequence because there is no GameManager.", this);
                return false;
            }

            _closingForVictory = true;
            StartCoroutine(CloseDoorsAndWin());
        }

        return true;
    }

    // Wait for the elevator doors to close and then trigger the victory event
    private System.Collections.IEnumerator CloseDoorsAndWin()
    {
        while (elevatorAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash != ElevatorCloseState ||
               elevatorAnimator.IsInTransition(0))
        {
            yield return null;
        }

        while (elevatorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        GameManager.Instance.FadeToVictory();
    }
}
