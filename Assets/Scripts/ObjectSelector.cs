using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputReader))]
public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private ISelectable _selectedObject = null;
    private ISelectableAction _selectedObjectAction = null;
    private InputReader _reader;
    private Ray _ray;
    private RaycastHit _hit;
    private float _rayMaxDistance = 1000.0f;

    private void Awake()
    {
        _reader = GetComponent<InputReader>();
    }

    private void OnEnable()
    {
        _reader.LeftMouseButtonClicked += ProcessLeftButtonClick;
        _reader.RightMouseButtonClicked += ProcessRightButtonClick;
    }

    private void OnDisable()
    {
        _reader.LeftMouseButtonClicked -= ProcessLeftButtonClick;
        _reader.RightMouseButtonClicked -= ProcessRightButtonClick;
    }

    private void ProcessLeftButtonClick()
    {
        if (TryRayCastTarget())
        {
            if (_hit.collider.TryGetComponent(out ISelectable clickedObject))
            {
                SelectObject(clickedObject);

                if (_hit.collider.TryGetComponent(out ISelectableAction clickedObjectAction))
                {
                    _selectedObjectAction = clickedObjectAction;
                }
            }
            else if (_selectedObject != null)
            {
                Deselect();
            }
        }
        else if (_selectedObject != null)
        {
            Deselect();
        }
    }

    private void SelectObject(ISelectable clickedObject)
    {
        if (_selectedObject != null)
            _selectedObject.Deselect();

        _selectedObject = clickedObject;
        _selectedObject.Select();
    }

    private void Deselect()
    {
        _selectedObject.Deselect();
        _selectedObject = null;
        _selectedObjectAction = null;
    }

    private bool TryRayCastTarget()
    {
        _ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        return Physics.Raycast(_ray, out _hit, _rayMaxDistance);
    }

    private void ProcessRightButtonClick()
    {
        if (_selectedObject != null && _selectedObjectAction != null)
            if (TryRayCastTarget())
                _selectedObjectAction.ExecuteSelectedObjectAction();
    }
}
