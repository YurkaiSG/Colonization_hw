using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(UnitMover), typeof(UnitCollisionHandler))]
public class Unit : MonoBehaviour
{
    private UnitMover _mover;
    private UnitCollisionHandler _collisionHandler;
    private int _unitMoveDistanceAfterSpawn = 5;

    public IPickable _pickedUpObject;
    
    public States CurrentState { get; set; }
    public bool IsCarrying => _collisionHandler.IsCarrying;

    public event Action<Unit> ResourceTaken;
    public event Action<Unit> ResourceDelivered;
    public event Action<Unit> ResourceNotFound;

    public enum States
    {
        Idle,
        Working,
        Colonizing,
        Spawned
    }

    private void Awake()
    { 
        _mover = GetComponent<UnitMover>();
        _collisionHandler = GetComponent<UnitCollisionHandler>();
    }

    private void Start()
    {
        CurrentState = States.Spawned;
        MoveAfterSpawn();
    }

    private void OnEnable()
    {
        _mover.Arrived += CheckForPickedResource;
        _collisionHandler.PickedUp += OnObjectPickUp;
    }

    private void OnDisable()
    {
        _mover.Arrived -= CheckForPickedResource;
        _collisionHandler.PickedUp -= OnObjectPickUp;
    }

    private void MoveAfterSpawn()
    {
        Vector3 newTargetPosition = transform.localPosition;
        newTargetPosition = newTargetPosition + transform.forward * _unitMoveDistanceAfterSpawn;
        _mover.SetTarget(newTargetPosition);
    }

    private void CheckForPickedResource()
    {
        _mover.ResetTarget();
        CurrentState = States.Idle;

        if (_pickedUpObject == null)            
            ResourceNotFound?.Invoke(this);
    }

    public void SetNewTarget(Transform targetPosition)
    {
        _mover.SetTarget(targetPosition.position);
        CurrentState = States.Working;
    }

    public void OnObjectPickUp(IPickable item)
    {
        _pickedUpObject = item;
        ResourceTaken?.Invoke(this);
    }

    public IPickable DropObject()
    {
        IPickable temp = _pickedUpObject;
        _pickedUpObject = null;
        _mover.ResetTarget();
        _collisionHandler.ResetCarryingStatus();
        CurrentState = States.Idle;
        ResourceDelivered?.Invoke(this);
        return temp;
    }
}
