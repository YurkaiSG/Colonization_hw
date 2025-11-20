using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaScanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private Transform _returnPoint;
    [SerializeField] private List<Unit> _controlledUnits = new List<Unit>();

    private AreaScanner _scanner;

    public event Action<Resource> ResourceAdded;

    private void Awake()
    {
        _scanner = GetComponent<AreaScanner>();

        for (int i = 0; i < _controlledUnits.Count; i++)
        {
            _controlledUnits[i].ResourceTaken += SendUnitToBase;
            _controlledUnits[i].ResourceDelivered += SendUnitToResource;
            _controlledUnits[i].ResourceNotFound += SendUnitToResource;
        }
    }

    private void OnEnable()
    {
        _scanner.ScanPerformed += CheckForIdleWorkers;
    }

    private void OnDisable()
    {
        _scanner.ScanPerformed -= CheckForIdleWorkers;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
            if (_controlledUnits.Contains(unit) && unit.IsCarrying)
                if (unit._pickedUpObject is Resource)
                    ResourceAdded?.Invoke(unit.DropObject() as Resource);
    }

    private void SendUnitToResource(Unit unit)
    {
        if (_scanner.ScannedAmount == 0)
            return;

        unit.SetNewTarget(_scanner.GetResourcePosition());
    }

    private void SendUnitToBase(Unit unit)
    {
        unit.SetNewTarget(_returnPoint);
    }

    private void CheckForIdleWorkers()
    {
        foreach (Unit unit in _controlledUnits)
            if (unit.CurrentState == Unit.States.Idle)
                SendUnitToResource(unit);
    }
}
