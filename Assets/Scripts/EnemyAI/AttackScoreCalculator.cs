using System.Collections.Generic;
using UnityEngine;

public static class AttackScoreCalculator
{
    private static DirectionType[] _directions = { DirectionType.Up, DirectionType.Right, DirectionType.Down, DirectionType.Left };

    public static AttackScore CalculateScore(UnitBase unit, MapManager mapManager, Vector2Int initialCoord)
    {
        unit.DistanceRange.TargetableCoords.Clear();
        mapManager.TileRange.UnitMoveRangeAndCalculateTotal(unit, initialCoord);

        AttackScore maxAttackScore = default;

        foreach (Vector2Int coord in unit.DistanceRange.ReachableCoords)
        {
            AttackScore currentAttackScore = CalculateAttackScore(unit, mapManager, coord);
            if (currentAttackScore.Score > maxAttackScore.Score)
            {
                maxAttackScore = currentAttackScore;
            }
        }

        return maxAttackScore;
    }

    private static AttackScore CalculateAttackScore(UnitBase unit, MapManager mapManager, Vector2Int currentCoord)
    {
        AttackScore maxAttackScore = default;
        foreach (WeaponData weaponData in unit.Weapon.WeaponDataList)
        {
            if (weaponData.EnergyCost > unit.Stat.Energy) continue;

            AttackScore currentAttackScore = default;

            switch (weaponData.WeaponType)
            {
                case WeaponType.SelectTarget:
                    currentAttackScore = CalculateSelectTargetScore(unit, mapManager, currentCoord, weaponData);
                    break;

                case WeaponType.SelectDirection:
                    currentAttackScore = CalculateSelectDirectionScore(unit, mapManager, currentCoord, weaponData);
                    break;
            }

            if (currentAttackScore.Score > maxAttackScore.Score) maxAttackScore = currentAttackScore;
        }
        // Debug.Log($"{currentCoord} {maxAttackScore.Score}");
        return maxAttackScore;
    }

    // Weapon이 Select Target
    private static AttackScore CalculateSelectTargetScore(UnitBase unit, MapManager mapManager, Vector2Int currentCoord, WeaponData weaponData)
    {
        mapManager.TileRange.SelectTargetWeaponRange(
            unit, currentCoord, weaponData.MinRange, weaponData.MaxRange, unit.Weapon.LocalTargetCoords, false);

        AttackScore maxAttackScore = default;
        foreach (Vector2Int targetableCoord in unit.Weapon.LocalTargetCoords)
        {
            EnemyUnit enemy = unit as EnemyUnit;

            UnitBase target = mapManager.UnitMap[targetableCoord.x, targetableCoord.y];
            
            AttackScore newAttackScore = CalculateSelectTargetRangeScore(unit, mapManager, currentCoord, targetableCoord, weaponData);
            if (newAttackScore.Score > maxAttackScore.Score)
            {
                maxAttackScore = newAttackScore;
            }
        }
        return maxAttackScore;
    }

    private static AttackScore CalculateSelectTargetRangeScore(UnitBase unit, MapManager mapManager, Vector2Int currentCoord, Vector2Int targetableCoord, WeaponData weaponData)
    {
        mapManager.TileRange.SelectTargetWeaponRange(
            unit, targetableCoord, 0, weaponData.ExplosionRange, unit.Weapon.GlobalTargetCoords, true);

        int score = 0;
        EnemyUnit enemy = unit as EnemyUnit;

        foreach (Vector2Int targetCoord in unit.Weapon.GlobalTargetCoords)
        {
            UnitBase targetUnit = mapManager.UnitMap[targetCoord.x, targetCoord.y];

            if (targetUnit == null || targetUnit.UnitType != unit.UnitType)
                unit.DistanceRange.TargetableCoords.Add(targetCoord);
            score += AttackScore(mapManager, currentCoord, targetCoord, weaponData.Attack);
        }

        return new AttackScore(currentCoord, weaponData, DirectionType.None, targetableCoord, score);
    }

    private static AttackScore CalculateSelectDirectionScore(UnitBase unit, MapManager mapManager, Vector2Int currentCoord, WeaponData weaponData)
    {
        AttackScore maxAttackScore = default;
        SetLocalTargetCoords(unit, currentCoord, weaponData.WeaponRangeShape, weaponData.ExplosionRange);
        foreach (DirectionType direction in _directions)
        {
            AttackScore currentAttackScore = CalculateSelectDirectionRangeScore(unit, mapManager, currentCoord, direction, weaponData);
            if (currentAttackScore.Score > maxAttackScore.Score) maxAttackScore = currentAttackScore;
        }

        return maxAttackScore;
    }

    private static void SetLocalTargetCoords(UnitBase unit, Vector2Int coord, WeaponRangeShape weaponRangeShape, int explosionRange)
    {
        List<Vector2Int> localCoords = unit.Weapon.LocalTargetCoords;
        localCoords.Clear();

        switch (weaponRangeShape)
        {
            case WeaponRangeShape.Line:
                for (int i = 0; i < explosionRange; i++)
                {
                    localCoords.Add(new Vector2Int(0, i + 1));
                }
                break;

            case WeaponRangeShape.CircularSector:
                for (int i = 0; i < explosionRange; i++)
                {
                    for (int j = -i; j <= i; j++)
                    {
                        localCoords.Add(new Vector2Int(j, i + 1));
                    }
                }
                break;
        }
    }

    private static AttackScore CalculateSelectDirectionRangeScore(UnitBase unit, MapManager mapManager, Vector2Int coord, DirectionType direction, WeaponData weaponData)
    {
        bool _canAttackArea = false;

        AttackScore attackScore = default;
        int score = 0;
        foreach (Vector2Int localCoord in unit.Weapon.LocalTargetCoords)
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
                || globalCoord.x >= mapManager.TileMap.GetLength(0)
                || globalCoord.y >= mapManager.TileMap.GetLength(1)) continue;

            UnitBase targetUnit = mapManager.UnitMap[globalCoord.x, globalCoord.y];

            if (targetUnit == null || targetUnit.UnitType != unit.UnitType)
                unit.DistanceRange.TargetableCoords.Add(globalCoord);

            if (targetUnit != null && targetUnit.UnitType != unit.UnitType) _canAttackArea = true;

            score += AttackScore(mapManager, coord, globalCoord, weaponData.Attack);
        }

        if (_canAttackArea)
        {
            attackScore = new AttackScore(coord, weaponData, direction, Vector2Int.zero, score);
        }
        return attackScore;
    }

    private static int AttackScore(MapManager mapManager, Vector2Int currentCoord, Vector2Int targetCoord, int attack)
    {
        UnitBase targetUnit = mapManager.UnitMap[targetCoord.x, targetCoord.y];
        int distance = VectorExtensions.CoordDistance(currentCoord, targetCoord);

        if (targetUnit == null || targetUnit.UnitType != UnitType.Player) return 0;

        if (targetUnit.Stat.Health < attack) return 100 + attack;

        if (!targetUnit.Weapon.CanGetReturnAttack(distance)) return 50 + attack;

        return attack;
    }
}
