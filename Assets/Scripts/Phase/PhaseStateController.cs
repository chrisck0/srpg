using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public sealed class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private List<PlayerUnit> _playerUnitList;
    [SerializeField] private List<EnemyUnit> _enemyUnitList;
    [SerializeField] private TestPhaseTransition _testPhaseTransition;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;
    private PhaseType _phaseType;

    public Action<StateBase> OnStateChanged { get; set; }

    public List<PlayerUnit> PlayerUnitList => _playerUnitList;
    public List<EnemyUnit> EnemyUnitList => _enemyUnitList;
    public TestPhaseTransition TestPhaseTransition => _testPhaseTransition;
    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;
    public PhaseType PhaseType => _phaseType;

    private void Awake() => InitPhaseStates();
    private void Start() => StartPlayerPhase();

    private void InitPhaseStates()
    {
        _playerPhaseState = new PlayerPhaseState(this);
        _enemyPhaseState = new EnemyPhaseState(this);
    }

    private void StartPlayerPhase()
    {
        SetState(_playerPhaseState);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }

    public void SetPhaseType(PhaseType phaseType)
    {
        _phaseType = phaseType;
    }
}
