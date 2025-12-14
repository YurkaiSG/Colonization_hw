using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaScannerData), typeof(BaseOperations))]
public class Base : MonoBehaviour
{
    [SerializeField] private Transform _returnPoint;
    [SerializeField] private List<Unit> _controlledUnits = new List<Unit>();

    private AreaScannerData _scannerData;
    private BaseOperations _baseOperations;
    private bool _isConstructionInProgress = false;
    private Transform _constructionPosition;

    public int ControlledUnitsCount => _controlledUnits.Count;

    public event Action<Resource> ResourceAdded;

    private void Awake()
    {
        _scannerData = GetComponent<AreaScannerData>();
        _baseOperations = GetComponent<BaseOperations>();
    }

    private void OnEnable()
    {
        _scannerData.ScanPerformed += CheckForIdleWorkers;
        _baseOperations.UnitCreated += AssignNewUnitToBase;
        _baseOperations.ConstructionReady += EnableConstructionStatus;
    }

    private void OnDisable()
    {
        _scannerData.ScanPerformed -= CheckForIdleWorkers;
        _baseOperations.UnitCreated -= AssignNewUnitToBase;
        _baseOperations.ConstructionReady -= EnableConstructionStatus;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
            if (_controlledUnits.Contains(unit) && unit.IsCarrying)
                if (unit._pickedUpObject is Resource)
                    ResourceAdded?.Invoke(unit.DropObject() as Resource);
    }

    private void AssignNewUnitToBase(Unit unit)
    {
        _controlledUnits.Add(unit);
        unit.ResourceTaken += SendUnitToBase;
        unit.ResourceDelivered += SelectNextWork;
        unit.ResourceNotFound += SendUnitToResource;
    }

    private void SelectNextWork(Unit unit)
    {
        if (_isConstructionInProgress)
            SendUnitToNewBase(unit);
        else
            SendUnitToResource(unit);
    }

    private void SendUnitToResource(Unit unit)
    {
        if (_scannerData.ScannedAmount == 0)
            return;

        Transform resource = _scannerData.GetResourcePosition();

        if (resource != null)
            unit.SetNewTarget(resource);
    }

    private void SendUnitToBase(Unit unit)
    {
        unit.SetNewTarget(_returnPoint);
    }

    private void EnableConstructionStatus(Transform targetPosition)
    {
        _constructionPosition = targetPosition;
        _isConstructionInProgress = true;
    }

    private void SendUnitToNewBase(Unit unit)
    {
        unit.ResourceTaken -= SendUnitToBase;
        unit.ResourceDelivered -= SelectNextWork;
        unit.ResourceNotFound -= SendUnitToResource;
        unit.SetNewTarget(_constructionPosition);
        unit.CurrentState = Unit.States.Colonizing;
        _controlledUnits.Remove(unit);
        _baseOperations.EndConstruction(unit);
        _isConstructionInProgress = false;
    }

    private void CheckForIdleWorkers()
    {
        foreach (Unit unit in _controlledUnits)
        {
            if (unit.CurrentState == Unit.States.Idle)
            {
                if (_isConstructionInProgress)
                {
                    SendUnitToNewBase(unit);
                    return;
                }
                else
                {
                    SendUnitToResource(unit);
                }
            }
        }
    }

    public void AddBuilderToBase(Unit unit)
    {
        AssignNewUnitToBase(unit);
        unit.CurrentState = Unit.States.Idle;
    }
}
