using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private Button _endTurnButton;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;

    public Action<StateBase> OnStateChanged { get; set; }
    public event Action OnPlayerPhaseStarted;
    public event Action OnEnemyPhaseStarted;

    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;

    private void OnEnable()
    {
        BindButtonEvents();
        BindSpawnEvents();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();
        UnbindSpawnEvents();
    }

    private void StartPhase()
    {
        InitPhaseStates(_spawnManager.SpawnedUnitDict);
        StartPlayerPhase();
    }

    private void InitPhaseStates(Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _playerPhaseState = new PlayerPhaseState(this, _unitCommandStateController, spawnedUnitDict);
        _enemyPhaseState = new EnemyPhaseState(this, spawnedUnitDict);
    }

    public void StartPlayerPhase()
    {
        SetState(_playerPhaseState);
        OnPlayerPhaseStarted?.Invoke();
    }

    public void StartEnemyPhase()
    {
        SetState(_enemyPhaseState);
        OnEnemyPhaseStarted?.Invoke();
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }
    
    private void BindButtonEvents()
    {
        _endTurnButton.onClick.AddListener(StartEnemyPhase);
    }
    
    private void UnbindButtonEvents()
    {
        _endTurnButton.onClick.RemoveListener(StartEnemyPhase);
    }

    private void BindSpawnEvents()
    {
        _spawnManager.OnStageLoaded += StartPhase;
    }

    private void UnbindSpawnEvents()
    {
        _spawnManager.OnStageLoaded -= StartPhase;
    }
}
