using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class UnitMoveState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _infoAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private PlayerUnit _selectedUnit;
    private TileBase _currentTile;
    private TileCost[,] _tileCostMap;
    private Stack<Vector2Int> _pathCoordStack = new();
    private bool _hasSelected;
    private bool _isInfoOpen;
    private Vector2Int _targetCoord;

    public UnitMoveState(UnitCommandStateController controller,
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
        ShowMoveRange();
    }

    public override void Update()
    {
        if (_isInfoOpen) return;

        if (_hasSelected) MoveUnit();
        else RaycastTiles();
    }

    public override void Exit()
    {
        UnbindInputActions();
        _controller.SelectedPlayerUnit.DistanceRange.HideDistance();
        HideUI();
    }

    private void ShowMoveRange()
    {
        _selectedUnit = _controller.SelectedPlayerUnit;
        Vector2Int currentCoord = _selectedUnit.transform.position.WorldToCoord();
        _tileCostMap = _mapManager.TileRange.UnitMoveRange(_selectedUnit, currentCoord);
        _selectedUnit.DistanceRange.ShowDistance(_mapManager.TileRange.TileCostMap);
    }

    private void RaycastTiles()
    {
        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            _currentTile = null;
            HideUI();
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;

        _currentTile = tile;
        Vector2Int coord = hit.transform.position.WorldToCoord();

        _controller.ShowTileInfo(tile);
        _controller.ShowUnitInfo(tile);
        _controller.ShowCursor(coord);
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_currentTile == null || _isInfoOpen || _hasSelected) return;
        Vector2Int currentCoord = _selectedUnit.transform.position.WorldToCoord();
        Vector2Int nextCoord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[nextCoord.x, nextCoord.y];

        if (unit != null && unit != _selectedUnit
            && unit.UnitType == UnitType.Player && (unit as PlayerUnit).CanAction)
        {
            PlayerUnit playerUnit = unit as PlayerUnit;

            _controller.SelectedPlayerUnit.DistanceRange.HideDistance();
            _controller.SetSelectedPlayerUnit(playerUnit);
            playerUnit.Select();
            ShowMoveRange();
        }
        else if (!_mapManager.TileMap[nextCoord.x, nextCoord.y].CanSelect)
        {
            _controller.SetState(_controller.UnitSelectState);
        }
        else
        {
            MoveToDestination(currentCoord, nextCoord);
            HideUI();
        }
    }

    private void Info(InputAction.CallbackContext context)
    {
        if (_currentTile == null || _isInfoOpen || _hasSelected) return;

        Vector2Int coord = _currentTile.transform.position.WorldToCoord();

        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null) return;

        _controller.HideTileUI();
        _controller.ShowUnitDetailedInfo(_currentTile);
        _isInfoOpen = true;
    }

    private void MoveToDestination(Vector2Int currentCoord, Vector2Int destinationCoord)
    {
        _controller.SelectedPlayerUnit.ToggleMove();

        _hasSelected = true;

        _mapManager.UnitMap[currentCoord.x, currentCoord.y] = null;

        Vector2Int pathCoord = destinationCoord;
        while (pathCoord != currentCoord)
        {
            _pathCoordStack.Push(pathCoord);

            pathCoord = _tileCostMap[pathCoord.x, pathCoord.y].PrevTileCost.Coord;
        }

        _targetCoord = currentCoord;
        _controller.HideCursor();
    }

    private void MoveUnit()
    {
        if (Vector3.Distance(_selectedUnit.transform.position, _targetCoord.CoordToWorld()) >= 0.1f)
        {
            _selectedUnit.transform.Translate(Vector3.forward
                * Time.deltaTime * _selectedUnit.MoveSpeed);
            return;
        }

        if (_pathCoordStack.Count == 0)
        {
            _mapManager.UnitMap[_targetCoord.x, _targetCoord.y] = _selectedUnit;
            _hasSelected = false;
            _controller.SelectedPlayerUnit.ToggleMove();
            _controller.SetState(_controller.UnitCommandState);
            return;
        }

        _targetCoord = _pathCoordStack.Pop();
        _selectedUnit.transform.LookAt(_targetCoord.CoordToWorld());
        _selectedUnit.transform.Translate(Vector3.forward *
            Time.deltaTime * _selectedUnit.MoveSpeed);
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        if (_hasSelected) return;

        if (_isInfoOpen)
        {
            _controller.HideUnitDetailedInfo();
            _controller.ShowTileUI(_currentTile);
            _isInfoOpen = false;
        }
        else
        {
            _controller.SelectedPlayerUnit.UnSelect();
            _controller.SetState(_controller.UnitSelectState);
        }
    }

    private void HideUI()
    {
        _controller.HideCursor();
        _controller.HideTileUI();
    }

    private void BindInputActions()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _infoAction.action.Enable();
        _infoAction.action.performed += Info;
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _infoAction.action.Disable();
        _infoAction.action.performed -= Info;
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
        _cursorPosition.action.Disable();
    }
}
