using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Base))]
public class BaseColonizator : MonoBehaviour, ISelectable, ISelectableAction
{
    [SerializeField] private BaseStorage _storage;
    [SerializeField] private BaseConstructionSite _constructionSitePrefab;
    [SerializeField] private int _minUnitsForColonization = 2;
    [SerializeField] private LayerMask _groundLayerMask;

    private BaseConstructionSite _preview;
    private BaseConstructionSite _placedConstructionSite;
    private Base _base;
    private Camera _camera;
    private Ray _ray;
    private RaycastHit _hit;
    private float _rayMaxDistance = 1000.0f;

    public event Action<bool> SelectionChanged;
    public event Action<Transform> ConstructionSitePlaced;
    public event Action<Transform> ConstructionSiteMoved;

    public bool IsSelected { get; private set; }
    public bool IsConstructing { get; private set; }

    private void Awake()
    {
        IsSelected = false;
        IsConstructing = false;
        _camera = Camera.main;
        _base = GetComponent<Base>();
    }

    private void OnEnable()
    {
        _base.ColonizationFinished += TransferUnitToNewBase;
    }

    private void OnDisable()
    {
        _base.ColonizationFinished -= TransferUnitToNewBase;
    }

    private void Update()
    {
        if (IsSelected)
            MovePreviewToMousePosition();
    }

    private void MovePreviewToMousePosition()
    {
        _ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(_ray, out _hit, _rayMaxDistance, _groundLayerMask))
            _preview.transform.position = _hit.point;
    }

    private void TransferUnitToNewBase(Unit unit)
    {
        _placedConstructionSite.SetBuilder(unit);
        _placedConstructionSite = null;
        IsConstructing = false;
    }

    public void Select()
    {
        SelectionChanged?.Invoke(true);
        IsSelected = true;

        if (_preview == null)
        {
            _preview = Instantiate(_constructionSitePrefab);
        }
        else
        {
            _preview.gameObject.SetActive(true);
            _preview.ResetCollisionCounter();
        }
    }

    public void Deselect()
    {
        SelectionChanged?.Invoke(false);
        _preview?.gameObject.SetActive(false);
        IsSelected = false;
    }

    public void ExecuteSelectedObjectAction()
    {
        MovePreviewToMousePosition();

        if (_preview.TryPlace())
        {
            if (_placedConstructionSite == null)
            {
                if (_base.ControlledUnitsCount >= _minUnitsForColonization)
                {
                    IsConstructing = true;
                    _placedConstructionSite = Instantiate(_constructionSitePrefab, _hit.point, _preview.transform.rotation);
                    _placedConstructionSite.PlaceConstructionSite();
                    ConstructionSitePlaced?.Invoke(_placedConstructionSite.transform);
                }
            }
            else
            {
                IsConstructing = true;
                _placedConstructionSite.transform.position = _hit.point;
                _placedConstructionSite.PlaceConstructionSite();
                ConstructionSiteMoved?.Invoke(_placedConstructionSite.transform);
            }
        }
    }
}
