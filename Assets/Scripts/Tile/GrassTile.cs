using UnityEngine;

public class GrassTile : TileBase
{
    public override TileType TileType => TileType.Grass;
    public override int MoveCost => 1;
}
