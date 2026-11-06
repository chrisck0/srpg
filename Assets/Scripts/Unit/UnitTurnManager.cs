using UnityEngine;
using System.Collections.Generic;

public class UnitTurnManager : MonoBehaviour
{
    [SerializeField] private List<PlayerUnit> _playerUnitList;
    [SerializeField] private List<EnemyUnit> _enemyUnitList;
    [SerializeField] private PhaseStateController _phaseStatController;
    private int currentTurn;
    private Queue<EnemyUnit> _activeEnemyUnit = new();
    private Queue<EnemyUnit> _inactiveEnemyUnit = new();

    private void OnEnable() => BindPhaseEvents();
    private void OnDisable() => UnbindPhaseEvents();

    private void PhaseChanged(StateBase stateBase)
    {
        
    }

    private void BindPhaseEvents()
    {
        _phaseStatController.OnStateChanged += PhaseChanged;
    }

    private void UnbindPhaseEvents()
    {
        _phaseStatController.OnStateChanged -= PhaseChanged;
    }
}
