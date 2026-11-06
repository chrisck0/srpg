using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;
    private List<PlayerUnit> _playerUnitList;
    private Queue<PlayerUnit> _activePlayerUnit = new();
    private Queue<PlayerUnit> _inactivePlayerUnit = new();

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
        }
        Debug.Log("PlayerPhase Start");
        _currentPlayerUnit = null;
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
        _inactivePlayerUnit.Clear();
    }

    private void PlayerPhaseEnd()
    {
        _phaseStateController.SetState(_phaseStateController.EnemyPhaseState);
    }

    private void PlayerActioned()
    {
        UnbindPlayerUnitEvents();
        _currentPlayerUnit.UnbindInputEvents();
        _currentPlayerUnit = null;
    }

    private void BindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnPlayerActioned += PlayerActioned;
    }

    private void UnbindPlayerUnitEvents()
    {
        _currentPlayerUnit.OnPlayerActioned -= PlayerActioned;
    }
}
