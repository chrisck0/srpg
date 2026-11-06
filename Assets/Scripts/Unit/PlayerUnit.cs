using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnit : UnitBase
{
    [SerializeField] private InputActionReference _playerAction;

    public event Action OnPlayerActioned;

    private void Action(InputAction.CallbackContext context)
    {
        Debug.Log($"{name} Actioned");
        OnPlayerActioned?.Invoke();
    }

    public void BindInputEvents()
    {
        _playerAction.action.Enable();
        _playerAction.action.performed += Action;
    }

    public void UnbindInputEvents()
    {
        _playerAction.action.Disable();
        _playerAction.action.performed -= Action;
    }
}
