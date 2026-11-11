
using UnityEngine;
using UnityEngine.Events;

public class ElevatorInteraction : MonoBehaviour, IInteractable
{
    private static readonly int ElevatorCloseState = Animator.StringToHash("Base Layer.ElevatorClose");
    [SerializeField] private string _promptMessage = "Interact with Elevator";
    public string PromptMessage => _promptMessage;
    [SerializeField] private Animator elevatorAnimator;
    [SerializeField] private bool opensDoors;
    private bool _closingForVictory;
    public MusicManager audioManager;
    [SerializeField] private AudioClip doorOpenSFX;
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

        if (_closingForVictory)
        {
            return false;
        }

        if (!opensDoors && GameManager.Instance == null)
        {
            Debug.LogError(
                "Cannot start the victory sequence because there is no GameManager.",
                this);
            return false;
        }

        if (audioManager != null)
        {
            AudioClip clip = opensDoors ? doorOpenSFX : doorCloseSFX;
            audioManager.PlaySFX(clip);
        }

        elevatorAnimator.SetBool("isOpen", opensDoors);

        if (!opensDoors)
        {
            _closingForVictory = true;
            StartCoroutine(CloseDoorsAndWin());
        }

        return true;
    }

    private System.Collections.IEnumerator CloseDoorsAndWin()
    {
        // Wait until the closing animation starts.
        while (elevatorAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash
                != ElevatorCloseState ||
            elevatorAnimator.IsInTransition(0))
        {
            yield return null;
        }

        // Wait until the closing animation finishes.
        while (elevatorAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // Give the elevator sound time to play before the victory screen.
        yield return new WaitForSeconds(2f);

        GameManager.Instance.FadeToVictory();
    }

}
