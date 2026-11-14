using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private List<WeaponData> _weaponDataList = new();
    private UnitBase _owner;
    private MapManager _mapManager;
    private List<Vector2Int> _localTargetCoords = new();
    private List<Vector2Int> _globalTargetCoords = new();
    private DirectionType _prevWeaponDirection = DirectionType.None;
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
        Debug.Log($"{_owner.Stat.Data.Name} attacked {target.Stat.Data.Name} DMG {weaponData.Attack}");
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

    /// <summary>
    /// 선택 무기 사거리, 공격 범위 설정
    /// </summary>
    public void SetRange(WeaponData weaponData, MapManager mapManager)
    {
        _mapManager = mapManager;

        Vector2Int coord = transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                SelectTargetWeaponDistance(coord, weaponData.MinRange, weaponData.MaxRange);
                break;
            case WeaponType.SelectDirection:
                SetLocalTargetCoords(coord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
                break;
        }
    }

    // 무기 사거리 설정
    private void SelectTargetWeaponDistance(Vector2Int coord, int minRange, int maxRange)
    {
        _mapManager.TileRange.SelectTargetWeaponRange(
            _owner, coord, minRange, maxRange, _globalTargetCoords, false);
    }

    // 범위 무기의 공격 범위 설정 (로컬)
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
    /// 모든 종류 무기 공격 범위 계산
    /// </summary>
    public void SelectWeaponRange(
        Vector2Int targetCoord, UnitType targetUnitType, MapManager mapManager, DirectionType weaponDirection,
        WeaponData weaponData)
    {
        _mapManager = mapManager;
        Vector2Int coord = transform.position.WorldToCoord();

        switch (weaponData.WeaponType)
        {
            case WeaponType.SelectTarget:
                SelectTargetRange(targetCoord, targetUnitType,
                    weaponData.WeaponRangeShape, weaponData.ExplosionRange);
                break;

            case WeaponType.SelectDirection:
                if (_prevWeaponDirection != weaponDirection)
                {
                    SelectDirectionRange(coord, targetUnitType, weaponDirection);
                }
                break;
        }
    }

    // 타겟 선택 무기의 범위 설정
    private void SelectTargetRange(Vector2Int targetCoord, UnitType targetUnitType,
        WeaponRangeShape weaponRangeShape, int explosionRange)
    {
        _globalTargetCoords.Clear();
        _owner.DistanceRange.RangeCoords.Clear();

        TileBase tile = _mapManager.TileMap[targetCoord.x, targetCoord.y];
        UnitBase unit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];

        if (!tile.CanSelect || unit == null || unit != null && unit.UnitType != targetUnitType) return;

        switch (weaponRangeShape)
        {
            case WeaponRangeShape.Single:
                _owner.DistanceRange.RangeCoords.Add(targetCoord);
                _globalTargetCoords.Add(targetCoord);
                break;

            case WeaponRangeShape.Explosion:
                _mapManager.TileRange.SelectTargetWeaponRange(_owner, targetCoord, 0, explosionRange, _globalTargetCoords, true);
                break;
        }
    }

    // 범위 공격 무기의 방향, 범위 설정
    private void SelectDirectionRange(Vector2Int coord, UnitType targetUnitType, DirectionType direction)
    {
        _globalTargetCoords.Clear();
        _canAttackArea = false;
        _owner.DistanceRange.RangeCoords.Clear();

        foreach (Vector2Int localCoord in _localTargetCoords)
        {
            Vector2Int rotatedLocalCoord = localCoord;

            switch (direction)
            {
                case DirectionType.Up:
                    rotatedLocalCoord = localCoord;
                    break;
                case DirectionType.Right:
                    rotatedLocalCoord = new Vector2Int(localCoord.y, -localCoord.x);
                    break;
                case DirectionType.Down:
                    rotatedLocalCoord = new Vector2Int(-localCoord.x, -localCoord.y);
                    break;
                case DirectionType.Left:
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

            _owner.DistanceRange.RangeCoords.Add(globalCoord);
        }
    }

    /// <summary>
    /// 타겟 좌표 반환
    /// </summary>
    public List<Vector2Int> GetTargetCoords()
    {
        return _globalTargetCoords;
    }

    /// <summary>
    /// 범위 공격 무기의 범위 내에 공격 대상이 있는지 여부
    /// </summary>
    public bool GetCanAttackArea()
    {
        return _canAttackArea;
    }

    /// <summary>
    /// 반격 가능한 거리인지 여부
    /// </summary>
    public bool CanGetReturnAttack(UnitBase targetUnit, int distance)
    {
        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        if (targetWeapon.MinRange <= distance
            && targetWeapon.MaxRange >= distance)
            return true;
        return false;
    }

    /// <summary>
    /// 반격 확률 계산하여 반격 받는지 여부
    /// </summary>
    /// <returns></returns>
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
                targetUnit.Stat,
                targetWeapon,
                _owner.Stat
                );
        }

        return isHit;
    }

    /// <summary>
    /// 타겟 공격 가능한지 여부
    /// </summary>
    public bool CanAttackTargets(WeaponData weaponData)
    {
        if (_globalTargetCoords.Count == 0) return false;

        if (weaponData.WeaponType == WeaponType.SelectDirection
            && !GetCanAttackArea()) return false;

        return true;
    }

    /// <summary>
    /// 타겟 좌표 전체에 공격 
    /// </summary>
    public async UniTask AttackTargets(WeaponData weaponData, UnitType targetType,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        _owner.ConsumeEnergy(weaponData.EnergyCost);

        _owner.OnAttackWeapon?.Invoke(weaponData.Id);

        await UniTask.Delay(1000, DelayType.DeltaTime);

        foreach (Vector2Int coord in _globalTargetCoords)
        {
            UnitBase targetUnit = _mapManager.UnitMap[coord.x, coord.y];

            if (targetUnit == null || targetUnit.UnitType != targetType) continue;

            AttackTarget(
                targetUnit,
                weaponData,
                turn,
                totalPlayerUnits,
                currentTurnCount
                );
        }

        await UniTask.Delay(2000, DelayType.DeltaTime);

        _owner.DistanceRange.HideRange();
    }

    /// <summary>
    /// 공격한 타겟 좌표 유닛에게 반격
    /// </summary>
    public async UniTask GetAttackReturn(UnitType targetType,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        foreach (Vector2Int coord in _globalTargetCoords)
        {
            UnitBase targetUnit = _mapManager.UnitMap[coord.x, coord.y];

            if (targetUnit == null || targetUnit.UnitType != targetType) continue;

            int distance = VectorExtensions.CoordDistance(coord, transform.position.WorldToCoord());

            if (!targetUnit.Weapon.CanGetReturnAttack(distance)) continue;

            await GetReturnAttack(targetUnit, distance, turn, totalPlayerUnits, currentTurnCount);
            await UniTask.Delay(2000, DelayType.DeltaTime);
            _owner.DistanceRange.HideRange();
        }
    }

    /// <summary>
    /// 타겟 공격
    /// </summary>
    private void AttackTarget(UnitBase targetUnit, WeaponData weaponData,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        if (targetUnit != null)
        {
            bool canHit = ProbabilityCalculator.CalculateProbability(
                turn,
                totalPlayerUnits,
                currentTurnCount,
                _owner.Stat,
                weaponData,
                targetUnit.Stat
                );

            if (canHit) Attack(weaponData, targetUnit);
            else targetUnit.Avoid();
        }
    }

    /// <summary>
    /// 타겟에게서 반격
    /// </summary>
    private async UniTask GetReturnAttack(UnitBase targetUnit, int distance,
        int turn, int totalPlayerUnits, int currentTurnCount)
    {
        WeaponData targetWeapon = targetUnit.Weapon.WeaponDataList[0];
        targetUnit.OnAttackWeapon?.Invoke(targetWeapon.Id);

        _owner.DistanceRange.RangeCoords.Clear();

        Vector2Int coord = transform.position.WorldToCoord();
        TileBase tile = _mapManager.TileMap[coord.x, coord.y];

        _owner.DistanceRange.RangeCoords.Add(coord);
        _owner.DistanceRange.ShowRange();

        await UniTask.Delay(1000, DelayType.DeltaTime);

        if (CalculateGetReturnAttack(
                targetUnit,
                distance,
                turn,
                totalPlayerUnits,
                currentTurnCount
                ))
            targetUnit.Weapon.Attack(targetWeapon, _owner);
        else _owner.Avoid();
    }

    private void CacheComponents()
    {
        _owner = GetComponent<UnitBase>();
    }
}
