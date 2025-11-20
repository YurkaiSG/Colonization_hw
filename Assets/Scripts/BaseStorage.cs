using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseStorage : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private Transform _storageLocation;
    [SerializeField] private Vector3 _storageSize = new Vector3(4, 5, 10);
    [SerializeField] private Vector3 _internalOffset = new Vector3(1f, 1f, 1f);

    private List<Resource> resources;
    public int ResourceAmount => resources.Count;

    private void Awake()
    {
        resources = new List<Resource>();
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
        float xPos = resources.Count % (int)_storageSize.x * _internalOffset.x;
        float yPos = resources.Count / (int)(_storageSize.x * _storageSize.z) * _internalOffset.y;
        float zPos = resources.Count / (int)_storageSize.x % _storageSize.z * _internalOffset.z;
        Vector3 newPosition = new Vector3(_storageLocation.position.x + xPos, _storageLocation.position.y + yPos, _storageLocation.position.z + zPos);
        resources.Add(resource);
        resource.transform.SetPositionAndRotation(newPosition, _storageLocation.rotation);
        resource.transform.SetParent(transform);
    }
}
