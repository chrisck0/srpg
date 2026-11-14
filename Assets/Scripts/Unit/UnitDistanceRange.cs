using System.Collections.Generic;
using UnityEngine;

public class UnitDistanceRange : MonoBehaviour
{
    [SerializeField] private UnitBase _owner;

    private HashSet<Vector2Int> _reachableCoords = new();
    private HashSet<Vector2Int> _distanceCoords = new();
    private HashSet<Vector2Int> _rangeCoords = new();
    private TileBase[,] _tileMap;

    public HashSet<Vector2Int> ReachableCoords => _reachableCoords;
    public HashSet<Vector2Int> DistanceCoords => _distanceCoords;
    public HashSet<Vector2Int> RangeCoords => _rangeCoords;

    public void SetMap(TileBase[,] tileMap)
    {
        _tileMap = tileMap;
    }

    public void ShowDistance(TileCost[,] tileCostMap)
    {
        Debug.Log($"Show Distance {_distanceCoords.Count}");
        foreach (Vector2Int coord in _distanceCoords)
        {
            TileBase tile = _tileMap[coord.x, coord.y];
            int distance = tileCostMap[coord.x, coord.y].Distance;
            tile.ShowDistance(distance);
        }
    }

    public void HideDistance()
    {
        foreach (Vector2Int coord in _distanceCoords)
        {
            TileBase tile = _tileMap[coord.x, coord.y];
            tile.HideDistance();
        }
    }

    public void ShowRange()
    {
        foreach (Vector2Int coord in _rangeCoords)
        {
            TileBase tile = _tileMap[coord.x, coord.y];
            tile.ShowRange();
        }
    }

    public void HideRange()
    {
        foreach (Vector2Int coord in _rangeCoords)
        {
            TileBase tile = _tileMap[coord.x, coord.y];
            tile.HideRange();
        }
    }
}
