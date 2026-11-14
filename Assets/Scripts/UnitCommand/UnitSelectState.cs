using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public sealed class UnitSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _infoAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private UnitBase _prevUnit;
    private bool _isInfoOpen;
    private List<EnemyUnit> _showingDangerEnemyList = new();

    public UnitSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference infoAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground)
    {
        _controller = controller;
        _selectAction = selectAction;
        _infoAction = infoAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
    }

    public override void Enter()
    {
        BindInputActions();
    }

    public override void Update()
    {
        if (_isInfoOpen) return;

        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            _currentTile = null;
            _controller.HideCursor();
            _controller.HideTileUI();
            HideDistanceRange();
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;
        _currentTile = tile;
        _controller.ShowCursor(tile.transform.position.WorldToCoord());
        _controller.ShowTileUI(tile);
        ShowDistanceRange(tile);
    }

    public override void Exit()
    {
        _currentTile = null;
        HideDistanceRange();
        UnbindInputActions();
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null || _isInfoOpen) return;

        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null) return;

        switch (unit.UnitType)
        {
            case UnitType.Player:
                PlayerUnit playerUnit = (unit as PlayerUnit);

                if (playerUnit == null || !playerUnit.CanAction) return;

                _controller.SetSelectedPlayerUnit(playerUnit);
                playerUnit.Select();
                _controller.SetState(_controller.UnitMoveState);
                break;

            case UnitType.Enemy:
                EnemyUnit enemyUnit = (unit as EnemyUnit);

                if (enemyUnit == null) return;

                if (enemyUnit.EnemyAI.IsShowingDanger)
                {
                    enemyUnit.EnemyAI.HideDanger();
                    _controller.ShowingDangerEnemyList.Remove(enemyUnit);
                }

                else
                {
                    enemyUnit.EnemyAI.ShowDanger();
                    _controller.ShowingDangerEnemyList.Add(enemyUnit);
                }
                break;
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        HideDetailedInfo();
    }

    private void ShowDistanceRange(TileBase tile)
    {
        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == _prevUnit) return;

        HideDistanceRange();

        if (unit == null
            || unit.UnitType == UnitType.Player && !(unit as PlayerUnit).CanAction) return;

        _prevUnit = unit;

        unit.DistanceRange.ShowTargetable();
        unit.DistanceRange.ShowReachable();
    }

    private void HideDistanceRange()
    {
        if (_prevUnit == null) return;

        _prevUnit.DistanceRange.HideTargetable();
        _prevUnit.DistanceRange.HideReachable();
        _prevUnit = null;
    }

    private void ShowDetailedInfo(InputAction.CallbackContext context)
    {
        if (_currentTile == null) return;

        if (_isInfoOpen) HideDetailedInfo();
        else
        {
            Vector2Int coord = _currentTile.transform.position.WorldToCoord();

            UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

            if (unit == null) return;

            _controller.HideTileUI();
            _controller.ShowUnitDetailedInfo(_currentTile);
            _isInfoOpen = true;
        }
    }

    private void HideDetailedInfo()
    {
        _controller.HideUnitDetailedInfo();
        _controller.ShowTileUI(_currentTile);
        _isInfoOpen = false;
    }

    private void BindInputActions()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _infoAction.action.Enable();
        _infoAction.action.performed += ShowDetailedInfo;
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _infoAction.action.Disable();
        _infoAction.action.performed -= ShowDetailedInfo;
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
        _cursorPosition.action.Disable();
    }
}
