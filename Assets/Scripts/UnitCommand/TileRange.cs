using System.Collections.Generic;
using UnityEngine;

public class TileRange
{
    private PriorityQueue _priorityQueue;
    private TileBase[,] _tileMap;
    private UnitBase[,] _unitMap;
    private bool[,] _visitedMap;
    private TileCost[,] _tileCostMap;
    private Vector2Int[] _directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public TileCost[,] TileCostMap => _tileCostMap;

    public TileRange(TileBase[,] tileMap, UnitBase[,] unitMap)
    {
        _priorityQueue = new PriorityQueue();

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

    // 다익스트라 관련 초기화
    private void Init()
    {
        for (int i = 0; i < _tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < _tileMap.GetLength(1); j++)
            {
                _visitedMap[j, i] = false;
                _tileCostMap[j, i].InitData();
                _tileMap[j, i].InitSelect();
            }
        }

        _priorityQueue.Clear();
    }

    /// <summary>
    /// 이동 가능 위치 표시 및 이동 가능 거리까지만 계산
    /// </summary>
    public TileCost[,] UnitMoveRange(UnitBase unit, Vector2Int startCoord)
    {
        Init();
        unit.DistanceRange.DistanceCoords.Clear();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        _priorityQueue.Add(startTileCost);

        while (_priorityQueue.Count() > 0)
        {
            TileCost currentTileCost = _priorityQueue.Remove();
            Vector2Int currentCoord = currentTileCost.Coord;

            if (currentTileCost.Distance > unit.Stat.Data.Move) break;


            // 유닛이 좌표 위에 없거나 시작 좌표인 경우
            UnitBase currentUnit = _unitMap[currentCoord.x, currentCoord.y];
            if (!currentUnit || currentCoord == startCoord)
            {
                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();
            }

            _visitedMap[currentCoord.x, currentCoord.y] = true;

            unit.DistanceRange.DistanceCoords.Add(currentCoord);

            foreach (Vector2Int direction in _directions)
            {
                Vector2Int nextCoord = currentCoord + direction;

                if (nextCoord.x < 0 || nextCoord.y < 0
                    || nextCoord.x >= _tileMap.GetLength(1) || nextCoord.y >= _tileMap.GetLength(0)
                    || _visitedMap[nextCoord.x, nextCoord.y]) continue;

                TileData nextTileData = _tileMap[nextCoord.x, nextCoord.y].TileData;

                // 도착할 수 없는 타일인 경우
                if (nextTileData.Cost == int.MaxValue
                    || _unitMap[nextCoord.x, nextCoord.y] != null && _unitMap[nextCoord.x, nextCoord.y]?.UnitType != unit.UnitType)
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

                if (!_priorityQueue.PriorityQueueIndex.ContainsKey(nextTileCost))
                    _priorityQueue.Add(nextTileCost);
                else _priorityQueue.CompareWithParent(nextTileCost);
            }
        }

        return _tileCostMap;
    }

    /// <summary>
    /// 이동 가능 위치 표시 및 맵 전체 거리 계산
    /// </summary>
    public TileCost[,] UnitMoveRangeAndCalculateTotal(UnitBase unit, Vector2Int startCoord)
    {
        Init();
        unit.DistanceRange.ReachableCoords.Clear();
        unit.DistanceRange.DistanceCoords.Clear();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        _priorityQueue.Add(startTileCost);

        while (_priorityQueue.Count() > 0)
        {
            TileCost currentTileCost = _priorityQueue.Remove();
            Vector2Int currentCoord = currentTileCost.Coord;

            // 유닛이 좌표 위에 없거나 시작 좌표인 경우
            UnitBase currentUnit = _unitMap[currentCoord.x, currentCoord.y];
            if (!currentUnit || currentCoord == startCoord)
            {
                if (currentTileCost.Distance <= unit.Stat.Data.Move)
                {
                    _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();
                    unit.DistanceRange.ReachableCoords.Add(currentCoord);
                    unit.DistanceRange.DistanceCoords.Add(currentCoord);
                }
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
                    || _unitMap[nextCoord.x, nextCoord.y] != null && _unitMap[nextCoord.x, nextCoord.y]?.UnitType != unit.UnitType)
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

                if (!_priorityQueue.PriorityQueueIndex.ContainsKey(nextTileCost))
                    _priorityQueue.Add(nextTileCost);
                else _priorityQueue.CompareWithParent(nextTileCost);
            }
        }

        return _tileCostMap;
    }

    /// <summary>
    /// 타겟 선택 무기 내 범위의 좌표 저장
    /// </summary>
    public void SelectTargetWeaponRange(UnitBase unit, Vector2Int startCoord, int minRange, int maxRange, List<Vector2Int> coordList, bool isRange)
    {
        Init();
        coordList.Clear();

        if (!isRange) unit.DistanceRange.DistanceCoords.Clear();
        else unit.DistanceRange.RangeCoords.Clear();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        _priorityQueue.Add(startTileCost);

        while (_priorityQueue.Count() > 0)
        {
            TileCost currentTileCost = _priorityQueue.Remove();
            Vector2Int currentCoord = currentTileCost.Coord;

            if (currentTileCost.Distance > maxRange) break;

            if (currentTileCost.Distance >= minRange
                && currentTileCost.Distance <= maxRange)
            {
                if (!isRange) unit.DistanceRange.DistanceCoords.Add(currentCoord);
                else unit.DistanceRange.RangeCoords.Add(currentCoord);

                _tileMap[currentCoord.x, currentCoord.y].SetCanSelect();

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

                if (!_priorityQueue.PriorityQueueIndex.ContainsKey(nextTileCost))
                    _priorityQueue.Add(nextTileCost);
                else _priorityQueue.CompareWithParent(nextTileCost);
            }
        }
    }

    /// <summary>
    /// 시작과 끝 좌표의 거리를 계산
    /// </summary>
    public TileCost CalculateTileCost(Vector2Int startCoord, Vector2Int endCoord, UnitType targetUnitType)
    {
        Init();

        TileCost startTileCost = _tileCostMap[startCoord.x, startCoord.y];
        startTileCost.SetData(_tileMap[startCoord.x, startCoord.y].TileData, null, 0);

        _priorityQueue.Add(startTileCost);

        while (_priorityQueue.Count() > 0)
        {
            TileCost currentTileCost = _priorityQueue.Remove();
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

                if (!_priorityQueue.PriorityQueueIndex.ContainsKey(nextTileCost))
                    _priorityQueue.Add(nextTileCost);
                else _priorityQueue.CompareWithParent(nextTileCost);
            }
        }

        Debug.Log("Could not reach Tile");
        return null;
    }

    /// <summary>
    /// 상대 유닛의 이웃한 타일까지의 거리
    /// </summary>
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
}
