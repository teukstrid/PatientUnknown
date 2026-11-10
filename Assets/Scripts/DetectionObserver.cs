using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Observer : MonoBehaviour
{
    // States the nurses can be in
    private enum State
    {
        Patrol,
        Noticing,
        Chasing,
        Investigating
    }

    [SerializeField] private Transform player; // Reference the player's transform to assign in inspector (playerarmature)
    [SerializeField] private GameObject investigationGhost;
    [SerializeField, Min(0f)] private float noticeDuration = 1.5f; // Time it takes for the nurse to notice the player before starting the chase
    [SerializeField, Min(0f)] private float lostPlayerWaitTime = 3f; // Time the nurse will wait at the last known position of the player before returning to patrol
    [SerializeField] private Image noticeIndicator; // UI element to show the nurse is noticing the player
    [SerializeField, Min(0f)] private float viewDistance = 8f; // How far the nurse can see the player
    [SerializeField, Range(0f, 360f)] private float viewAngle = 110f; // At what angle the nurse can see the player
    [SerializeField, Min(0f)] private float catchDistance = 1.2f; // How close the nurse needs to be to the player to catch them

    [Header("Syringe Interaction")]
    [SerializeField, Min(0f)] private float syringeInteractionDistance = 2f; // How close the player needs to be to the nurse to use a syringe on them
    [SerializeField, Range(90f, 180f)] private float behindAngle = 120f; // The angle behind the nurse that the player must be in to use a syringe on them

    private PlayerInventory _playerInventory; // Reference to the PlayerInventory script attached to the player
    private bool _isDisabled; // Flag to indicate if the nurse has been disabled by the player
    private NavMeshAgent _agent; // Reference to the navmeshagent attached to the nurse
    private NursePatrolCoroutines _patrol; // Reference to the NursePatrolCoroutines script attached to the nurse
    private Animator _animator; // Reference to the animator attached to the nurse
    
    private State _state = State.Patrol; // Current state of the nurse
    private float _noticeTimer; // Timer to track how long the nurse has been noticing the player
    private float _searchTimer; // Timer to track how long the nurse has been waiting at the last known position of the player
    private bool _waitingAtLastKnownPosition; // Flag to indicate if the nurse is waiting at the last known position of the player
    private Vector3 _lastKnownPlayerPosition; // The last known position of the player that the nurse is investigating
    private bool _isCatching;
    private NursePatrolCoroutines _nursePatrol;

    [SerializeField] private float _chaseSpeed = 5;
    
    private void Awake()
    {
        _agent = GetComponentInParent<NavMeshAgent>(); // Get the navmeshagent component on this object or a parent
        _patrol = GetComponentInParent<NursePatrolCoroutines>(); // Get the NursePatrolCoroutines script on this object or a parent
        _animator = GetComponentInChildren<Animator>();
        HideInvestigationGhost(); // Hide the ghost imprint of the player

        if (player != null)
        {
            _playerInventory = player.GetComponentInParent<PlayerInventory>();

            if (_playerInventory == null)
                _playerInventory = player.GetComponentInChildren<PlayerInventory>();
        }

        if (_animator == null)
        {
            Debug.LogError("Observer could not find the nurse's Animator.", this);
        }

        if (_agent == null)
        {
            Debug.LogError("Observer needs a NavMeshAgent on this object or a parent.", this);
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogError("Assign the player Transform to the Observer.", this);
        }

        SetNoticeIndicator(false); // Start the game with the notice indicator hidden
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("A GameManager must be active in the scene for the Nurse catch behavior.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if (_isDisabled || _isCatching)
        return;

        bool canSeePlayer = CanSeePlayer(); // Check if the nurse can see the player

        // If the nurse can see the player, update the last known position and change the state accordingly.
        // If the nurse cannot see the player, handle the current state and update timers if needed.
        if (canSeePlayer)
        {
            _lastKnownPlayerPosition = player.position;

            if (_state == State.Patrol)
            {
                BeginNoticing();
            }
            else if (_state == State.Investigating)
            {
                BeginChasing();
            }
        }
        else if (_state == State.Noticing)
        {
            Debug.Log("Noticing cancelled: nurse lost sight of player.");
            CancelNoticing();
        }
        else if (_state == State.Chasing)
        {
            BeginInvestigating();
        }

        // State machine to handle the nurse's states
        switch (_state)
        {
            case State.Noticing:
                UpdateNoticing();
                break;
            case State.Chasing:
                UpdateChasing();
                break;
            case State.Investigating:
                UpdateInvestigation();
                break;
        }
        
        // If the player presses E button, call the trydisablewithsyringe method
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryDisableWithSyringe();
        }
    }

    // Check if the nurse can see the player based on distance and angle
    //TODO: Add raycast to check for obstacles between the nurse and the player
    private bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 directionToPlayer = player.position - _agent.transform.position;
        if (directionToPlayer.magnitude > viewDistance)
        {
            return false;
        }

        directionToPlayer.y = 0f;
        Vector3 forward = _agent.transform.forward;
        forward.y = 0f;

        return Vector3.Angle(forward, directionToPlayer) <= viewAngle * 0.5f;
    }

    // The nurse begins noticing the player, stopping its patrol and starting the notice timer
    private void BeginNoticing()
    {
        Debug.Log("Nurse entered Noticing state!");

        _state = State.Noticing;
        _noticeTimer = 0f;
        _agent.isStopped = true;
        _patrol?.PausePatrol();
        SetNoticeIndicator(true);
        UpdateNoticeIndicator();
    }

    // Update the noticing state, incrementing the notice timer and updating the notice indicator
    private void UpdateNoticing()
    {
        _noticeTimer += Time.deltaTime;
        UpdateNoticeIndicator();

        if (_noticeTimer >= noticeDuration)
        {
            BeginChasing();
        }
    }

    // Update the notice indicator's fill amount based on the notice timer and duration
    private void UpdateNoticeIndicator()
    {
        if (noticeIndicator != null)
        {
            noticeIndicator.fillAmount = noticeDuration <= 0f
                ? 1f
                : Mathf.Clamp01(_noticeTimer / noticeDuration);

            Debug.Log($"Notice fill: {noticeIndicator.fillAmount:F2}");
        }
    }

    // Cancel the nurse's noticing state, resetting the notice timer and going back to patrol
    private void CancelNoticing()
    {
        _state = State.Patrol;
        _noticeTimer = 0f;
        SetNoticeIndicator(false);
        _patrol?.ResumePatrol();
    }

    // The nurse begins chasing the player, stopping its patrol and updating its destination to the player's last known position
    private void BeginChasing()
    {
        _state = State.Chasing;
        _agent.speed = _chaseSpeed;

        HideInvestigationGhost();
        Debug.Log("AI STATE: CHASING", this);
        _waitingAtLastKnownPosition = false;

        SetNoticeIndicator(false);

        SetAnimations(false, true, false);

        _agent.isStopped = false;
        UpdateChasing();
    }

    // Update the chasing state, checking if the nurse is close enough to catch the player and updating its destination to the player's last known position
    private void UpdateChasing()
    {
        if (_isCatching)
            return;

        Vector3 distanceToPlayer = player.position - _agent.transform.position;
        distanceToPlayer.y = 0f;

        if (distanceToPlayer.sqrMagnitude <= catchDistance * catchDistance)
        {
            CatchPlayer();
            return;
        }

        _lastKnownPlayerPosition = player.position;
        _agent.SetDestination(_lastKnownPlayerPosition);
    }

    // The nurse begins investigating the player's last known position, stopping its patrol and updating its destination
    private void BeginInvestigating()
    {
        _state = State.Investigating;
        ShowInvestigationGhost();
        Debug.Log("AI STATE: INVESTIGATING", this);
        _waitingAtLastKnownPosition = false;
        SetAnimations(false, true, false);
        _agent.isStopped = false;

        if (!_agent.SetDestination(_lastKnownPlayerPosition))
        {
            Debug.LogWarning("Observer could not set a path to the player's last known position.", this);
            StartWaitingAtLastKnownPosition();
        }

        if (_patrol != null)
        _agent.speed = _patrol._moveSpeed;
    }

    // Update the investigating state, checking if the nurse has reached the last known position and starting the wait timer if so
    private void UpdateInvestigation()
    {
        if (!_waitingAtLastKnownPosition)
        {
            if (_agent.pathPending)
            {
                return;
            }

            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                return;
            }

            StartWaitingAtLastKnownPosition();
        }

        _searchTimer -= Time.deltaTime;

        if (_searchTimer <= 0f)
        {
            Debug.Log("AI STATE: RETURNING TO PATROL", this);
            _state = State.Patrol;
            HideInvestigationGhost();

            SetAnimations(false, false, false);

            _patrol?.ResumePatrol();
        }
    }

    // When the nurse has lost the player, a "ghost" of the player will be left on the last known position showing
    // where the nurse will stop to investigate before going back to it's patrol
    // The method also makes sure the ghost does not spawn in the air or clipping through the ground
    // And it plays an idle animation so it does not t-pose
    private void ShowInvestigationGhost()
    {
        if (investigationGhost == null)
            return;

        Vector3 rayOrigin = _lastKnownPlayerPosition + Vector3.up * 1.5f;
        RaycastHit[] groundHits = Physics.RaycastAll(
            rayOrigin,
            Vector3.down,
            10f,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore
        );
        System.Array.Sort(groundHits, (a, b) => a.distance.CompareTo(b.distance));

        bool foundGround = false;
        RaycastHit groundHit = default;
        foreach (RaycastHit hit in groundHits)
        {
            Transform hitTransform = hit.transform;
            if (player != null &&
                (hitTransform == player ||
                 hitTransform.IsChildOf(player) ||
                 player.IsChildOf(hitTransform)))
            {
                continue;
            }

            groundHit = hit;
            foundGround = true;
            break;
        }

        if (!foundGround)
        {
            Debug.LogWarning("Observer could not find a ground collider near the player's last known position; the ghost will remain hidden.", this);
            return;
        }

        float modelOffset = investigationGhost.transform.childCount > 0
            ? investigationGhost.transform.GetChild(0).localPosition.y
            : 0f;
        investigationGhost.transform.SetPositionAndRotation(
            groundHit.point - Vector3.up * modelOffset,
            player.rotation
        );
        investigationGhost.SetActive(true);

        Animator ghostAnimator = investigationGhost.GetComponentInChildren<Animator>();
        if (ghostAnimator != null)
        {
            foreach (AnimatorControllerParameter parameter in ghostAnimator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    if (parameter.name == "Grounded")
                        ghostAnimator.SetBool(parameter.name, true);
                    else if (parameter.name == "isWalking" ||
                             parameter.name == "isChasing" ||
                             parameter.name == "isSearching" ||
                             parameter.name == "Jump" ||
                             parameter.name == "FreeFall")
                        ghostAnimator.SetBool(parameter.name, false);
                }
                else if (parameter.type == AnimatorControllerParameterType.Float &&
                         (parameter.name == "Speed" || parameter.name == "MotionSpeed"))
                {
                    ghostAnimator.SetFloat(parameter.name, 0f);
                }
            }
        }
    }

    // Hide the ghost
    private void HideInvestigationGhost()
    {
        if (investigationGhost != null)
        {
            investigationGhost.SetActive(false);
        }
    }

    // Start waiting at the last known position, stopping the agent and setting the search timer
    private void StartWaitingAtLastKnownPosition()
    {
        _waitingAtLastKnownPosition = true;
        _searchTimer = lostPlayerWaitTime;
        _agent.isStopped = true;

        SetAnimations(false, false, true);
    }

    // Set the visibility of the notice indicator
    private void SetNoticeIndicator(bool visible)
    {
        if (noticeIndicator != null)
        {
            Debug.Log($"Notice indicator: {visible}, Image: {noticeIndicator.name}");

            noticeIndicator.enabled = visible;
            if (!visible)
            {
                noticeIndicator.fillAmount = 0f;
            }
        }
    }

    // Try disabling the nurse with a syringe. Can only be done in certain states and if the angle and distance to the nurse
    // is correct
    private void TryDisableWithSyringe()
    {
        // The nurse must be patrolling or investigating/searching.
        if (_state != State.Patrol &&
            _state != State.Investigating)
            return;

        if (_isDisabled || _isCatching)
            return;

        if (player == null || _playerInventory == null)
            return;

        // The player must have a syringe.
        if (!_playerInventory.HasSyringe)
            return;

        Vector3 toPlayer = player.position - _agent.transform.position;
        toPlayer.y = 0f;

        // The player must be close enough.
        if (toPlayer.magnitude > syringeInteractionDistance)
            return;

        // The player must be behind the nurse.
        float angle = Vector3.Angle(
            _agent.transform.forward,
            toPlayer
        );

        if (angle < behindAngle)
            return;

        // Do not allow the interaction if the nurse can see the player.
        if (CanSeePlayer())
            return;

        // Consume the syringe only after all checks pass.
        if (!_playerInventory.UseSyringe())
            return;

        StartCoroutine(CollapseNurse());
    }

    // If a syringe is used on the nurse at the right angle and state, this method will run and the nurse will play a
    // collapse animation and be disabled (not able to move or detect the player)
    private System.Collections.IEnumerator CollapseNurse()
    {
        _isDisabled = true;
        _state = State.Patrol;
        HideInvestigationGhost();

        // Stop movement and clear the path.
        _agent.isStopped = true;
        _agent.ResetPath();

        // Stop the patrol coroutine.
        _patrol?.PausePatrol();

        // Clear the detection indicator.
        SetNoticeIndicator(false);

        if (_animator != null)
        {
            // Clear other animation states.
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isChasing", false);
            _animator.SetBool("isSearching", false);

            // Trigger Collapse after pausing the patrol.
            _animator.ResetTrigger("Catch");
            _animator.ResetTrigger("Collapse");
            _animator.SetTrigger("Collapse");

            // Check whether the Animator has the Collapse trigger.
            bool hasCollapseTrigger = System.Array.Exists(
                _animator.parameters,
                p => p.name == "Collapse" &&
                    p.type == AnimatorControllerParameterType.Trigger
            );

            string controllerName =
                _animator.runtimeAnimatorController != null
                    ? _animator.runtimeAnimatorController.name
                    : "None";

            Debug.Log(
                "Collapse trigger sent. Animator enabled: " +
                _animator.enabled +
                ", Controller: " + controllerName +
                ", Parameter exists: " + hasCollapseTrigger,
                _animator
            );
        }
        else
        {
            Debug.LogError("Cannot play Collapse: nurse Animator is null.", this);
        }
        GetComponent<NurseAudio>()?.PlayCollapse();
        yield break;
    }

    // If the nurse get within range of the player, the CatchPlayer method is run
    // This locks the player movement, play a catch animation and SFX before fading to black and restarting the game
    private void CatchPlayer()
    {
        _isCatching = true;
        _state = State.Chasing;

        // Stop the nurse from moving.
        _agent.isStopped = true;
        _agent.ResetPath();

        // Stop the patrol coroutine.
        _patrol?.PausePatrol();

        // Tell the GameManager the player has been caught.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCaught();
        }

        // Play the Catch animation.
        if (_animator != null)
        {
            _animator.SetBool("isWalking", false);
            _animator.SetBool("isChasing", false);
            _animator.SetBool("isSearching", false);
            _animator.ResetTrigger("Catch");
            Debug.Log(
            $"Triggering Catch on Animator: {_animator.gameObject.name}",
            _animator
         );
            _animator.SetTrigger("Catch");
        }
    }

    // Set the animation state of the nurse
    private void SetAnimations(bool walking, bool chasing, bool searching)
    {
        if (_animator == null) return;

        _animator.SetBool("isWalking", walking);
        _animator.SetBool("isChasing", chasing);
        _animator.SetBool("isSearching", searching);
    }
}
