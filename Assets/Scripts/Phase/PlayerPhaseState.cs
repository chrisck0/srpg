using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;
    private List<PlayerUnit> _playerUnitList;
    private Queue<PlayerUnit> _activePlayerUnit = new();
    private PlayerUnit _currentPlayerUnit;

    public PlayerPhaseState(PhaseStateController phaseStateController)
    {
        _phaseStateController = phaseStateController;
        _playerUnitList = phaseStateController.PlayerUnitList;
    }

    public override void Enter()
    {
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            _activePlayerUnit.Enqueue(playerUnit);
            playerUnit.UnitStat.AddEnergy(5);
        }
        Debug.Log("PlayerPhase Start");
    }

    public override void Update()
    {
        if (_currentPlayerUnit == null)
        {
            if (_activePlayerUnit.Count != 0)
            {
                _currentPlayerUnit = _activePlayerUnit.Dequeue();

                Debug.Log($"Current turn : {_currentPlayerUnit.UnitStat.UnitData.Name}");

                _currentPlayerUnit.BindInputEvents();
                BindPlayerUnitEvents();
            }
            else
            {
                PlayerPhaseEnd();
            }
        }
    }

    public override void Exit()
    {
        _activePlayerUnit.Clear();

        if (_currentPlayerUnit != null)
        {
            UnbindCurrentPlayerUnit();
        }
    }

    private void PlayerPhaseEnd()
    {
        _phaseStateController.SetState(_phaseStateController.EnemyPhaseState);
    }

    private void UnbindCurrentPlayerUnit()
    {
        UnbindPlayerUnitEvents();
        _currentPlayerUnit.UnbindInputEvents();
        _currentPlayerUnit = null;
    }

    private void BindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnPlayerActioned += UnbindCurrentPlayerUnit;
    }

    private void UnbindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnPlayerActioned -= UnbindCurrentPlayerUnit;
    }
}
