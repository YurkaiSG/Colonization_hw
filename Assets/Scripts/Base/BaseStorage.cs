using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseStorage : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private Transform _storageLocation;
    [SerializeField] private Vector3 _storageSize = new Vector3(4, 5, 10);
    [SerializeField] private Vector3 _internalOffset = new Vector3(1f, 1f, 1f);

    private List<Resource> _resources;
    public int ResourceAmount => _resources.Count;
    public event Action AmountChanged;

    private void Awake()
    {
        _resources = new List<Resource>();
    }

    private void OnEnable()
    {
        _base.ResourceAdded += StoreResource;
    }

    private void OnDisable()
    {
        _base.ResourceAdded -= StoreResource;
    }

    private void StoreResource(Resource resource)
    {
        float xPos = ResourceAmount % (int)_storageSize.x * _internalOffset.x;
        float yPos = ResourceAmount / (int)(_storageSize.x * _storageSize.z) * _internalOffset.y;
        float zPos = ResourceAmount / (int)_storageSize.x % _storageSize.z * _internalOffset.z;
        Vector3 newPosition = new Vector3(_storageLocation.position.x + xPos, _storageLocation.position.y + yPos, _storageLocation.position.z + zPos);
        resource.transform.SetPositionAndRotation(newPosition, _storageLocation.rotation);
        resource.transform.SetParent(transform);
        _resources.Add(resource);
        AmountChanged?.Invoke();
    }

    public void SpendResources(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Resource temp = _resources[ResourceAmount - 1];
            _resources.RemoveAt(ResourceAmount - 1);
            Destroy(temp.gameObject);
        }
    }
}
