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

    /// <summary>
    /// 무기 범위 계산
    /// </summary>
    public void ShowWeaponRange(MapManager mapManager, WeaponDirection weaponDirection,
        WeaponData weaponData, bool showRange)
    {
        _mapManager = mapManager;
        Vector2Int coord = transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                HidePreviousGlobalTargetRange();
                SelectTargetRange(coord, weaponData.WeaponRangeShape, weaponData.ExplosionRange, showRange);
                break;

            case WeaponType.SelectDirection:
                if (_prevWeaponDirection != weaponDirection)
                {
                    HidePreviousGlobalTargetRange();
                    SelectDirectionRange(coord, weaponDirection, showRange);
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

    private void SelectTargetRange(Vector2Int targetCoord, WeaponRangeShape weaponRangeShape,
        int explosionRange, bool showRange)
    {
        _globalTargetCoords.Clear();

        TileBase tile = _mapManager.TileMap[targetCoord.x, targetCoord.y];
        UnitBase unit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (!tile.CanSelect || unit != null && unit.UnitType == UnitType.Enemy) return;

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
    private void SelectDirectionRange(Vector2Int coord, WeaponDirection direction, bool showRange)
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

            if (unit != null && unit.UnitType == UnitType.Enemy) _canAttackArea = true;

            if (showRange) _mapManager.TileMap[globalCoord.x, globalCoord.y].ShowRange();
            _globalTargetCoords.Add(globalCoord);
        }
    }

    private void CacheComponents()
    {
        _owner = GetComponent<UnitBase>();
    }
}
