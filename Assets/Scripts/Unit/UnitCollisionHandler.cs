using System;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Unit))]
public class UnitCollisionHandler : MonoBehaviour
{
    [SerializeField] private Transform _pickUpPosition;

    public bool IsCarrying { get; private set; }
    public event Action<IPickable> PickedUp;

    private void Awake()
    {
        IsCarrying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsCarrying == false)
        {
            if (other.TryGetComponent(out IPickable pickable))
            {
                pickable.PickUp();
                other.transform.SetPositionAndRotation(_pickUpPosition.position, _pickUpPosition.rotation);
                other.transform.SetParent(_pickUpPosition);
                IsCarrying = true;
                PickedUp?.Invoke(pickable);
            }
        }
    }

    public void ResetCarryingStatus() => IsCarrying = false;
}
