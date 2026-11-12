using UnityEngine;

public struct AttackScore
{
    public Vector2Int CurrentCoord;
    public WeaponData AttackWeaponData;
    public DirectionType WeaponDirection;
    public Vector2Int TargetCoord;
    public int Score;

    public AttackScore(Vector2Int currentCoord, WeaponData attackWeaponData, DirectionType direction,
        Vector2Int targetCoord, int score)
    {
        CurrentCoord = currentCoord;
        AttackWeaponData = attackWeaponData;
        WeaponDirection = direction;
        TargetCoord = targetCoord;
        Score = score;
    }
}
