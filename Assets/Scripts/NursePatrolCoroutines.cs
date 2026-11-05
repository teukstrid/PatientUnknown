using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NursePatrolCoroutines : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints; // Exposed array of waypoints
    private int _currentWaypointIndex = 0; // The index of the current waypoint
    [SerializeField] private float _waitTime = 1f; // How long the nurse should wait before moving, can be adjusted in the inspector
    private NavMeshAgent _agent; // Reference the navmeshagent attached to the nurse
    private Coroutine _patrolCoroutine; // Reference to the running patrol coroutine
    [SerializeField] private bool _returnToFirstWaypoint = true; // Will the nurse patrol back to their starting position or not
    private int _direction = 1;
    [SerializeField] private float _moveSpeed = 2f; // Speed of the nurse
    

    private void Awake()
    {
        // Get the navmeshagent component on the same gameobject as this script and set the speed of the nurse
        _agent = GetComponent<NavMeshAgent>();

            if (_agent != null)
        {
            _agent.speed = _moveSpeed;
        }
    }

    private void Start()
    {
        // Check that the NavMeshAgent and waypoints are assigned
        if (_agent == null || _waypoints == null || _waypoints.Length == 0)
        {
            Debug.LogError("NursePatrol needs a NavMeshAgent and at least one waypoint.", this);
            enabled = false;
            return;
        }

        // Check that the nurse is standing on the baked NavMesh
        if (!_agent.isOnNavMesh)
        {
            Debug.LogError("Nurse must start on a baked NavMesh.", this);
            enabled = false;
            return;
        }

        // Begin patrolling from the first waypoint in the array
        _currentWaypointIndex = 0;
        _patrolCoroutine = StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        // Repeat the patrol indefinetely while this coroutine is running
        while (true)
        {
            Transform waypoint = _waypoints[_currentWaypointIndex]; // get the transform at the current index in the waypoint array

            _agent.isStopped = false; // Allow the nurse to move
            _agent.SetDestination(waypoint.position); // tell it to move to this waypoint
            Debug.Log($"Nurse moving to waypoint: {waypoint.name}");

            while (_agent.pathPending ||
                   _agent.remainingDistance > _agent.stoppingDistance)
            {
                yield return null;
            }

            // Stop and wait at this patrol point.
            _agent.isStopped = true;
            yield return new WaitForSeconds(_waitTime);

            // If returnToFirstWaypoint is true, the Nurse will move between the waypoints 0 -> 1 -> 2 -> 2 -> 0 etc
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
            // If returnToFirstWaypoint is false, the Nurse will never return to the first waypoint, 0 -> 1 -> 2 -> 1 -> 2 etc
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
    }

    public void ResumePatrol()
    {
        if (_patrolCoroutine == null && isActiveAndEnabled && _agent != null && _agent.isOnNavMesh)
        {
            _patrolCoroutine = StartCoroutine(Patrol());
        }
    }

    private void OnDisable()
    {
        // Check if we have a reference to a running coroutine. Stop the coroutine and clear reference.
        if (_patrolCoroutine != null)
        {
            StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
        }
    }
}