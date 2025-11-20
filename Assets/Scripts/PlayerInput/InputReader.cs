using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private PlayerInput _playerInput;

    public event Action<Vector2> Moved;
    public event Action<float> Zoomed;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Player.Move.performed += OnMove;
        _playerInput.Player.Zoom.performed += OnZoom;
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.action.ReadValue<Vector2>();
        Moved?.Invoke(direction);
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float direction = context.action.ReadValue<float>();
        Zoomed?.Invoke(direction);
    }
}
