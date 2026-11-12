using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetSelectState : StateBase
{
    private UnitCommandStateController _controller;
    private MapManager _mapManager => _controller.MapManager;
    private InputActionReference _selectAction;
    private InputActionReference _cancelAction;
    private InputActionReference _cursorPosition;
    private LayerMask _ground;
    private TileBase _currentTile;
    private List<Vector2Int> _globalTargetCoords;

    public TargetSelectState(UnitCommandStateController controller,
        InputActionReference selectAction,
        InputActionReference cancelAction,
        InputActionReference cursorPosition,
        LayerMask ground)
    {
        _controller = controller;
        _selectAction = selectAction;
        _cancelAction = cancelAction;
        _cursorPosition = cursorPosition;
        _ground = ground;
    }

    public override void Enter()
    {
        BindInputActions();
        _controller.SelectedPlayerUnit.Weapon.SetRange(_controller.SelectedWeaponData, _mapManager, true);
    }

    public override void Update()
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

        DirectionType weaponDirection = GetWeaponDirection(_controller.SelectedPlayerUnit.transform.position.WorldToCoord(), coord);
        
        _controller.SelectedPlayerUnit.Weapon.SelectWeaponRange(
            coord, UnitType.Enemy,
            _mapManager, weaponDirection,
            _controller.SelectedWeaponData, true);

        _globalTargetCoords = _controller.SelectedPlayerUnit.Weapon.GetTargetCoords();
        _controller.ShowCursor(coord);
        ShowBattleInfo(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        _mapManager.TileRange.InitTiles();
        _controller.SelectedPlayerUnit.Weapon.ClearCoords();
        HideUI();
    }

    private DirectionType GetWeaponDirection(Vector2Int startCoord, Vector2Int targetCoord)
    {
        Vector2Int directionCoord = targetCoord - startCoord;
        if (Mathf.Abs(directionCoord.x) >= Mathf.Abs(directionCoord.y))
        {
            if (Mathf.Sign(directionCoord.x) == 1f)
            {
                return DirectionType.Right;
            }
            else
            {
                return DirectionType.Left;
            }
        }
        else
        {
            if (Mathf.Sign(directionCoord.y) == 1f)
            {
                return DirectionType.Up;
            }
            else
            {
                return DirectionType.Down;
            }
        }
    }

    private void ShowBattleInfo(Vector2Int targetCoord)
    {
        UnitBase targetUnit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (targetUnit == null || targetUnit.UnitType != UnitType.Enemy)
        {
            _controller.HideBattleInfo();
            return;
        }

        foreach (Vector2Int coord in _globalTargetCoords)
        {
            if (coord != targetCoord) continue;

            WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];

            Vector2Int currentCoord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();
            int distance = VectorExtensions.CoordDistance(currentCoord, coord);

            bool canReturnAttack = targetWeapon.MinRange <= distance
                && targetWeapon.MaxRange >= distance;

                _controller.ShowBattleInfo(_controller.SelectedPlayerUnit.UnitStat,
                targetUnit.UnitStat,
                _controller.SelectedWeaponData,
                targetUnit.Weapon.WeaponDataList[0],
                canReturnAttack,
                true);
            return;
        }
        _controller.HideBattleInfo();
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (_globalTargetCoords.Count == 0) return;

        WeaponData weaponData = _controller.SelectedWeaponData;

        if (weaponData.WeaponType == WeaponType.SelectDirection
            && !_controller.SelectedPlayerUnit.Weapon.GetCanAttackArea()) return;

        _controller.SelectedPlayerUnit.ConsumeEnergy(weaponData.EnergyCost);

        foreach (Vector2Int coord in _globalTargetCoords)
        {
            UnitBase targetUnit = _mapManager.UnitMap[coord.x, coord.y];

            if (targetUnit == null || targetUnit.UnitType != UnitType.Enemy) continue;

            _controller.SelectedPlayerUnit.Weapon.AttackTarget(
                targetUnit,
                weaponData,
                _controller.Turn,
                _controller.TotalPlayerUnits,
                _controller.CurrentTurnCount
                );

            if (_controller.SelectedPlayerUnit.Weapon.CalculateGetReturnAttack(
                targetUnit,
                VectorExtensions.CoordDistance(coord, _controller.SelectedPlayerUnit.transform.position.WorldToCoord()),
                _controller.Turn,
                _controller.TotalPlayerUnits,
                _controller.CurrentTurnCount
                ))
                _controller.SelectedPlayerUnit.Weapon.GetReturnAttack(targetUnit);
        }
        _controller.SetState(_controller.CommandEndState);
    }

    private void Cancel(InputAction.CallbackContext context)
    {
        _controller.SetState(_controller.UnitCommandState);
        _controller.HideCursor();
    }

    private void HideUI()
    {
        _controller.HideCursor();
        _controller.HideBattleInfo();
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
