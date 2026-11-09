using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TargetSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private CommandUIController _commandUIController;
    private TileInfoUI _tileInfoUI;
    private UnitInfoUI _unitInfoUI;
    private List<Vector2Int> _localTargetCoords = new();
    private List<Vector2Int> _globalTargetCoords = new();
    private WeaponDirection _prevWeaponDirection = WeaponDirection.None;

    private bool _isWeaponSelectDirection => _controller.SelectedWeaponData.WeaponType == WeaponType.SelectDirection;

    public TargetSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground,
        CommandUIController commandUIController)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
        _commandUIController = commandUIController;
        _tileInfoUI = commandUIController.TileInfoUI;
        _unitInfoUI = commandUIController.UnitInfoUI;
    }

    public override void Enter()
    {
        BindInputActions();
        ShowSelectRange(_controller.SelectedWeaponData);
        Debug.Log("Target Select");
    }

    public override void Update()
    {
        Vector2 screen = _cursorPosition.action.ReadValue<Vector2>();
        Ray ray = Camera.main.ScreenPointToRay(screen);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, _ground))
        {
            _controller.HideCursor();
            _currentTile = null;
            _tileInfoUI.Hide();
            _unitInfoUI.Hide();
            return;
        }

        TileBase tile = hit.collider.gameObject.GetComponent<TileBase>();

        if (tile == null || tile == _currentTile) return;

        _currentTile = tile;

        ShowTileInfo(tile);
        ShowUnitInfo(tile);

        Vector2Int coord = hit.transform.position.WorldToCoord();
        Vector2Int playerCoord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();

        WeaponDirection weaponDirection = GetWeaponDirection(playerCoord, coord);

        if (_isWeaponSelectDirection && _prevWeaponDirection != weaponDirection)
        {
            HidePreviousSelectDirectionRange();
            ShowSelectDirectionRange(playerCoord, weaponDirection);
        }

        _controller.ShowCursor(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        _mapManager.TileRange.InitTiles();
        _controller.HideCursor();
        _prevWeaponDirection = WeaponDirection.None;
    }

    private void ShowTileInfo(TileBase tile)
    {
        _tileInfoUI.Show();
        TileData tileData = tile.TileData;
        _tileInfoUI.SetData(tileData.Name, tileData.Description);
    }

    private void ShowUnitInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _unitInfoUI.Hide();
            return;
        }

        _unitInfoUI.Show();
        UnitStat unitStat = unit.UnitStat;
        _unitInfoUI.SetData(unitStat.UnitData.Name,
            $"HP {unitStat.Health}/{unitStat.UnitData.Health}",
            $"EN {unitStat.Energy}/{unitStat.UnitData.Energy}",
            unit.Weapon.WeaponDataList[0].Name); // 임시로 첫번째 무기 등록
    }

    private void ShowSelectRange(WeaponData weaponData)
    {
        Vector2Int coord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                ShowSelectTargetRange(coord, weaponData.MinRange, weaponData.MaxRange);
                break;
            case WeaponType.SelectDirection:
                SetLocalTargetCoords(coord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
                break;
        }
    }

    private void ShowExplosionRange()
    {

    }

    private void ShowSelectTargetRange(Vector2Int coord, int minRange, int maxRange)
    {
        TileCost[,] tileCost = _mapManager.TileRange.SelectTargetWeaponRange(coord, minRange, maxRange);
    }

    private void SetLocalTargetCoords(Vector2Int coord, WeaponRangeShape weaponRangeShape, int explosionRange)
    {
        _localTargetCoords.Clear();

        switch(weaponRangeShape)
        {
            case WeaponRangeShape.Line:
                for (int i = 0; i < explosionRange; i++)
                {
                    _localTargetCoords.Add(new Vector2Int(0, i + 1));
                }
                break;

            case WeaponRangeShape.CircularSector:
                for (int i = 0; i < explosionRange; i++)
                {
                    for (int j = -i; j <= i; j++)
                    {
                        _localTargetCoords.Add(new Vector2Int(j, i + 1));
                    }
                }
                break;
        }
    }

    private WeaponDirection GetWeaponDirection(Vector2Int startCoord, Vector2Int targetCoord)
    {
        Vector2Int directionCoord = targetCoord - startCoord;
        if (Mathf.Abs(directionCoord.x) >= Mathf.Abs(directionCoord.y))
        {
            if (Mathf.Sign(directionCoord.x) == 1f)
            {
                return WeaponDirection.Right;
            }
            else
            {
                return WeaponDirection.Left;
            }
        }
        else
        {
            if (Mathf.Sign(directionCoord.y) == 1f)
            {
                return WeaponDirection.Up;
            }
            else
            {
                return WeaponDirection.Down;
            }
        }
    }

    private void ShowSelectDirectionRange(Vector2Int coord, WeaponDirection direction)
    {
        _globalTargetCoords.Clear();

        foreach (Vector2Int localCoord in _localTargetCoords)
        {
            Vector2Int rotatedLocalCoord = localCoord;

            switch(direction)
            {
                case WeaponDirection.Up:
                    rotatedLocalCoord = localCoord;
                    break;
                case WeaponDirection.Right:
                    rotatedLocalCoord = new Vector2Int(localCoord.y, -localCoord.x);
                    break;
                case WeaponDirection.Down:
                    rotatedLocalCoord = new Vector2Int(-localCoord.x, -localCoord.y);
                    break;
                case WeaponDirection.Left:
                    rotatedLocalCoord = new Vector2Int(-localCoord.y, localCoord.x);
                    break;
            }

            Vector2Int globalCoord = coord + rotatedLocalCoord;

            if (globalCoord.x < 0
                || globalCoord.y < 0
                || globalCoord.x >= _mapManager.TileMap.GetLength(0)
                || globalCoord.y >= _mapManager.TileMap.GetLength(1)) return;

            _mapManager.TileMap[globalCoord.x, globalCoord.y].ShowRange();
            _globalTargetCoords.Add(globalCoord);
        }
    }

    private void HidePreviousSelectDirectionRange()
    {
        foreach (Vector2Int coord in _globalTargetCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].HideRange();
        }
    }

    private void ShowMultiTargetRange(Vector2Int coord, WeaponRangeShape weaponRangeShape, int minRange, int maxRange)
    {
        switch (weaponRangeShape)
        {
            
        }
    }

    private void Select(InputAction.CallbackContext context)
    {
        WeaponData weaponData = _controller.SelectedWeaponData;

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                break;

            case WeaponType.SelectDirection:

                _controller.SelectedPlayerUnit.ConsumeEnergy(weaponData.EnergyCost);
                
                foreach (Vector2Int coord in _globalTargetCoords)
                {
                    AttackTarget(weaponData, coord);
                }
                _controller.SetState(_controller.CommandEndState);
                break;
            
        }
    }

    private void AttackTarget(WeaponData weaponData, Vector2Int targetCoord)
    {
        if (!_mapManager.TileMap[targetCoord.x, targetCoord.y].CanSelect) return;

        UnitBase unit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (unit != null)
        {
            EnemyUnit enemyUnit = (unit as EnemyUnit);

            if (enemyUnit == null) return;

            _controller.SelectedPlayerUnit.Weapon.Attack(_controller.SelectedWeaponData, enemyUnit);
        }
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        _controller.SetState(_controller.UnitCommandState);
        _controller.HideCursor();
    }

    private void BindInputActions()
    {
        _selectAction.action.Enable();
        _selectAction.action.performed += Select;
        _cancelAction.action.Enable();
        _cancelAction.action.performed += Cancel;
        _cursorPosition.action.Enable();
    }

    private void UnbindInputActions()
    {
        _selectAction.action.Disable();
        _selectAction.action.performed -= Select;
        _cancelAction.action.Disable();
        _cancelAction.action.performed -= Cancel;
        _cursorPosition.action.Disable();
    }
}
