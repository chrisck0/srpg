using UnityEngine;

public class ForestTile : TileBase
{
    public override TileType TileType => TileType.Forest;
    public override int MoveCost => 2;
}
