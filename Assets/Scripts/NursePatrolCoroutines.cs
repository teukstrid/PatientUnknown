
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NursePatrolCoroutines : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;

    [SerializeField] private float _waitTime = 1f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private Coroutine _patrolCoroutine;

    [SerializeField] private bool _returnToFirstWaypoint = true;
    private int _direction = 1;

    [SerializeField] private float _moveSpeed = 2f;

    private void Awake()
    {
        // The NavMeshAgent and patrol script are on the parent.
        _agent = GetComponent<NavMeshAgent>();

        // The Animator is on the child character model.
        _animator = GetComponentInChildren<Animator>();

        if (_agent != null)
        {
            _agent.speed = _moveSpeed;
        }

        if (_animator == null)
        {
            Debug.LogWarning(
                "No Animator found on the nurse or its children.", this);
        }
    }

    private void Start()
    {
        if (_agent == null ||
            _waypoints == null ||
            _waypoints.Length == 0)
        {
            Debug.LogError(
                "NursePatrol needs a NavMeshAgent and at least one waypoint.",
                this);

            enabled = false;
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            Debug.LogError(
                "Nurse must start on a baked NavMesh.", this);

            enabled = false;
            return;
        }

        _currentWaypointIndex = 0;
        _patrolCoroutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            Transform waypoint = _waypoints[_currentWaypointIndex];

            // Begin walking.
            _agent.isStopped = false;
            _agent.SetDestination(waypoint.position);
            SetWalking(true);

            Debug.Log($"Nurse moving to waypoint: {waypoint.name}");

            while (_agent.pathPending ||
                   _agent.remainingDistance > _agent.stoppingDistance)
            {
                yield return null;
            }

            // Stop walking and wait at the waypoint.
            _agent.isStopped = true;
            SetWalking(false);

            yield return new WaitForSeconds(_waitTime);

            // Preserve the existing waypoint behaviour.
            if (_returnToFirstWaypoint)
            {
                _currentWaypointIndex += _direction;

                if (_currentWaypointIndex >= _waypoints.Length - 1)
                {
                    _direction = -1;
                }
                else if (_currentWaypointIndex <= 0)
                {
                    _direction = 1;
                }
            }
            else
            {
                if (_currentWaypointIndex == _waypoints.Length - 1)
                {
                    _direction = -1;
                }

                if (_currentWaypointIndex == 1)
                {
                    _direction = 1;
                }

                _currentWaypointIndex += _direction;
            }
        }
    }

    private void SetWalking(bool walking)
    {
        if (_animator != null)
        {
            _animator.SetBool("isWalking", walking);
        }
    }

    public void PausePatrol()
    {
        if (_patrolCoroutine != null)
        {
            StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
        }

        if (_agent != null)
        {
            _agent.isStopped = true;
        }

        SetWalking(false);
    }

    public void ResumePatrol()
    {
        if (_patrolCoroutine == null &&
            isActiveAndEnabled &&
            _agent != null &&
            _agent.isOnNavMesh)
        {
            _patrolCoroutine = StartCoroutine(Patrol());
        }
    }

    private void OnDisable()
    {
        if (_patrolCoroutine != null)
        {
            StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
        }

        SetWalking(false);
    }
}