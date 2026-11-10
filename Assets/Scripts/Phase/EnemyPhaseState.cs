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
            enemyUnit.UnitStat.AddEnergy(5);
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

                BindEnemyUnitEvents();

                _currentEnemyUnit.EnemyAI.Behave();
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
        _phaseStateController.StartPlayerPhase();
    }

    private void EnemyActioned()
    {
        UnbindEnemyUnitEvents();
        _currentEnemyUnit = null;
    }

    private void BindEnemyUnitEvents()
    {
        _currentEnemyUnit.OnEnemyActioned += EnemyActioned;
    }

    private void UnbindEnemyUnitEvents()
    {
        _currentEnemyUnit.OnEnemyActioned -= EnemyActioned;
    }
}
