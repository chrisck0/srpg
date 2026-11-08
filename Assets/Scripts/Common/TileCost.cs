using UnityEngine;

public class TileCost
{
    private Vector2Int _coord;
    private TileData _tileData;
    private TileCost _prevTileCost;
    private int _distance;

    public Vector2Int Coord => _coord;
    public TileData TileData => _tileData;
    public TileCost PrevTileCost => _prevTileCost;
    public int Distance => _distance;

    public TileCost(Vector2Int coord)
    {
        _coord = coord;
        InitData();
    }

    public void SetData(TileData tileData, TileCost prevTileCost, int distance)
    {
        _tileData = tileData;
        _prevTileCost = prevTileCost;
        _distance = distance;
    }

    public void InitData()
    {
        _tileData = null;
        _prevTileCost = null;
        _distance = int.MaxValue;
    }
}
