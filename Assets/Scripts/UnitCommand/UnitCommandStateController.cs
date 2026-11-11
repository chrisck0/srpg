using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class UnitCommandStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileCursor _tileCursor;
    [SerializeField] private CommandUIController _commandUIController;
    [SerializeField] private InputActionReference _selectAction;
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
    private bool _isGameEnd;

    public Action<StateBase> OnStateChanged { get; set; }
    public Action OnUnitCommandEnd;

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

    private void Awake() => InitUnitCommandStates();
    private void OnEnable() => BindPhaseEvents();
    private void Start() => InitUI();
    private void OnDisable() => UnbindPhaseEvents();

    private void InitUnitCommandStates()
    {
        _unitSelectState = new UnitSelectState(
            this, _selectAction, _cursorPosition, _ground);
        _unitMoveState = new UnitMoveState(
            this, _selectAction, _cancelAction, _cursorPosition, _ground);
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
        if (_isGameEnd) return;
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
        TileData tileData = tile.TileData;
        _commandUIController.TileInfoUI.SetData(tileData.Name, tileData.Description);
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
        UnitStat unitStat = unit.UnitStat;
        _commandUIController.UnitInfoUI.SetData(unitStat.UnitData.Name,
            $"HP {unitStat.Health}/{unitStat.UnitData.Health}",
            $"EN {unitStat.Energy}/{unitStat.UnitData.Energy}",
            unit.Weapon.WeaponDataList[0].Name); // 임시로 첫번째 무기 등록
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
        _isGameEnd = true;
    }

    public void HideCursor()
    {
        _tileCursor.Hide();
    }

    public void HideTileInfo()
    {
        _commandUIController.TileInfoUI.Hide();
    }

    public void HideUnitInfo()
    {
        _commandUIController.UnitInfoUI.Hide();
    }

    public void HideBattleInfo()
    {
        _commandUIController.BattleInfoUI.Hide();
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
