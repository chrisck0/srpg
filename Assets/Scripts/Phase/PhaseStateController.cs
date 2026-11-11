using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
public class PhaseStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private UnitCommandStateController _unitCommandStateController;
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private GameUIController _gameUIController;
    [SerializeField] private Button _endTurnButton;

    private StateBase _playerPhaseState;
    private StateBase _enemyPhaseState;
    private int _turn;
    private int _initialPlayerNum;
    private int _initialEnemyNum;
    private List<UnitBase> _playerUnitList;
    private List<UnitBase> _enemyUnitList;

    public event Action OnGameEnd;

    public Action<StateBase> OnStateChanged { get; set; }
    public event Action OnPlayerPhaseStarted;
    public event Action OnEnemyPhaseStarted;
    public MapManager MapManager => _mapManager;
    public int Turn => _turn;
    public int TotalPlayerUnits => _playerUnitList.Count;
    public int TotalEnemyUnits => _enemyUnitList.Count;

    public StateBase PlayerPhaseState => _playerPhaseState;
    public StateBase EnemyPhaseState => _enemyPhaseState;

    private void OnEnable()
    {
        BindButtonEvents();
        BindSpawnEvents();
        InitUI();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();
        UnbindSpawnEvents();
    }

    private void InitUI()
    {
        _gameUIController.DeactivateAll();
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

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;
        _playerUnitList = playerUnitList;
        _initialPlayerNum = playerUnitList.Count;

        foreach (UnitBase playerUnitBase in _playerUnitList)
        {
            PlayerUnit playerUnit = playerUnitBase as PlayerUnit;

            if (playerUnit == null) continue;
            BindUnitEvents(playerUnit);
        }

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;
        _enemyUnitList = enemyUnitList;
        _initialEnemyNum = enemyUnitList.Count;

        foreach (UnitBase enemyUnitBase in _enemyUnitList)
        {
            EnemyUnit enemyUnit = enemyUnitBase as EnemyUnit;

            if (enemyUnit == null) continue;
            BindUnitEvents(enemyUnit);
            enemyUnit.EnemyAI.SetData(_mapManager);
        }
    }

    public void StartPlayerPhase()
    {
        WaitPlayerPhaseUI().Forget();
    }

    public void StartEnemyPhase()
    {
        WaitEnemyPhaseUI().Forget();
    }

    private async UniTask WaitPlayerPhaseUI()
    {
        _turn++;
        _gameUIController.PhaseUI.SetPlayerPhaseText();
        _gameUIController.PhaseUI.Show();

        await UniTask.Delay(3000, DelayType.Realtime);

        _gameUIController.PhaseUI.Hide();
        SetState(_playerPhaseState);
        OnPlayerPhaseStarted?.Invoke();
    }

    private async UniTask WaitEnemyPhaseUI()
    {
        _gameUIController.PhaseUI.SetEnemyPhaseText();
        _gameUIController.PhaseUI.Show();

        await UniTask.Delay(3000, DelayType.Realtime);

        _gameUIController.PhaseUI.Hide();
        SetState(_enemyPhaseState);
        OnEnemyPhaseStarted?.Invoke();
    }

    private void UnitDead(UnitBase unit)
    {
        Debug.Log($"{unit.UnitStat.UnitData.Name} Died");
        switch (unit.UnitType)
        {
            case UnitType.Player:
                _playerUnitList.Remove(unit);
                break;

            case UnitType.Enemy:
                _enemyUnitList.Remove(unit);
                break;
        }

        Vector2Int coord = unit.transform.position.WorldToCoord();
        _mapManager.UnitMap[coord.x, coord.y] = null;

        if (_playerUnitList.Count == 0) ShowGameResult(false);
        if (CheckGameWin(unit)) ShowGameResult(true);
        UnbindUnitEvents(unit);
    }

    private bool CheckGameWin(UnitBase unit)
    {
        switch(_spawnManager.StageData.StageWinCondition)
        {
            case StageWinCondition.Rout:
                if (_enemyUnitList.Count == 0) return true;
                break;

            case StageWinCondition.Boss:
                if (_spawnManager.StageData.TargetIds[0] == unit.UnitStat.UnitData.Id) return true;
                break;
        }

        return false;
    }

    private void ShowGameResult(bool isGameWin)
    {
        _gameUIController.ResultUI.SetData(isGameWin, _turn,
            _initialEnemyNum - _enemyUnitList.Count, _initialPlayerNum - _playerUnitList.Count);
        _stateMachine.enabled = false;
        _mapManager.TileRange.InitTiles();
        OnGameEnd?.Invoke();
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

    private void BindUnitEvents(UnitBase unit)
    {
        unit.OnUnitDead += UnitDead;
    }

    private void UnbindUnitEvents(UnitBase unit)
    {
        unit.OnUnitDead -= UnitDead;
    }
}
