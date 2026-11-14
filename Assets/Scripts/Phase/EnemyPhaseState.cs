using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyPhaseState : StateBase
{
    private PhaseStateController _controller;
    private List<UnitBase> _enemyUnitList;
    private Queue<EnemyUnit> _activeEnemyUnit = new();
    private EnemyUnit _currentEnemyUnit;
    private int _currentTurnCount;
    private bool _isStateEnd;

    public EnemyPhaseState(PhaseStateController controller, Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _controller = controller;

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;

        _enemyUnitList = enemyUnitList;
    }

    public override void Enter()
    {
        _isStateEnd = false;
        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            enemyUnit.UnitStat.AddEnergy(5);
            enemyUnit.EnemyAI.InitDanger();
            _activeEnemyUnit.Enqueue(enemyUnit);
        }
    }

    public override void Update()
    {
        if (_isStateEnd) return;

        if (_currentEnemyUnit == null)
        {
            if (_activeEnemyUnit.Count != 0)
            {
                _currentTurnCount++;
                _currentEnemyUnit = _activeEnemyUnit.Dequeue();

                Debug.Log($"Current turn : {_currentEnemyUnit.UnitStat.UnitData.Name}");

                BindEnemyUnitEvents();

                _currentEnemyUnit.EnemyAI.Behave(
                    _controller.Turn,
                    _controller.TotalEnemyUnits,
                    _currentTurnCount
                    );
            }
            else
            {
                EnemyPhaseEnd();
                _isStateEnd = true;
                _currentTurnCount = 0;
            }
        }
    }

    public override void Exit()
    {
        _activeEnemyUnit.Clear();
        _currentEnemyUnit = null;

        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            enemyUnit.OnUnitCanAction?.Invoke();

            if (!enemyUnit.EnemyAI.IsShowingDanger) continue;

            enemyUnit.EnemyAI.CalculateScore();
            enemyUnit.EnemyAI.ShowDanger();
        }
    }

    private void EnemyPhaseEnd()
    {
        _controller.StartPlayerPhase();
    }

    private void EnemyActioned()
    {
        UnbindEnemyUnitEvents();
        WaitForSeconds().Forget();
    }

    private async UniTask WaitForSeconds()
    {
        await UniTask.Delay(1000, DelayType.DeltaTime);
        _currentEnemyUnit = null;
    }

    private void BindEnemyUnitEvents()
    {
        _currentEnemyUnit.OnUnitActioned += EnemyActioned;
    }

    private void UnbindEnemyUnitEvents()
    {
        _currentEnemyUnit.OnUnitActioned -= EnemyActioned;
    }
}
