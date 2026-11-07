using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class UnitCommandStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private TileCursor _tileCursor;
    [SerializeField] private UnitCommandUI _unitCommandUI;
    [SerializeField] private AttackCommandUI _attackCommandUI;
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

    public Action<StateBase> OnStateChanged { get; set; }

    public MapManager MapManager => _mapManager;
    public StateBase UnitSelectState => _unitSelectState;
    public StateBase UnitMoveState => _unitMoveState;
    public StateBase UnitCommandState => _unitCommandState;
    public StateBase TargetSelectState => _targetSelectState;
    public StateBase CommandEndState => _commandEndState;
    public PlayerUnit SelectedPlayerUnit => _selectedPlayerUnit;
    public Vector2Int PreviousPlayerCoord => _previousPlayerCoord;
    public WeaponData SelectedWeaponData => _selectedWeaponData;

    private void Awake() => InitUnitCommandStates();
    private void OnEnable() => BindPhaseEvents();
    private void Start() => InitUI();
    private void OnDisable() => UnbindPhaseEvents();

    private void InitUnitCommandStates()
    {
        _unitSelectState = new UnitSelectState(
            this, _selectAction, _cursorPosition, _ground, _tileCursor);
        _unitMoveState = new UnitMoveState(
            this, _selectAction, _cancelAction, _cursorPosition, _ground, _tileCursor);
        _unitCommandState = new UnitCommandState(
            this, _selectAction, _cancelAction, _unitCommandUI, _attackCommandUI);
        _targetSelectState = new TargetSelectState(this);
        _commandEndState = new CommandEndState(this);
    }

    private void InitUI()
    {
        _tileCursor.gameObject.SetActive(false);
        _unitCommandUI.gameObject.SetActive(false);
        _attackCommandUI.gameObject.SetActive(false);
    }

    private void StartUnitCommand()
    {
        SetState(_unitSelectState);
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

    private void BindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted += StartUnitCommand;
    }

    private void UnbindPhaseEvents()
    {
        _phaseStateController.OnPlayerPhaseStarted -= StartUnitCommand;
    }
}
