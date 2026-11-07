using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;
    private List<EnemyUnit> _enemyUnitList;
    private Queue<EnemyUnit> _activeEnemyUnit = new();

    private EnemyUnit _currentEnemyUnit;

    public EnemyPhaseState(PhaseStateController phaseStateController)
    {
        _phaseStateController = phaseStateController;
        _enemyUnitList = phaseStateController.EnemyUnitList;

        // TODO: Unit 소환하는 클래스로 옮기기
        Dictionary<int, WeaponData> weaponDataDict = _phaseStateController.DataManager.WeaponDataDict;

        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            foreach (int weaponId in enemyUnit.UnitStat.UnitData.WeaponIdList)
            {
                if (weaponDataDict.TryGetValue(weaponId, out WeaponData weaponData))
                {
                    enemyUnit.UnitStat.AddWeapon(weaponData);
                }
            }
        }
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
