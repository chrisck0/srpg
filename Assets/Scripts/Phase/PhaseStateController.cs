using System;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public sealed class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private TestPhaseTransition _testPhaseTransition;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;

    public Action<StateBase> OnStateChanged { get; set; }

    public TestPhaseTransition TestPhaseTransition => _testPhaseTransition;
    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;

    private void Awake() => InitPhaseStates();
    private void Start() => StartPlayerPhase();

    private void InitPhaseStates()
    {
        _playerPhaseState = new PlayerPhaseState(this);
        _enemyPhaseState = new EnemyPhaseState(this);
    }

    private void StartPlayerPhase()
    {
        OnStateChanged?.Invoke(_playerPhaseState);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }
}
