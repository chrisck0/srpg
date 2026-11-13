using UnityEngine;
using UnityEngine.InputSystem;

public sealed class UnitCommandState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _cancelAction;
    private CommandUIController _commandUIController;
    private UnitCommandUI _unitCommandUI;
    private AttackCommandUI _attackCommandUI;
    private bool _isAttackSelected;

    public UnitCommandState(UnitCommandStateController controller,
        InputActionReference cancelAction,
        CommandUIController commandUIController)
    {
        _controller = controller;
        _cancelAction = cancelAction;
        _commandUIController = commandUIController;
        _unitCommandUI = commandUIController.UnitCommandUI;
        _attackCommandUI = commandUIController.AttackCommandUI;
    }

    public override void Enter()
    {
        BindInputActions();
        BindCommandEvents();
        _unitCommandUI.Show();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        UnbindInputActions();
        UnbindCommandEvents();
        _unitCommandUI.Hide();
        _attackCommandUI.Hide();
        _controller.HideCursor();
        _isAttackSelected = false;
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (_isAttackSelected)
        {
            _commandUIController.AttackCommandUI.Hide();
            _isAttackSelected = false;
            return;
        }

        PlayerUnit playerUnit = _controller.SelectedPlayerUnit;
        MoveUnit(playerUnit,
            playerUnit.transform.position.WorldToCoord(),
            _controller.PreviousPlayerCoord);

        _controller.SetState(_controller.UnitSelectState);
    }

    private void MoveUnit(PlayerUnit playerUnit, Vector2Int prevCoord, Vector2Int nextCoord)
    {
        playerUnit.transform.position = nextCoord.CoordToWorld();
        _mapManager.UnitMap[prevCoord.x, prevCoord.y] = null;
        _mapManager.UnitMap[nextCoord.x, nextCoord.y] = playerUnit;

        _controller.SetState(_controller.UnitCommandState);
    }

    private void Attack()
    {
        _attackCommandUI.InitWeaponInfo(_controller.SelectedPlayerUnit);
        _attackCommandUI.Show();
        _isAttackSelected = true;
    }

    private void Wait()
    {
        _controller.SetState(_controller.CommandEndState);
        _controller.SelectedPlayerUnit?.Action();
    }

    private void WeaponSelected(WeaponData weaponData)
    {
        if (_controller.SelectedPlayerUnit.UnitStat.Energy < weaponData.EnergyCost) return;
        _controller.SetSelectedWeaponData(weaponData);
        _controller.SetState(_controller.TargetSelectState);
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

    private void BindCommandEvents()
    {
        _unitCommandUI.OnAttack += Attack;
        _unitCommandUI.OnWait += Wait;
        _attackCommandUI.OnWeaponSelected += WeaponSelected;
    }

    private void UnbindCommandEvents()
    {
        _unitCommandUI.OnAttack -= Attack;
        _unitCommandUI.OnWait -= Wait;
        _attackCommandUI.OnWeaponSelected -= WeaponSelected;
    }
}
