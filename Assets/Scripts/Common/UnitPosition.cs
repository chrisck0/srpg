using System;
using UnityEngine;

[Serializable]
public struct UnitPosition
{
    public int Id;
    public Vector2Int Position;

    public UnitPosition(int id, Vector2Int position)
    {
        Id = id;
        Position = position;
    }
}