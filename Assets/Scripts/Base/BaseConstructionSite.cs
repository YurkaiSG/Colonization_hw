using System;
using UnityEngine;
using UnityEngine.AI;

public class BaseConstructionSite : MonoBehaviour
{
    [SerializeField] private Base _basePrefab;
    [SerializeField] private Collider _collider;
    [SerializeField] private NavMeshObstacle _navMeshObstacle;

    private bool _canPlace = true;
    private bool _canFinishBuilding = false;
    private int _collisionsAmount = 0;
    public Unit _builder;
    private Base _buildedBase;

    public Mode CurrentMode { get; private set; }
    public event Action<bool> SelectionPreviewChanged;
    public event Action BuildingPlaced;

    public enum Mode
    {
        Preview,
        Fixed
    }

    private void Awake()
    {
        CurrentMode = Mode.Preview;
        ChangeCollisionsState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CurrentMode == Mode.Preview)
        {
            _collisionsAmount++;
            _canPlace = false;
            SelectionPreviewChanged?.Invoke(false);
        }
        else if (CurrentMode == Mode.Fixed)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                if (unit == _builder)
                {
                    _canFinishBuilding = true;
                    MakeAttemptToBuild();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CurrentMode == Mode.Preview)
        {
            _collisionsAmount--;

            if (_collisionsAmount == 0)
            {
                _canPlace = true;
                SelectionPreviewChanged?.Invoke(true);
            }
        }
    }

    private void ChangeCollisionsState(bool state)
    {
        _collider.enabled = state;
        _navMeshObstacle.enabled = state;
    }

    private void MakeAttemptToBuild()
    {
        if (_canFinishBuilding && _builder != null)
            BuildBase();
    }

    private void BuildBase()
    {
        _buildedBase = Instantiate(_basePrefab, transform.position, transform.rotation);
        _buildedBase.AddBuilderToBase(_builder);
        Destroy(gameObject);
    }

    public void PlaceConstructionSite()
    {
        ChangeCollisionsState(true);
        CurrentMode = Mode.Fixed;
        BuildingPlaced?.Invoke();
    }

    public void SetBuilder(Unit unit)
    {
        _builder = unit;
        MakeAttemptToBuild();
    }

    public void ResetCollisionCounter()
    {
        _collisionsAmount = 0;
        _canPlace = true;
        SelectionPreviewChanged?.Invoke(true);
    }

    public bool TryPlace() => _canPlace;
}
