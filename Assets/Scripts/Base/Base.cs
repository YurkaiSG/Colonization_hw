using System;
using System.Collections.Generic;
using TMPro;
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
    public event Action<Unit> ColonizationFinished;

    private void Awake()
    {
        _scannerData = GetComponent<AreaScannerData>();
        _baseOperations = GetComponent<BaseOperations>();
    }

    private void OnEnable()
    {
        _scannerData.ScanPerformed += CheckForIdleWorkers;
        _baseOperations.UnitCreated += AssignUnitToBase;
        _baseOperations.ColonizationReady += BeginColonization;
        _baseOperations.ColonizationPositionChanged += ChangeColonizationPosition;
    }

    private void OnDisable()
    {
        _scannerData.ScanPerformed -= CheckForIdleWorkers;
        _baseOperations.UnitCreated -= AssignUnitToBase;
        _baseOperations.ColonizationReady -= BeginColonization;
        _baseOperations.ColonizationPositionChanged -= ChangeColonizationPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Unit unit))
            if (_controlledUnits.Contains(unit) && unit.IsCarrying)
                if (unit._pickedUpObject is Resource)
                    ResourceAdded?.Invoke(unit.DropObject() as Resource);
    }

    private void AssignUnitToBase(Unit unit)
    {
        _controlledUnits.Add(unit);
        unit.ResourceTaken += ReturnUnitToBase;
        unit.ResourceDelivered += SelectNextWork;
        unit.ResourceNotFound += SendUnitToResource;
    }

    private void SelectNextWork(Unit unit)
    {
        if (_isConstructionInProgress)
            SendBuilder(unit);
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

    private void ReturnUnitToBase(Unit unit)
    {
        unit.SetNewTarget(_returnPoint);
    }

    private void BeginColonization(Transform targetPosition)
    {
        ChangeColonizationPosition(targetPosition);
        _isConstructionInProgress = true;
    }

    private void ChangeColonizationPosition(Transform targetPosition)
    {
        _constructionPosition = targetPosition;
    }

    private void SendBuilder(Unit unit)
    {
        unit.ResourceTaken -= ReturnUnitToBase;
        unit.ResourceDelivered -= SelectNextWork;
        unit.ResourceNotFound -= SendUnitToResource;
        unit.SetNewTarget(_constructionPosition);
        unit.CurrentState = Unit.States.Colonizing;
        _controlledUnits.Remove(unit);
        ColonizationFinished?.Invoke(unit);
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
                    SendBuilder(unit);
                    return;
                }
                else
                {
                    SendUnitToResource(unit);
                }
            }
        }
    }

    public void TransferBuilderToBase(Unit unit)
    {
        AssignUnitToBase(unit);
        unit.CurrentState = Unit.States.Idle;
    }
}
