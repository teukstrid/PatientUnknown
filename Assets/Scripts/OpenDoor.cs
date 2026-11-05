using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openStateName = "DoorOpen";
    [SerializeField] private string closeStateName = "DoorClose";

    private int _openStateHash;
    private int _closeStateHash;
    private bool _isOpen;
    private bool _isAnimating;
    private int _playingStateHash;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("OpenDoor needs an Animator reference.", this);
            enabled = false;
            return;
        }

        _openStateHash = Animator.StringToHash("Base Layer." + openStateName);
        _closeStateHash = Animator.StringToHash("Base Layer." + closeStateName);

        if (!animator.HasState(0, _openStateHash) || !animator.HasState(0, _closeStateHash))
        {
            Debug.LogError(
                "The assigned Animator Controller must contain the configured open and close states.",
                this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!_isAnimating)
        {
            return;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.fullPathHash == _playingStateHash &&
            state.normalizedTime >= 1f &&
            !animator.IsInTransition(0))
        {
            _isAnimating = false;
        }
    }

    public void OpenDoorAnimation()
    {
        if (!enabled || _isAnimating)
        {
            return;
        }

        _isOpen = !_isOpen;
        _playingStateHash = _isOpen ? _openStateHash : _closeStateHash;
        _isAnimating = true;
        animator.Play(_playingStateHash, 0, 0f);
    }
}
