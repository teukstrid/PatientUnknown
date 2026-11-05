using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Observer : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Noticing,
        Chasing,
        Investigating
    }

    [SerializeField] private Transform player; // Reference the player's transform to assign in inspector (playerarmature)
    [SerializeField, Min(0f)] private float noticeDuration = 1.5f; // Time it takes for the nurse to notice the player before starting the chase
    [SerializeField, Min(0f)] private float lostPlayerWaitTime = 3f; // Time the nurse will wait at the last known position of the player before returning to patrol
    [SerializeField] private Image noticeIndicator; // UI element to show the nurse is noticing the player
    [SerializeField, Min(0f)] private float viewDistance = 8f; // How far the nurse can see the player
    [SerializeField, Range(0f, 360f)] private float viewAngle = 110f; // At what angle the nurse can see the player
    [SerializeField, Min(0f)] private float catchDistance = 1.2f; // How close the nurse needs to be to the player to catch them

    private NavMeshAgent _agent; // Reference to the navmeshagent attached to the nurse
    private NursePatrolCoroutines _patrol; // Reference to the NursePatrolCoroutines script attached to the nurse
    private State _state = State.Patrol; // Current state of the nurse
    private float _noticeTimer; // Timer to track how long the nurse has been noticing the player
    private float _searchTimer; // Timer to track how long the nurse has been waiting at the last known position of the player
    private bool _waitingAtLastKnownPosition; // Flag to indicate if the nurse is waiting at the last known position of the player
    private Vector3 _lastKnownPlayerPosition; // The last known position of the player that the nurse is investigating
    
    private void Awake()
    {
        _agent = GetComponentInParent<NavMeshAgent>(); // Get the navmeshagent component on this object or a parent
        _patrol = GetComponentInParent<NursePatrolCoroutines>(); // Get the NursePatrolCoroutines script on this object or a parent

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

        SetNoticeIndicator(false);
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
        bool canSeePlayer = CanSeePlayer(); // Check if the nurse can see the player

        // If the nurse can see the player, update the last known position and change the state accordingly. If the nurse cannot see the player, handle the current state and update timers if needed.
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
        _waitingAtLastKnownPosition = false;
        SetNoticeIndicator(false);
        _agent.isStopped = false;
        UpdateChasing();
    }

    // Update the chasing state, checking if the nurse is close enough to catch the player and updating its destination to the player's last known position
    private void UpdateChasing()
    {
        Vector3 distanceToPlayer = player.position - _agent.transform.position;
        distanceToPlayer.y = 0f;
        if (distanceToPlayer.sqrMagnitude <= catchDistance * catchDistance)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerCaught();
            }

            return;
        }

        _lastKnownPlayerPosition = player.position;
        _agent.SetDestination(_lastKnownPlayerPosition);
    }

    // The nurse begins investigating the player's last known position, stopping its patrol and updating its destination
    private void BeginInvestigating()
    {
        _state = State.Investigating;
        _waitingAtLastKnownPosition = false;
        _agent.isStopped = false;

        if (!_agent.SetDestination(_lastKnownPlayerPosition))
        {
            Debug.LogWarning("Observer could not set a path to the player's last known position.", this);
            StartWaitingAtLastKnownPosition();
        }
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

            if (_agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance)
            {
                return;
            }

            StartWaitingAtLastKnownPosition();
        }

        _searchTimer -= Time.deltaTime;
        if (_searchTimer <= 0f)
        {
            _state = State.Patrol;
            _patrol?.ResumePatrol();
        }
    }

    // Start waiting at the last known position, stopping the agent and setting the search timer
    private void StartWaitingAtLastKnownPosition()
    {
        _waitingAtLastKnownPosition = true;
        _searchTimer = lostPlayerWaitTime;
        _agent.isStopped = true;
    }

    // Set the visibility of the notice indicator
    private void SetNoticeIndicator(bool visible)
    {
        if (noticeIndicator != null)
        {
            noticeIndicator.enabled = visible;
            if (!visible)
            {
                noticeIndicator.fillAmount = 0f;
            }
        }
    }
}
