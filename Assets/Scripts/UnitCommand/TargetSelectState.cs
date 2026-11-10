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
    private List<Vector2Int> _localTargetCoords = new();
    private List<Vector2Int> _globalTargetCoords = new();
    private WeaponDirection _prevWeaponDirection = WeaponDirection.None;
    private bool _canAttackArea;

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
        ShowSelectRange(_controller.SelectedWeaponData);
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

        ShowWeaponRange(coord);
        ShowBattleInfo(coord);
    }

    public override void Exit()
    {
        UnbindInputActions();
        _mapManager.TileRange.InitTiles();
        _localTargetCoords.Clear();
        _globalTargetCoords.Clear();
        HideUI();
        _prevWeaponDirection = WeaponDirection.None;
    }

    private void ShowSelectRange(WeaponData weaponData)
    {
        Vector2Int coord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                ShowSelectTargetRange(coord, weaponData.MinRange, weaponData.MaxRange, true);
                break;
            case WeaponType.SelectDirection:
                SetLocalTargetCoords(coord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
                break;
        }
    }

    private void ShowSelectTargetRange(Vector2Int coord, int minRange, int maxRange, bool isWeaponRange)
    {
        TileCost[,] tileCost = _mapManager.TileRange.SelectTargetWeaponRange(coord, minRange, maxRange, isWeaponRange);
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

    private void ShowWeaponRange(Vector2Int coord)
    {
        Vector2Int playerCoord = _controller.SelectedPlayerUnit.transform.position.WorldToCoord();

        WeaponDirection weaponDirection = GetWeaponDirection(playerCoord, coord);
        WeaponData weaponData = _controller.SelectedWeaponData;

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                HidePreviousGlobalTargetRange();
                ShowMultiTargetRange(coord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
                break;

            case WeaponType.SelectDirection:
                if (_prevWeaponDirection != weaponDirection)
                {
                    HidePreviousGlobalTargetRange();
                    ShowSelectDirectionRange(playerCoord, weaponDirection);
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

    // 범위 공격 표시
    private void ShowSelectDirectionRange(Vector2Int coord, WeaponDirection direction)
    {
        _globalTargetCoords.Clear();
        _canAttackArea = false;

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

            UnitBase unit = _mapManager.UnitMap[globalCoord.x, globalCoord.y];

            if (unit != null && unit.UnitType == UnitType.Enemy) _canAttackArea = true;

            _mapManager.TileMap[globalCoord.x, globalCoord.y].ShowRange();
            _globalTargetCoords.Add(globalCoord);
        }
    }

    private void HidePreviousGlobalTargetRange()
    {
        foreach (Vector2Int coord in _globalTargetCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].HideRange();
        }
    }

    // 타겟 선택했을 때 공격 범위 표시
    private void ShowMultiTargetRange(Vector2Int targetCoord, WeaponRangeShape weaponRangeShape, int explosionRange)
    {
        _globalTargetCoords.Clear();

        TileBase tile = _mapManager.TileMap[targetCoord.x, targetCoord.y];
        UnitBase unit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (!tile.CanSelect || unit != null && unit.UnitType == UnitType.Enemy) return;

        switch (weaponRangeShape)
        {
            case WeaponRangeShape.Single:
                _mapManager.TileMap[targetCoord.x, targetCoord.y].ShowRange();
                _globalTargetCoords.Add(targetCoord);

                break;

            case WeaponRangeShape.Explosion:
                _mapManager.TileRange.SelectTargetWeaponRange(targetCoord, 0, explosionRange, false, _globalTargetCoords);
                break;
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
            && !_canAttackArea) return;

        _controller.SelectedPlayerUnit.ConsumeEnergy(weaponData.EnergyCost);

        foreach (Vector2Int coord in _globalTargetCoords)
        {
            AttackTarget(weaponData, coord);
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

            bool isHit = false;

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

            if (canHit) _controller.SelectedPlayerUnit.Weapon.Attack(_controller.SelectedWeaponData, targetUnit);
            else Debug.Log($"{targetUnit.UnitStat.UnitData.Name} avoided");

            if (isHit) targetUnit.Weapon.Attack(targetWeapon, _controller.SelectedPlayerUnit);
            else Debug.Log($"No return attack");
        }
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
