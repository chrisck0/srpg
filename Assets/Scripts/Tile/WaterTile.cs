using UnityEngine;

public class WaterTile : TileBase
{
    public override TileType TileType => TileType.Water;
    public override int MoveCost => int.MaxValue;
}
