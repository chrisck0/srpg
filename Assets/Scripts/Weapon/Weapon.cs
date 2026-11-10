using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private List<WeaponData> _weaponDataList = new();
    private UnitBase _owner;
    private MapManager _mapManager;
    private List<Vector2Int> _localTargetCoords = new();
    private List<Vector2Int> _globalTargetCoords = new();
    private WeaponDirection _prevWeaponDirection = WeaponDirection.None;
    private bool _canAttackArea;

    public List<WeaponData> WeaponDataList => _weaponDataList;

    private void Awake() => CacheComponents();

    public void AddWeapon(WeaponData weaponData)
    {
        _weaponDataList.Add(weaponData);
    }

    /// <summary>
    /// 무기로 타겟 공격
    /// </summary>
    public void Attack(WeaponData weaponData, UnitBase target)
    {
        Debug.Log($"{_owner.UnitStat.UnitData.Name} attacked {target.UnitStat.UnitData.Name} DMG {weaponData.Attack}");
        target.TakeDamage(weaponData.Attack);
    }

    /// <summary>
    /// 단일 무기가 반격 가능한 사거리면 true, 아니면 false
    /// </summary>
    public bool CanGetReturnAttack(int distance)
    {
        WeaponData weapon = WeaponDataList[0];
        if (weapon.MinRange <= distance
            && weapon.MaxRange >= distance)
        {
            return true;
        }
        return false;
    }

    public void ClearCoords()
    {
        _localTargetCoords.Clear();
        _globalTargetCoords.Clear();
    }

    public void ShowSelectRange(WeaponData weaponData, MapManager mapManager)
    {
        _mapManager = mapManager;

        Vector2Int coord = transform.position.WorldToCoord();

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

        switch (weaponRangeShape)
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

    /// <summary>
    /// 무기 범위 계산
    /// </summary>
    public void ShowWeaponRange(
        Vector2Int targetCoord, UnitType targetUnitType, MapManager mapManager, WeaponDirection weaponDirection,
        WeaponData weaponData, bool showRange)
    {
        _mapManager = mapManager;
        Vector2Int coord = transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                HidePreviousGlobalTargetRange();
                SelectTargetRange(targetCoord, targetUnitType,
                    weaponData.WeaponRangeShape, weaponData.ExplosionRange, showRange);
                break;

            case WeaponType.SelectDirection:
                if (_prevWeaponDirection != weaponDirection)
                {
                    HidePreviousGlobalTargetRange();
                    SelectDirectionRange(coord, targetUnitType, weaponDirection, showRange);
                }
                break;
        }
    }

    private void HidePreviousGlobalTargetRange()
    {
        foreach (Vector2Int coord in _globalTargetCoords)
        {
            _mapManager.TileMap[coord.x, coord.y].HideRange();
        }
    }

    private void SelectTargetRange(Vector2Int targetCoord, UnitType targetUnitType,
        WeaponRangeShape weaponRangeShape, int explosionRange, bool showRange)
    {
        _globalTargetCoords.Clear();

        TileBase tile = _mapManager.TileMap[targetCoord.x, targetCoord.y];
        UnitBase unit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (!tile.CanSelect || unit == null || unit != null && unit.UnitType != targetUnitType) return;

        switch (weaponRangeShape)
        {
            case WeaponRangeShape.Single:
                if (showRange) _mapManager.TileMap[targetCoord.x, targetCoord.y].ShowRange();
                _globalTargetCoords.Add(targetCoord);

                break;

            case WeaponRangeShape.Explosion:
                _mapManager.TileRange.SelectTargetWeaponRange(targetCoord, 0, explosionRange, false, _globalTargetCoords);
                break;
        }
    }

    // 범위 공격 표시
    private void SelectDirectionRange(Vector2Int coord, UnitType targetUnitType, WeaponDirection direction, bool showRange)
    {
        _globalTargetCoords.Clear();
        _canAttackArea = false;

        foreach (Vector2Int localCoord in _localTargetCoords)
        {
            Vector2Int rotatedLocalCoord = localCoord;

            switch (direction)
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
                || globalCoord.y >= _mapManager.TileMap.GetLength(1)) continue;

            UnitBase unit = _mapManager.UnitMap[globalCoord.x, globalCoord.y];

            if (unit != null && unit.UnitType == targetUnitType)
            {
                _canAttackArea = true;
                _globalTargetCoords.Add(globalCoord);
            }
            else
            {
                if (showRange) _globalTargetCoords.Add(globalCoord);
            }

            if (showRange) _mapManager.TileMap[globalCoord.x, globalCoord.y].ShowRange();
        }
    }

    public List<Vector2Int> GetTargetCoords()
    {
        return _globalTargetCoords;
    }

    public bool GetCanAttackArea()
    {
        return _canAttackArea;
    }

    public bool CalculateGetReturnAttack(UnitBase targetUnit, int distance,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        bool isHit = false;

        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        if (targetWeapon.MinRange <= distance
            && targetWeapon.MaxRange >= distance)
        {
            isHit = ProbabilityCalculator.CalculateProbability(
                turn,
                totalPlayerUnits,
                currentTurnCount,
                targetUnit.UnitStat,
                targetWeapon,
                _owner.UnitStat
                );
        }

        return isHit;
    }

    public void AttackTarget(UnitBase targetUnit, WeaponData weaponData,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        if (targetUnit != null)
        {
            if (targetUnit.UnitType != UnitType.Enemy) return;

            bool canHit = ProbabilityCalculator.CalculateProbability(
                turn,
                totalPlayerUnits,
                currentTurnCount,
                _owner.UnitStat,
                weaponData,
                targetUnit.UnitStat
                );

            if (canHit) Attack(weaponData, targetUnit);
            else Debug.Log($"{targetUnit.UnitStat.UnitData.Name} avoided");
        }
    }

    public void GetReturnAttack(UnitBase targetUnit)
    {
        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        targetUnit.Weapon.Attack(targetWeapon, _owner);
    }

    private void CacheComponents()
    {
        _owner = GetComponent<UnitBase>();
    }
}
