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
    private WeaponDirection _prevWeaponDirection = WeaponDirection.None;

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
        _controller.SelectedPlayerUnit.Weapon.ShowSelectRange(_controller.SelectedWeaponData);
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

        _controller.SelectedPlayerUnit.Weapon.ShowWeaponRange(
            coord, UnitType.Enemy,
            _mapManager, GetWeaponDirection(_controller.SelectedPlayerUnit.transform.position.WorldToCoord(), coord),
            _controller.SelectedWeaponData, true);

        _globalTargetCoords = _controller.SelectedPlayerUnit.Weapon.GetGlobalTargetCoords();
        _controller.ShowCursor(coord);
        ShowBattleInfo(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        _mapManager.TileRange.InitTiles();
        _controller.SelectedPlayerUnit.Weapon.ClearCoords();
        HideUI();
        _prevWeaponDirection = WeaponDirection.None;
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
            AttackTarget(weaponData, coord);
            if (CalculateGetReturnAttack(coord)) GetReturnAttack(coord);
        }
        _controller.SetState(_controller.CommandEndState);
    }

    private void AttackTarget(WeaponData weaponData, Vector2Int targetCoord)
    {
        UnitBase targetUnit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (targetUnit != null)
        {
            if (targetUnit.UnitType != UnitType.Enemy) return;

            bool canHit = ProbabilityCalculator.CalculateProbability(
                _controller.Turn,
                _controller.TotalPlayerUnits,
                _controller.CurrentTurnCount,
                _controller.SelectedPlayerUnit.UnitStat,
                weaponData,
                targetUnit.UnitStat
                );

            if (canHit) _controller.SelectedPlayerUnit.Weapon.Attack(_controller.SelectedWeaponData, targetUnit);
            else Debug.Log($"{targetUnit.UnitStat.UnitData.Name} avoided");
        }
    }

    private bool CalculateGetReturnAttack(Vector2Int targetCoord)
    {
        bool isHit = false;
        UnitBase targetUnit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        Vector2Int attackerCoord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();
        int distance = VectorExtensions.CoordDistance(attackerCoord, targetCoord);

        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        if (targetWeapon.MinRange <= distance
            && targetWeapon.MaxRange >= distance)
        {
            isHit = ProbabilityCalculator.CalculateProbability(
                _controller.Turn,
                _controller.TotalPlayerUnits,
                _controller.CurrentTurnCount,
                targetUnit.UnitStat,
                targetWeapon,
                _controller.SelectedPlayerUnit.UnitStat
                );
        }

        return isHit;
    }

    private void GetReturnAttack(Vector2Int targetCoord)
    {
        UnitBase targetUnit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];
        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        targetUnit.Weapon.Attack(targetWeapon, _controller.SelectedPlayerUnit);
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
