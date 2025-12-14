using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private NavMeshAgent _agent;
    private Vector3 CurrentTarget;
    private Vector3 CurrentPosition;

    public event Action Arrived;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
    }

    private void Update()
    {
        CheckForArrival();
    }

    private void CheckForArrival()
    {
        if (CurrentTarget != Vector3.zero)
        {
            CurrentPosition = transform.position;
            CurrentPosition.y = 0f;
        
            if ((int)CurrentPosition.x == (int)CurrentTarget.x 
                && (int)CurrentPosition.z == (int)CurrentTarget.z)
                Arrived?.Invoke();
        }
    }

    public void SetTarget(Vector3 targetPosition)
    {
        CurrentTarget = targetPosition;
        CurrentTarget.y = 0;
        _agent.SetDestination(CurrentTarget);
        _agent.isStopped = false;
    }

    public void ResetTarget()
    {
        _agent.isStopped = true;
        CurrentTarget = Vector3.zero;
    }
}
