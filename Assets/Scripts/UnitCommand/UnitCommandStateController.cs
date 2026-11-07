using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateMachine))]
public class UnitCommandStateController : MonoBehaviour, IStateEvent
{
    [SerializeField] private PhaseStateController _phaseStateController;
    [SerializeField] private DataManager _dataManager;
    [SerializeField] private MapManager _mapManager;
    [SerializeField] private InputActionReference _selectAction;
    [SerializeField] private InputActionReference _cursorPosition;
    [SerializeField] private LayerMask _ground;

    private StateBase _unitSelectState;
    private StateBase _unitMoveState;
    private StateBase _unitCommandState;
    private StateBase _targetSelectState;
    private PlayerUnit _selectedPlayerUnit;

    public Action<StateBase> OnStateChanged { get; set; }

    public DataManager DataManager => _dataManager;
    public MapManager MapManager => _mapManager;
    public StateBase UnitSelectState => _unitSelectState;
    public StateBase UnitMoveState => _unitMoveState;
    public StateBase UnitCommandState => _unitCommandState;
    public StateBase TargetSelectState => _targetSelectState;
    public PlayerUnit SelectedPlayerUnit => _selectedPlayerUnit;

    private void Awake() => InitUnitCommandStates();
    private void OnEnable() => BindPhaseEvents();
    private void OnDisable() => UnbindPhaseEvents();

    private void InitUnitCommandStates()
    {
        _unitSelectState = new UnitSelectState(this, _selectAction, _cursorPosition, _ground);
        _unitMoveState = new UnitMoveState(this);
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
