using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyUnit _owner;
    private BehaviorType _behaviorType;
    private MapManager _mapManager;
    private TileCost[,] _tileCostMap;
    private HashSet<Vector2Int> _reachableTiles;
    private Stack<Vector2Int> _pathCoordStack = new();
    private Vector2Int _nextCoord;
    private bool _isMoving;
    private Vector2Int _targetCoord;

    private void Update()
    {
        if (_isMoving) MoveUnit();
    }

    public void SetData(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    public void Behave()
    {
        int score = CalculateScore();

        if (score == 0)
        {
            MoveToClosestTarget();
        }
    }

    private int CalculateScore()
    {
        _tileCostMap = _mapManager.TileRange.UnitMoveRangeAndCalculateTotal(
            transform.position.WorldToCoord(), _owner.UnitStat.UnitData.Move, UnitType.Player);
        _reachableTiles = _mapManager.TileRange.ReachableTiles;
        Debug.Log(_reachableTiles.Count);

        int maxScore = 0;
        Vector2Int resultCoord = Vector2Int.zero;

        foreach (Vector2Int coord in _reachableTiles)
        {
            int score = CalculateAttackScore(coord);
            if (score > maxScore)
            {
                maxScore = score;
                resultCoord = coord;
            }
        }

        return maxScore;
    }

    private void MoveToClosestTarget()
    {
        int currentToTargetMinDistance = int.MaxValue;
        Vector2Int targetCoord = Vector2Int.zero;

        for (int i = 0; i < _tileCostMap.GetLength(1); i++)
        {
            for (int j = 0; j < _tileCostMap.GetLength(0); j++)
            {
                UnitBase unit = _mapManager.UnitMap[j, i];
                if (unit == null || unit.UnitType != UnitType.Player) continue;

                Vector2Int newCoord = new Vector2Int(j, i);
                int newDistance = _mapManager.TileRange.GetUnreachableDistance(newCoord);
                if (newDistance < currentToTargetMinDistance)
                {
                    currentToTargetMinDistance = newDistance;
                    targetCoord = newCoord;
                }
            }
        }

        int destinationToTargetMinDistance = int.MaxValue;
        Vector2Int destinationCoord = Vector2Int.zero;

        foreach (Vector2Int coord in _reachableTiles)
        {
            TileCost tileCost = _mapManager.TileRange.CalculateTileCost(coord, targetCoord, UnitType.Player);
            if (destinationToTargetMinDistance > tileCost.Distance)
            {
                destinationToTargetMinDistance = tileCost.Distance;
                destinationCoord = coord;
            }
        }

        Vector2Int currentCoord = transform.position.WorldToCoord();
        _mapManager.TileRange.CalculateTileCost(currentCoord, destinationCoord, UnitType.Player);
        MoveToDestination(transform.position.WorldToCoord(), destinationCoord);
    }

    private void MoveToDestination(Vector2Int currentCoord, Vector2Int destinationCoord)
    {
        _isMoving = true;

        _mapManager.UnitMap[currentCoord.x, currentCoord.y] = null;

        Vector2Int pathCoord = destinationCoord;

        int distance = VectorExtensions.CoordDistance(currentCoord, destinationCoord);

        int cnt = distance;

        while (pathCoord != currentCoord)
        {
            _pathCoordStack.Push(pathCoord);

            pathCoord = _tileCostMap[pathCoord.x, pathCoord.y].PrevTileCost.Coord;
        }

        _targetCoord = currentCoord;
    }

    private void MoveUnit()
    {
        if (Vector3.Distance(transform.position, _targetCoord.CoordToWorld()) >= 0.1f)
        {
            transform.Translate(Vector3.forward
                * Time.deltaTime * _owner.MoveSpeed);
            return;
        }

        if (_pathCoordStack.Count == 0)
        {
            _mapManager.UnitMap[_targetCoord.x, _targetCoord.y] = _owner;
            _isMoving = false;
            _owner.OnEnemyActioned?.Invoke();
            return;
        }

        _targetCoord = _pathCoordStack.Pop();
        transform.LookAt(_targetCoord.CoordToWorld());
        transform.Translate(Vector3.forward
            * Time.deltaTime * _owner.MoveSpeed);
    }

    private int CalculateAttackScore(Vector2Int targetCoord)
    {
        return 0;
    }

    private void Attack()
    {

    }
}
