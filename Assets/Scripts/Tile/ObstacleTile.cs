using UnityEngine;

public class ObstacleTile : TileBase
{
    public override TileType TileType => TileType.Obstacle;
    public override int MoveCost => int.MaxValue;
}
