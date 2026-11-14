using System.Collections.Generic;
using UnityEngine;

public class AttackScoreCalculator
{
    private EnemyUnit _owner;
    private MapManager _mapManager;
    private HashSet<Vector2Int> _reachableCoords;
    private List<Vector2Int> _targetableLists = new();
    private List<Vector2Int> _targetLists = new();
    private HashSet<Vector2Int> _dangerCoords;
    private DirectionType[] _directions = { DirectionType.Up, DirectionType.Right, DirectionType.Down, DirectionType.Left };

    public AttackScoreCalculator(EnemyUnit owner)
    {
        _owner = owner;
    } 

    public AttackScore CalculateScore(UnitBase unit, MapManager mapManager, Vector2Int initialCoord, HashSet<Vector2Int> dangerCoords)
    {
        _dangerCoords = dangerCoords;
        _dangerCoords.Clear();

        _mapManager = mapManager;

        _mapManager.TileRange.UnitMoveRangeAndCalculateTotal(unit, initialCoord);
        _reachableCoords = unit.DistanceRange.ReachableCoords;

        AttackScore maxAttackScore = default;

        foreach (Vector2Int coord in _reachableCoords)
        {
            AttackScore currentAttackScore = CalculateAttackScore(unit, coord);
            if (currentAttackScore.Score > maxAttackScore.Score)
            {
                maxAttackScore = currentAttackScore;
            }
        }

        return maxAttackScore;
    }

    private AttackScore CalculateAttackScore(UnitBase unit, Vector2Int currentCoord)
    {
        AttackScore maxAttackScore = default;
        foreach (WeaponData weaponData in _owner.Weapon.WeaponDataList)
        {
            if (weaponData.EnergyCost > _owner.Stat.Energy) continue;

            AttackScore currentAttackScore = default;

            switch (weaponData.WeaponType)
            {
                case WeaponType.SelectTarget:
                    currentAttackScore = CalculateSelectTargetScore(unit, currentCoord, weaponData);
                    break;

                case WeaponType.SelectDirection:
                    currentAttackScore = CalculateSelectDirectionScore(unit, currentCoord, weaponData);
                    break;
            }

            if (currentAttackScore.Score > maxAttackScore.Score) maxAttackScore = currentAttackScore;
        }
        // Debug.Log($"{currentCoord} {maxAttackScore.Score}");
        return maxAttackScore;
    }

    // Weapon이 Select Target
    private AttackScore CalculateSelectTargetScore(UnitBase unit, Vector2Int currentCoord, WeaponData weaponData)
    {
        _targetableLists.Clear();

        _mapManager.TileRange.SelectTargetWeaponRange(
            unit, currentCoord, weaponData.MinRange, weaponData.MaxRange, _targetableLists, false);

        AttackScore maxAttackScore = default;
        foreach (Vector2Int targetableCoord in _targetableLists)
        {
            _dangerCoords.Add(targetableCoord);

            UnitBase target = _mapManager.UnitMap[targetableCoord.x, targetableCoord.y];
            if (target == null || target.UnitType != UnitType.Player) continue;

            AttackScore newAttackScore = CalculateSelectTargetRangeScore(unit, currentCoord, targetableCoord, weaponData);
            if (newAttackScore.Score > maxAttackScore.Score)
            {
                maxAttackScore = newAttackScore;
            }
        }
        return maxAttackScore;
    }

    private AttackScore CalculateSelectTargetRangeScore(UnitBase unit, Vector2Int currentCoord, Vector2Int targetableCoord, WeaponData weaponData)
    {
        _targetLists.Clear();

        _mapManager.TileRange.SelectTargetWeaponRange(unit, targetableCoord, 0, weaponData.ExplosionRange, _targetLists, true);

        int score = 0;
        foreach (Vector2Int targetCoord in _targetLists)
        {
            _dangerCoords.Add(targetCoord);
            score += AttackScore(currentCoord, targetCoord, weaponData.Attack);
        }

        return new AttackScore(currentCoord, weaponData, DirectionType.None, targetableCoord, score);
    }

    private AttackScore CalculateSelectDirectionScore(UnitBase unit, Vector2Int currentCoord, WeaponData weaponData)
    {
        AttackScore maxAttackScore = default;
        SetLocalTargetCoords(currentCoord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
        foreach (DirectionType direction in _directions)
        {
            AttackScore currentAttackScore = CalculateSelectDirectionRangeScore(currentCoord, direction, weaponData);
            if (currentAttackScore.Score > maxAttackScore.Score) maxAttackScore = currentAttackScore;
        }

        return maxAttackScore;
    }

    private void SetLocalTargetCoords(Vector2Int coord, WeaponRangeShape weaponRangeShape, int explosionRange)
    {
        _targetableLists.Clear();

        switch (weaponRangeShape)
        {
            case WeaponRangeShape.Line:
                for (int i = 0; i < explosionRange; i++)
                {
                    _targetableLists.Add(new Vector2Int(0, i + 1));
                }
                break;

            case WeaponRangeShape.CircularSector:
                for (int i = 0; i < explosionRange; i++)
                {
                    for (int j = -i; j <= i; j++)
                    {
                        _targetableLists.Add(new Vector2Int(j, i + 1));
                    }
                }
                break;
        }
    }

    private AttackScore CalculateSelectDirectionRangeScore(Vector2Int coord, DirectionType direction, WeaponData weaponData)
    {
        bool _canAttackArea = false;

        AttackScore attackScore = default;
        int score = 0;
        foreach (Vector2Int localCoord in _targetableLists)
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

            _dangerCoords.Add(globalCoord);

            UnitBase unit = _mapManager.UnitMap[globalCoord.x, globalCoord.y];

            if (unit != null && unit.UnitType == UnitType.Player) _canAttackArea = true;

            score += AttackScore(coord, globalCoord, weaponData.Attack);
        }

        if (_canAttackArea)
        {
            attackScore = new AttackScore(coord, weaponData, direction, Vector2Int.zero, score);
        }
        return attackScore;
    }

    private int AttackScore(Vector2Int currentCoord, Vector2Int targetCoord, int attack)
    {
        UnitBase targetUnit = _mapManager.UnitMap[targetCoord.x, targetCoord.y];
        int distance = VectorExtensions.CoordDistance(currentCoord, targetCoord);

        if (targetUnit == null || targetUnit.UnitType != UnitType.Player) return 0;

        if (targetUnit.Stat.Health < attack) return 100 + attack;

        if (!targetUnit.Weapon.CanGetReturnAttack(distance)) return 50 + attack;

        return attack;
    }
}
