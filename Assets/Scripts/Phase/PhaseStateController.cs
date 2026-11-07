using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private Button _endTurnButton;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;
    private PhaseType _phaseType;

    public Action<StateBase> OnStateChanged { get; set; }

    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;
    public PhaseType PhaseType => _phaseType;

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
        _playerPhaseState = new PlayerPhaseState(this, spawnedUnitDict);
        _enemyPhaseState = new EnemyPhaseState(this, spawnedUnitDict);
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

    private void BindSpawnEvents()
    {
        _spawnManager.OnStageLoaded += StartPhase;
    }

    private void UnbindSpawnEvents()
    {
        _spawnManager.OnStageLoaded -= StartPhase;
    }
}
