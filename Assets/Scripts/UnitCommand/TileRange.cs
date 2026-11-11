using System.Collections.Generic;
using UnityEngine;

public class TileRange
{
    private TileBase[,] _tileMap;
    private UnitBase[,] _unitMap;
    private bool[,] _visitedMap;
    private TileCost[,] _tileCostMap;
    private List<TileCost> _priorityQueue = new();
    private Dictionary<TileCost, int> _priorityQueueIndex = new();
    private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    private HashSet<Vector2Int> _reachableTiles = new();

    public HashSet<Vector2Int> ReachableTiles => _reachableTiles;

    public TileRange(TileBase[,] tileMap, UnitBase[,] unitMap)
    {
        _tileMap = tileMap;
        _unitMap = unitMap;

        _visitedMap = new bool[_tileMap.GetLength(1), _tileMap.GetLength(0)];
        _tileCostMap = new TileCost[_tileMap.GetLength(1), _tileMap.GetLength(0)];

        for (int i = 0; i < _tileMap.GetLength(1); i++)
        {
            for (int j = 0; j < _tileMap.GetLength(0); j++)
            {
                Vector2Int coord = new(j, i);
                _tileCostMap[j, i] = new TileCost(coord);
            }
        }
    }

    private void Init()
    {
        for (int i = 0; i < _tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < _tileMap.GetLength(1); j++)
            {
                _visitedMap[j, i] = false;
                _tileCostMap[j, i].InitData();
            }
        }

        _priorityQueue.Clear();
        _priorityQueueIndex.Clear();
    }

    public void InitTiles()
    {
        for (int i = 0; i < _tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < _tileMap.GetLength(1); j++)
            {
                _tileMap[j, i].InitTile();
            }
        }
    }

    /// <summary>
    /// 이동 가능 위치 표시 및 이동 가능 거리까지만 계산
    /// </summary>
    public TileCost[,] UnitMoveRange(Vector2Int startCoord, int move, UnitType targetUnitType)
    {
        Init();
        _reachableTiles.Clear();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        PriorityQueueAdd(startTileCost);

        while (_priorityQueue.Count > 0)
        {
            TileCost currentTileCost = PriorityQueueRemove();
            Vector2Int currentCoord = currentTileCost.Coord;

            if (currentTileCost.Distance > move) break;

            // 유닛이 좌표 위에 없거나 시작 좌표인 경우
            UnitBase currentUnit = _unitMap[currentCoord.x, currentCoord.y];
            if (!currentUnit || currentCoord == startCoord)
            {
                _tileMap[currentCoord.x, currentCoord.y].ShowDistance(currentTileCost.Distance);
                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();
            }

            _visitedMap[currentCoord.x, currentCoord.y] = true;
            _reachableTiles.Add(currentCoord);

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int nextCoord = currentCoord + direction;

                if (nextCoord.x < 0 || nextCoord.y < 0
                    || nextCoord.x >= _tileMap.GetLength(1) || nextCoord.y >= _tileMap.GetLength(0)
                    || _visitedMap[nextCoord.x, nextCoord.y]) continue;

                TileData nextTileData = _tileMap[nextCoord.x, nextCoord.y].TileData;

                // 도착할 수 없는 타일인 경우
                if (nextTileData.Cost == int.MaxValue
                    || _unitMap[nextCoord.x, nextCoord.y]?.UnitType == targetUnitType)
                {
                    _visitedMap[nextCoord.x, nextCoord.y] = true;
                    continue;
                }

                TileCost nextTileCost = _tileCostMap[nextCoord.x, nextCoord.y];
                if (nextTileCost.Distance <= currentTileCost.Distance + nextTileData.Cost) continue;

                nextTileCost.SetData(
                    nextTileData,
                    currentTileCost,
                    currentTileCost.Distance + nextTileData.Cost
                    );

                if (!_priorityQueueIndex.ContainsKey(nextTileCost)) PriorityQueueAdd(nextTileCost);
                else PriorityQueueCompareWithParent(nextTileCost);
            }
        }

        return _tileCostMap;
    }

    /// <summary>
    /// 이동 가능 위치 표시 및 맵 전체 거리 계산
    /// </summary>
    public TileCost[,] UnitMoveRangeAndCalculateTotal(Vector2Int startCoord, int move, UnitType targetUnitType)
    {
        Init();
        _reachableTiles.Clear();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        PriorityQueueAdd(startTileCost);

        while (_priorityQueue.Count > 0)
        {
            TileCost currentTileCost = PriorityQueueRemove();
            Vector2Int currentCoord = currentTileCost.Coord;

            // 유닛이 좌표 위에 없거나 시작 좌표인 경우
            UnitBase currentUnit = _unitMap[currentCoord.x, currentCoord.y];
            if (!currentUnit || currentCoord == startCoord)
            {
                // _tileMap[currentCoord.x, currentCoord.y].ShowDistance(currentTileCost.Distance);
                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();

                if (currentTileCost.Distance <= move) _reachableTiles.Add(currentCoord);
            }

            _visitedMap[currentCoord.x, currentCoord.y] = true;

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int nextCoord = currentCoord + direction;

                if (nextCoord.x < 0 || nextCoord.y < 0
                    || nextCoord.x >= _tileMap.GetLength(1) || nextCoord.y >= _tileMap.GetLength(0)
                    || _visitedMap[nextCoord.x, nextCoord.y]) continue;

                TileData nextTileData = _tileMap[nextCoord.x, nextCoord.y].TileData;

                // 도착할 수 없는 타일인 경우
                if (nextTileData.Cost == int.MaxValue
                    || _unitMap[nextCoord.x, nextCoord.y]?.UnitType == targetUnitType)
                {
                    _visitedMap[nextCoord.x, nextCoord.y] = true;
                    continue;
                }

                TileCost nextTileCost = _tileCostMap[nextCoord.x, nextCoord.y];
                if (nextTileCost.Distance <= currentTileCost.Distance + nextTileData.Cost) continue;

                nextTileCost.SetData(
                    nextTileData,
                    currentTileCost,
                    currentTileCost.Distance + nextTileData.Cost
                    );

                if (!_priorityQueueIndex.ContainsKey(nextTileCost)) PriorityQueueAdd(nextTileCost);
                else PriorityQueueCompareWithParent(nextTileCost);
            }
        }

        return _tileCostMap;
    }

    public TileCost[,] SelectTargetWeaponRange(Vector2Int startCoord, int minRange, int maxRange, bool showRange = true, List<Vector2Int> coordList = null)
    {
        Init();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        PriorityQueueAdd(startTileCost);

        while (_priorityQueue.Count > 0)
        {
            TileCost currentTileCost = PriorityQueueRemove();
            Vector2Int currentCoord = currentTileCost.Coord;

            if (currentTileCost.Distance > maxRange) break;

            if (currentTileCost.Distance >= minRange
                && currentTileCost.Distance <= maxRange)
            {
                if (showRange) _tileMap[currentCoord.x, currentCoord.y].ShowDistance(currentTileCost.Distance);
                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();

                if (_unitMap[currentCoord.x, currentCoord.y] != null)
                    coordList.Add(currentCoord);
            }
                
            _visitedMap[currentCoord.x, currentCoord.y] = true;

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int nextCoord = currentCoord + direction;

                if (nextCoord.x < 0 || nextCoord.y < 0
                    || nextCoord.x >= _tileMap.GetLength(1) || nextCoord.y >= _tileMap.GetLength(0)
                    || _visitedMap[nextCoord.x, nextCoord.y]) continue;

                TileData nextTileData = _tileMap[nextCoord.x, nextCoord.y].TileData;

                TileCost nextTileCost = _tileCostMap[nextCoord.x, nextCoord.y];
                if (nextTileCost.Distance <= currentTileCost.Distance + 1) continue;

                nextTileCost.SetData(
                    nextTileData,
                    currentTileCost,
                    currentTileCost.Distance + 1
                    );

                if (!_priorityQueueIndex.ContainsKey(nextTileCost)) PriorityQueueAdd(nextTileCost);
                else PriorityQueueCompareWithParent(nextTileCost);
            }
        }

        return _tileCostMap;
    }

    public TileCost CalculateTileCost(Vector2Int startCoord, Vector2Int endCoord, UnitType targetUnitType)
    {
        Init();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        PriorityQueueAdd(startTileCost);

        while (_priorityQueue.Count > 0)
        {
            TileCost currentTileCost = PriorityQueueRemove();
            Vector2Int currentCoord = currentTileCost.Coord;

            if (currentCoord == endCoord)
            {
                return currentTileCost;    
            }
            
            if (!_unitMap[currentCoord.x, currentCoord.y])
            {
                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();
            }

            _visitedMap[currentCoord.x, currentCoord.y] = true;

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int nextCoord = currentCoord + direction;

                if (nextCoord.x < 0 || nextCoord.y < 0
                    || nextCoord.x >= _tileMap.GetLength(1) || nextCoord.y >= _tileMap.GetLength(0)
                    || _visitedMap[nextCoord.x, nextCoord.y]) continue;

                TileData nextTileData = _tileMap[nextCoord.x, nextCoord.y].TileData;

                // 도착할 수 없는 타일인 경우
                if (nextCoord != endCoord
                    && (nextTileData.Cost == int.MaxValue
                    || _unitMap[nextCoord.x, nextCoord.y]?.UnitType == targetUnitType))
                {
                    _visitedMap[nextCoord.x, nextCoord.y] = true;
                    continue;
                }

                TileCost nextTileCost = _tileCostMap[nextCoord.x, nextCoord.y];
                if (nextTileCost.Distance <= currentTileCost.Distance + nextTileData.Cost) continue;

                nextTileCost.SetData(
                    nextTileData,
                    currentTileCost,
                    currentTileCost.Distance + nextTileData.Cost
                    );

                if (!_priorityQueueIndex.ContainsKey(nextTileCost)) PriorityQueueAdd(nextTileCost);
                else PriorityQueueCompareWithParent(nextTileCost);
            }
        }

        Debug.Log("Could not reach Tile");
        return null;
    }

    // 상대 유닛의 이웃한 타일까지의 거리
    public int GetUnreachableDistance(Vector2Int coord)
    {
        int distance = int.MaxValue;

        foreach (Vector2Int direction in _directions)
        {
            Vector2Int neighborCoord = coord + direction;

            if (neighborCoord.x < 0 || neighborCoord.y < 0
                || neighborCoord.x >= _unitMap.GetLength(0) || neighborCoord.y >= _unitMap.GetLength(1))
                continue;

            int neighborDistance = _tileCostMap[neighborCoord.x, neighborCoord.y].Distance;
            if (neighborDistance < distance) distance = neighborDistance;
        }
        return distance;
    }

    /// <summary>
    /// MinHeap에 넣기
    /// </summary>
    private void PriorityQueueAdd(TileCost tileCost)
    {
        _priorityQueue.Add(tileCost);
        int index = _priorityQueue.Count - 1;
        _priorityQueueIndex[tileCost] = index;

        PriorityQueueCompareWithParent(tileCost);
    }

    /// <summary>
    /// MinHeap에서 루트 꺼내기
    /// </summary>
    private TileCost PriorityQueueRemove()
    {

        if (_priorityQueue.Count == 0) return null;

        TileCost result = _priorityQueue[0];

        _priorityQueue[0] = _priorityQueue[_priorityQueue.Count - 1];
        _priorityQueue.RemoveAt(_priorityQueue.Count - 1);

        int index = 0;

        while (true)
        {
            int leftChildIndex = index * 2 + 1;
            int rightChildIndex = index * 2 + 2;
            int smallestIndex = index;

            if (leftChildIndex < _priorityQueue.Count
                && _priorityQueue[leftChildIndex].Distance < _priorityQueue[smallestIndex].Distance)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex < _priorityQueue.Count
                && _priorityQueue[rightChildIndex].Distance < _priorityQueue[smallestIndex].Distance)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index) break;

            TileCost temp = _priorityQueue[smallestIndex];

            _priorityQueue[smallestIndex] = _priorityQueue[index];
            _priorityQueue[index] = temp;

            _priorityQueueIndex[_priorityQueue[smallestIndex]] = smallestIndex;
            _priorityQueueIndex[_priorityQueue[index]] = index;

            index = smallestIndex;
        }

        return result;
    }

    private void PriorityQueueCompareWithParent(TileCost tileCost)
    {
        int index = _priorityQueueIndex[tileCost];

        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (_priorityQueue[index].Distance >= _priorityQueue[parentIndex].Distance) break;

            TileCost temp = _priorityQueue[parentIndex];
            _priorityQueue[parentIndex] = _priorityQueue[index];
            _priorityQueue[index] = temp;

            _priorityQueueIndex[_priorityQueue[parentIndex]] = parentIndex;
            _priorityQueueIndex[_priorityQueue[index]] = index;

            index = parentIndex;
        }
    }
}
