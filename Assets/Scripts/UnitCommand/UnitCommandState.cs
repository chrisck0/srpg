using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cancelAction;
    
    public UnitCommandState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cancelAction)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cancelAction = cancelAction;
    }

    public override void Enter()
    {
        BindInputActions();
        Debug.Log("Unit Command");
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        UnbindInputActions();
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        PlayerUnit playerUnit = _controller.SelectedPlayerUnit;
        MoveUnit(playerUnit,
            playerUnit.transform.position.WorldToCoord(),
            _controller.PreviousPlayerCoord);
        _controller.SetState(_controller.UnitMoveState);
    }

    private void MoveUnit(PlayerUnit playerUnit, Vector2Int prevCoord, Vector2Int nextCoord)
    {
        playerUnit.transform.position = nextCoord.CoordToWorld();
        _mapManager.UnitMap[prevCoord.x, prevCoord.y] = null;
        _mapManager.UnitMap[nextCoord.x, nextCoord.y] = playerUnit;

        _controller.SetState(_controller.UnitCommandState);
    }

    private void BindInputActions()
    {
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
    }

    private void UnbindInputActions()
    {
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
    }
}
