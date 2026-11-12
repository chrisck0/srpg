using System;
using UnityEngine;

[Serializable]
public struct UnitPosition
{
    public int Id;
    public Vector2Int Position;
    public DirectionType Direction;

    public UnitPosition(int id, Vector2Int position, DirectionType direction)
    {
        Id = id;
        Position = position;
        Direction = direction;
    }
}