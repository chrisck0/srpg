using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class UnitCommandStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileCursor _tileCursor;
    [SerializeField] private CommandUIController _commandUIController;
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private InputActionReference _infoAction;
    [SerializeField] private InputActionReference _cancelAction;
    [SerializeField] private InputActionReference _cursorPosition;
    [SerializeField] private LayerMask _ground;

    private StateBase _unitSelectState;
    private StateBase _unitMoveState;
    private StateBase _unitCommandState;
    private StateBase _targetSelectState;
    private StateBase _commandEndState;
    private PlayerUnit _selectedPlayerUnit;
    private Vector2Int _previousPlayerCoord;
    private WeaponData _selectedWeaponData;
    private int _currentTurnCount;
    private List<EnemyUnit> _showingDangerEnemyList = new();

    public Action<StateBase> OnStateChanged { get; set; }
    public Action<PlayerUnit> OnUnitCommandEnd;

    public MapManager MapManager => _mapManager;
    public StateBase UnitSelectState => _unitSelectState;
    public StateBase UnitMoveState => _unitMoveState;
    public StateBase UnitCommandState => _unitCommandState;
    public StateBase TargetSelectState => _targetSelectState;
    public StateBase CommandEndState => _commandEndState;
    public PlayerUnit SelectedPlayerUnit => _selectedPlayerUnit;
    public Vector2Int PreviousPlayerCoord => _previousPlayerCoord;
    public WeaponData SelectedWeaponData => _selectedWeaponData;
    public int Turn => _phaseStateController.Turn;
    public int TotalPlayerUnits => _phaseStateController.TotalPlayerUnits;
    public int CurrentTurnCount => _currentTurnCount;
    public List<EnemyUnit> ShowingDangerEnemyList => _showingDangerEnemyList;

    private void Awake() => InitUnitCommandStates();
    private void OnEnable() => BindPhaseEvents();
    private void Start() => InitUI();
    private void OnDisable() => UnbindPhaseEvents();

    private void InitUnitCommandStates()
    {
        _unitSelectState = new UnitSelectState(
            this, _selectAction, _infoAction, _cancelAction, _cursorPosition, _ground);
        _unitMoveState = new UnitMoveState(
            this, _selectAction, _infoAction, _cancelAction, _cursorPosition, _ground);
        _unitCommandState = new UnitCommandState(
            this, _cancelAction, _commandUIController);
        _targetSelectState = new TargetSelectState(
            this, _selectAction, _cancelAction, _cursorPosition, _ground);
        _commandEndState = new CommandEndState(this);
    }

    private void InitUI()
    {
        _tileCursor.gameObject.SetActive(false);
        _commandUIController.DeactivateAll();
    }

    public void InitUnitCommand()
    {
        _currentTurnCount = 0;
        StartUnitCommand();
    }

    public void StartUnitCommand()
    {
        _currentTurnCount++;
        SetState(_unitSelectState);
    }

    private void EndUnitCommand()
    {
        SetState(_commandEndState);
    }

    public void SetState(StateBase state)
    {
        OnStateChanged?.Invoke(state);
    }

    public void SetSelectedPlayerUnit(PlayerUnit playerUnit)
    {
        _selectedPlayerUnit = playerUnit;
        _previousPlayerCoord = playerUnit.transform.position.WorldToCoord();
    }

    public void SetSelectedWeaponData(WeaponData weaponData)
    {
        _selectedWeaponData = weaponData;
    }

    public void ShowCursor(Vector2Int coord)
    {
        _tileCursor.SetPosition(coord);
        _tileCursor.Show();
    }

    public void ShowTileInfo(TileBase tile)
    {
        _commandUIController.TileInfoUI.Show();
        _commandUIController.TileInfoUI.SetData(tile);
    }

    public void ShowUnitMoveandAttack(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            tile.HideRange();
            return;
        }
    }

    public void ShowUnitInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _commandUIController.UnitInfoUI.Hide();
            return;
        }

        _commandUIController.UnitInfoUI.Show();
        _commandUIController.UnitInfoUI.SetData(unit);
    }

    public void ShowUnitDetailedInfo(TileBase tile)
    {
        Vector2Int coord = tile.transform.position.WorldToCoord();
        UnitBase unit = _mapManager.UnitMap[coord.x, coord.y];

        if (unit == null)
        {
            _commandUIController.UnitDetailedInfoUI.Hide();
            return;
        }

        _commandUIController.UnitDetailedInfoUI.Show();
        _commandUIController.UnitDetailedInfoUI.SetData(unit);
    }

    public void ShowBattleInfo(UnitStat playerStat, UnitStat enemyStat,
        WeaponData playerWeaponData, WeaponData enemyWeaponData,
        bool canReturnAttack, bool isPlayerPhase)
    {
        _commandUIController.BattleInfoUI.SetData(playerStat, enemyStat, playerWeaponData, enemyWeaponData,
            canReturnAttack, isPlayerPhase);
        _commandUIController.BattleInfoUI.Show();
    }

    private void GameEnd()
    {
        _stateMachine.enabled = false;
    }

    public void HideCursor()
    {
        _tileCursor.Hide();
    }

    public void ShowTileUI(TileBase tile)
    {
        if (tile == null) return;

        Vector2Int coord = tile.transform.position.WorldToCoord();

        ShowUnitInfo(tile);
        ShowCursor(coord);
    }

    public void HideTileUI()
    {
        HideTileInfo();
        HideUnitInfo();
    }

    public void HideTileInfo()
    {
        _commandUIController.TileInfoUI.Hide();
    }

    public void HideUnitInfo()
    {
        _commandUIController.UnitInfoUI.Hide();
    }

    public void HideUnitDetailedInfo()
    {
        _commandUIController.UnitDetailedInfoUI.Hide();
    }

    public void HideBattleInfo()
    {
        _commandUIController.BattleInfoUI.Hide();
    }

    public void ShowAllDanger()
    {
        foreach (EnemyUnit enemyUnit in _showingDangerEnemyList)
        {
            enemyUnit.EnemyAI.ShowDanger();
        }
    }

    public void HideAllDanger()
    {
        foreach (EnemyUnit enemyUnit in _showingDangerEnemyList)
        {
            enemyUnit.EnemyAI.HideDanger();
        }
    }

    private void BindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted += InitUnitCommand;
        _phaseStateController.OnEnemyPhaseStarted += EndUnitCommand;
        _phaseStateController.OnGameEnd += GameEnd;
    }

    private void UnbindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted -= InitUnitCommand;
        _phaseStateController.OnEnemyPhaseStarted -= EndUnitCommand;
        _phaseStateController.OnGameEnd -= GameEnd;
    }
}
