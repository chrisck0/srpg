using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPhaseState : StateBase
{
    private PhaseStateController _controller;
    private List<UnitBase> _playerUnitList;
    private Queue<PlayerUnit> _activePlayerUnit = new();
    private PlayerUnit _currentPlayerUnit;

    public PlayerPhaseState(PhaseStateController controller,
        Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _controller = controller;

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;

        _playerUnitList = playerUnitList;
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
        _controller.StartEnemyPhase();
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
