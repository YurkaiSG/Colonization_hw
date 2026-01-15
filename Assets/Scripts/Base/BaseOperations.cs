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

    private BaseColonizator _colonizator;
    private Base _base;
    private float _unitSpawnDelay = 0.5f;
    private Transform _constructionSitePosition;
    private bool _isConstructionInProgress = false;

    public State CurrentState { get; private set; }

    public event Action<Unit> UnitCreated;
    public event Action<Transform> ColonizationReady;
    public event Action<Transform> ColonizationPositionChanged;
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
        _colonizator.ConstructionSitePlaced += BeginColonization;
        _colonizator.ConstructionSiteMoved += ChangeConstructionPosition;
        _base.ColonizationFinished += EndConstruction;
    }

    private void OnDisable()
    {
        _storage.AmountChanged -= PerformStateOperation;
        _colonizator.ConstructionSitePlaced -= BeginColonization;
        _colonizator.ConstructionSiteMoved -= ChangeConstructionPosition;
        _base.ColonizationFinished -= EndConstruction;
    }

    private void Update()
    {
        if (_isReadyForConstruction && _isConstructionInProgress == false)
            PerformStateOperation();
    }

    private void PerformStateOperation()
    {
        switch (CurrentState)
        {
            case State.Normal:
                ExecuteNormalBehaviour();
                break;
            case State.Colonization:
                ExecuteColonizationBehaviour();
                break;
            default:
                break;
        }
    }

    private void BeginColonization(Transform position)
    {
        _isReadyForConstruction = true;
        _constructionSitePosition = position;
        CurrentState = State.Colonization;
        PerformStateOperation();
    }

    private void ChangeConstructionPosition(Transform position)
    {
        _constructionSitePosition = position;
        ColonizationPositionChanged?.Invoke(_constructionSitePosition);
    }

    private void ExecuteNormalBehaviour()
    {
        if (CanAfford(_unitCost))
        {
            _storage.SpendResources(_unitCost);
            SpawnUnit();
        }
    }

    private void ExecuteColonizationBehaviour()
    {
        if (CanAfford(_baseCost) && _isConstructionInProgress == false)
        {
            _storage.SpendResources(_baseCost);
            _isConstructionInProgress = true;
            ColonizationReady?.Invoke(_constructionSitePosition);
        }
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

    private void EndConstruction(Unit unit)
    {
        CurrentState = State.Normal;
        _isConstructionInProgress = false;
        _isReadyForConstruction = false;
    }
}
