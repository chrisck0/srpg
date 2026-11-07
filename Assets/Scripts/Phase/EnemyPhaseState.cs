using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;
    private List<UnitBase> _enemyUnitList;
    private Queue<EnemyUnit> _activeEnemyUnit = new();
    private EnemyUnit _currentEnemyUnit;

    public EnemyPhaseState(PhaseStateController phaseStateController, Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _phaseStateController = phaseStateController;

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;

        _enemyUnitList = enemyUnitList;
    }

    public override void Enter()
    {
        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            _activeEnemyUnit.Enqueue(enemyUnit);
        }
        Debug.Log("EnemyPhase Start");
    }

    public override void Update()
    {
        if (_currentEnemyUnit == null)
        {
            if (_activeEnemyUnit.Count != 0)
            {
                _currentEnemyUnit = _activeEnemyUnit.Dequeue();

                Debug.Log($"Current turn : {_currentEnemyUnit.UnitStat.UnitData.Name}");

                _currentEnemyUnit.BindInputEvents();
                BindPlayerUnitEvents();
            }
            else
            {
                EnemyPhaseEnd();
            }
        }
    }

    public override void Exit()
    {
        _activeEnemyUnit.Clear();
        _currentEnemyUnit = null;
    }

    private void EnemyPhaseEnd()
    {
        _phaseStateController.SetState(_phaseStateController.PlayerPhaseState);
    }

    private void PlayerActioned()
    {
        UnbindPlayerUnitEvents();
        _currentEnemyUnit.UnbindInputEvents();
        _currentEnemyUnit = null;
    }

    private void BindPlayerUnitEvents()
    {
        _currentEnemyUnit.OnEnemyActioned += PlayerActioned;
    }

    private void UnbindPlayerUnitEvents()
    {
        _currentEnemyUnit.OnEnemyActioned -= PlayerActioned;
    }
}
