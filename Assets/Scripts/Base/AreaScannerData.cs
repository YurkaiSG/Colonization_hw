using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaScanner))]
public class AreaScannerData : MonoBehaviour
{
    private AreaScanner _scanner;
    private List<Collider> FindedResources;

    public int ScannedAmount => FindedResources.Count;
    public event Action ScanPerformed;

    private void Awake()
    {
        _scanner = GetComponent<AreaScanner>();
        FindedResources = new List<Collider>();
    }

    private void OnEnable()
    {
        _scanner.Scanned += StoreScannedInfo;
    }

    private void OnDisable()
    {
        _scanner.Scanned -= StoreScannedInfo;
    }

    private void StoreScannedInfo(List<Collider> colliders)
    {
        FindedResources = colliders;
        SortByDistance();
        ScanPerformed?.Invoke();
    }

    private void SortByDistance()
    {
        FindedResources.Sort((x, y) => {
            return Vector3.Distance(transform.position, x.transform.position)
            .CompareTo(Vector3.Distance(transform.position, y.transform.position));
        });
    }

    public Transform GetResourcePosition()
    {
        while (FindedResources[0] == null && ScannedAmount != 0)
            FindedResources.RemoveAt(0);

        if (ScannedAmount != 0)
        {
            Transform resource = FindedResources[0].transform;
            FindedResources.RemoveAt(0);
            return resource;
        }
        else
        {
            return null;
        }
    }
}
