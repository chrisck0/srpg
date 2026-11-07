using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private List<PlayerUnit> _playerUnitList;
    [SerializeField] private List<EnemyUnit> _enemyUnitList;
    [SerializeField] private Button _endTurnButton;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;
    private PhaseType _phaseType;

    public Action<StateBase> OnStateChanged { get; set; }

    public DataManager DataManager => _dataManager;
    public List<PlayerUnit> PlayerUnitList => _playerUnitList;
    public List<EnemyUnit> EnemyUnitList => _enemyUnitList;
    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;
    public PhaseType PhaseType => _phaseType;

    private void OnEnable() => BindButtonEvents();
    private void Start()
    {
        InitPhaseStates();
        StartPlayerPhase();
    }

    private void OnDisable() => UnbindButtonEvents();

    private void InitPhaseStates()
    {
        _playerPhaseState = new PlayerPhaseState(this);
        _enemyPhaseState = new EnemyPhaseState(this);
    }

    private void StartPlayerPhase()
    {
        SetState(_playerPhaseState);
    }

    private void StartEnemyPhase()
    {
        SetState(_enemyPhaseState);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }

    public void SetPhaseType(PhaseType phaseType)
    {
        _phaseType = phaseType;
    }
    
    private void BindButtonEvents()
    {
        _endTurnButton.onClick.AddListener(StartEnemyPhase);
    }
    
    private void UnbindButtonEvents()
    {
        _endTurnButton.onClick.RemoveListener(StartEnemyPhase);
    }
}
