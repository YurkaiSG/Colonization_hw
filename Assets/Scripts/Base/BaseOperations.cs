using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BaseColonizator), typeof(Base))]
public class BaseOperations : MonoBehaviour
{
    [SerializeField] private BaseStorage _storage;
    [SerializeField] private int _unitCost = 3;
    [SerializeField] private int _baseCost = 5;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private bool _isStartingBase = false;
    [SerializeField] private int _startingUnitsAmount = 3;
    [SerializeField] private int _minUnitsForColonization = 2;

    private BaseColonizator _colonizator;
    private Base _base;
    private float _unitSpawnDelay = 0.5f;
    private Transform _constructionSitePosition;
    private bool _isConstructionInProgress = false;

    public State CurrentState { get; private set; }

    public event Action<Unit> UnitCreated;
    public event Action<Transform> ConstructionReady;
    public bool _isReadyForConstruction = false;

    public enum State
    {
        Normal,
        Colonization
    }

    private void Awake()
    {
        _colonizator = GetComponent<BaseColonizator>();
        _base = GetComponent<Base>();
        CurrentState = State.Normal;

        if (_isStartingBase)
            StartCoroutine(SpawnStartingUnits());
    }

    private void OnEnable()
    {
        _storage.AmountChanged += PerformStateOperation;
        _colonizator.ConstructionSitePlaced += BeginConstruction;
        _colonizator.ConstructionSiteMoved += ChangeConstructionPosition;
    }

    private void OnDisable()
    {
        _storage.AmountChanged -= PerformStateOperation;
        _colonizator.ConstructionSitePlaced -= BeginConstruction;
        _colonizator.ConstructionSiteMoved -= ChangeConstructionPosition;
    }

    private void Update()
    {
        if (_isReadyForConstruction && _isConstructionInProgress == false)
            AttemptToColonize();
    }

    private void PerformStateOperation()
    {
        switch (CurrentState)
        {
            case State.Normal:
                if (CanAfford(_unitCost)) ExecuteNormalBehaviour();
                break;
            case State.Colonization:
                if (CanAfford(_baseCost) && _isConstructionInProgress == false) ExecuteColonizationBehaviour();
                break;
            default:
                break;
        }
    }

    private void BeginConstruction(Transform position)
    {
        _isReadyForConstruction = true;
        _constructionSitePosition = position;
        AttemptToColonize();
    }

    private void AttemptToColonize()
    {
        if (_base.ControlledUnitsCount >= _minUnitsForColonization)
        {
            CurrentState = State.Colonization;
            PerformStateOperation();
        }
    }

    private void ChangeConstructionPosition(Transform position)
    {
        _constructionSitePosition = position;
        _isConstructionInProgress = true;
        ConstructionReady?.Invoke(_constructionSitePosition);
    }

    private void ExecuteNormalBehaviour()
    {
        _storage.SpendResources(_unitCost);
        SpawnUnit();
    }

    private void ExecuteColonizationBehaviour()
    {
        _storage.SpendResources(_baseCost);
        _isConstructionInProgress = true;
        ConstructionReady?.Invoke(_constructionSitePosition);
    }

    private void SpawnUnit()
    {
        Unit unit = Instantiate(_unitPrefab, _unitSpawnPoint.position, _unitSpawnPoint.rotation);
        UnitCreated?.Invoke(unit);
    }

    private bool CanAfford(int cost) => _storage.ResourceAmount >= cost;

    private IEnumerator SpawnStartingUnits()
    {
        WaitForSeconds wait = new WaitForSeconds(_unitSpawnDelay);

        for (int i = 0; i < _startingUnitsAmount; i++)
        {
            yield return wait;
            SpawnUnit();
        }
    }

    public void EndConstruction(Unit unit)
    {
        CurrentState = State.Normal;
        _isConstructionInProgress = false;
        _isReadyForConstruction = false;
        _colonizator.TransferUnitToNewBase(unit);
    }
}
