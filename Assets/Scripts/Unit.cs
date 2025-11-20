using System;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private Transform _pickUpPosition;

    private UnitMover _mover;

    public IPickable _pickedUpObject;
    public bool IsCarrying { get; private set; }
    public States CurrentState { get; set; }

    public event Action<Unit> ResourceTaken;
    public event Action<Unit> ResourceDelivered;
    public event Action<Unit> ResourceNotFound;

    public enum States
    {
        Idle,
        Working
    }

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
        IsCarrying = false;
        CurrentState = States.Idle;
    }

    private void OnEnable()
    {
        _mover.Arrived += CheckForPickedResource;
    }

    private void OnDisable()
    {
        _mover.Arrived -= CheckForPickedResource;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCarrying == false)
        {
            if (other.TryGetComponent(out IPickable pickable))
            {
                pickable.PickUp(_pickUpPosition);
                _pickedUpObject = pickable;
                IsCarrying = true;
                ResourceTaken?.Invoke(this);
            }
        }
    }

    private void CheckForPickedResource()
    {
        _mover.ResetTarget();
        CurrentState = States.Idle;
        ResourceNotFound?.Invoke(this);
    }

    public void SetNewTarget(Transform targetPosition)
    {
        _mover.SetTarget(targetPosition);
        CurrentState = States.Working;
    }

    public IPickable DropObject()
    {
        IPickable temp = _pickedUpObject;
        _pickedUpObject = null;
        _mover.ResetTarget();
        IsCarrying = false;
        CurrentState = States.Idle;
        ResourceDelivered?.Invoke(this);
        return temp;
    }
}
