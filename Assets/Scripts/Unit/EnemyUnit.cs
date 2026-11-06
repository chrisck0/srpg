using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitStat))]
public class EnemyUnit : UnitBase
{
    [SerializeField] private InputActionReference _enemyAction;

    public event Action OnEnemyActioned;

    private void Action(InputAction.CallbackContext context)
    {
        OnEnemyActioned?.Invoke();
    }

    public void BindInputEvents()
    {
        _enemyAction.action.Enable();
        _enemyAction.action.performed += Action;
    }

    public void UnbindInputEvents()
    {
        _enemyAction.action.Disable();
        _enemyAction.action.performed -= Action;
    }
}
