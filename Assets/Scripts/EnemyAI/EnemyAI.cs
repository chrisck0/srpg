using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyUnit _owner;
    private AttackScoreCalculator _calculator;
    private BehaviorType _behaviorType;
    private MapManager _mapManager;
    private TileCost[,] _tileCostMap;
    private HashSet<Vector2Int> _reachableTiles;
    private Stack<Vector2Int> _pathCoordStack = new();
    private AttackScore _maxAttackScore;
    private bool _isMoving;
    private bool _canAttack;
    private Vector2Int _moveTargetCoord;
    private List<Vector2Int> _attackTargetCoords = new();

    private void Awake() => Init();

    private void Update()
    {
        if (_isMoving) MoveUnit();
        else if (_canAttack) Attack();
    }

    public void SetData(MapManager mapManager)
    {
        _mapManager = mapManager;
    }

    public void Behave()
    {
        Vector2Int currentCoord = transform.position.WorldToCoord();

        _maxAttackScore = _calculator.CalculateScore(_mapManager, currentCoord);

        Debug.Log($"Attack Score {_maxAttackScore.CurrentCoord} {_maxAttackScore.AttackWeaponData} " +
            $"{_maxAttackScore.WeaponDirection} {_maxAttackScore.TargetCoord} {_maxAttackScore.Score}");

        _tileCostMap = _mapManager.TileRange.UnitMoveRange(
            currentCoord, _owner.UnitStat.UnitData.Move, UnitType.Player);

        if (_maxAttackScore.Score == 0)
        {
            MoveToClosestTarget();
        }
        else
        {
            MoveToDestination(currentCoord, _maxAttackScore.CurrentCoord);
            _canAttack = true;
        }
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

        _moveTargetCoord = currentCoord;
    }

    private void MoveUnit()
    {
        if (Vector3.Distance(transform.position, _moveTargetCoord.CoordToWorld()) >= 0.1f)
        {
            transform.Translate(Vector3.forward
                * Time.deltaTime * _owner.MoveSpeed);
            return;
        }

        if (_pathCoordStack.Count == 0)
        {
            _mapManager.UnitMap[_moveTargetCoord.x, _moveTargetCoord.y] = _owner;
            _isMoving = false;
            _owner.OnEnemyActioned?.Invoke();
            return;
        }

        _moveTargetCoord = _pathCoordStack.Pop();
        transform.LookAt(_moveTargetCoord.CoordToWorld());
        transform.Translate(Vector3.forward
            * Time.deltaTime * _owner.MoveSpeed);
    }

    private void Attack()
    {
        Debug.Log("Enemy Attacked");
    }

    private void Init()
    {
        _calculator = new AttackScoreCalculator(_owner);
    }
}
