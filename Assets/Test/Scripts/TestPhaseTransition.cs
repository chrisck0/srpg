using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPhaseTransition : MonoBehaviour
{
    [SerializeField] private InputActionReference _keyboard1Action;
    [SerializeField] private InputActionReference _keyboard2Action;

    public event Action OnPlayerPhaseEnd;
    public event Action OnEnemyPhaseEnd;

    private void OnEnable() => BindInputActions();
    private void OnDisable() => UnbindInputActions();

    private void PlayerPhaseEnd(InputAction.CallbackContext context)
    {
        OnPlayerPhaseEnd?.Invoke();
    }

    private void EnemyPhaseEnd(InputAction.CallbackContext context)
    {
        OnEnemyPhaseEnd?.Invoke();
    }

    private void BindInputActions()
    {
        _keyboard1Action.action.Enable();
        _keyboard1Action.action.performed += PlayerPhaseEnd;
        _keyboard2Action.action.Enable();
        _keyboard2Action.action.performed += EnemyPhaseEnd;
    }

    private void UnbindInputActions()
    {
        _keyboard1Action.action.Disable();
        _keyboard1Action.action.performed -= PlayerPhaseEnd;
        _keyboard2Action.action.Disable();
        _keyboard2Action.action.performed -= EnemyPhaseEnd;
    }
}
