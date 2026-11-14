using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPhaseState : StateBase
{
    private PhaseStateController _controller;
    private UnitCommandStateController _unitCommandStateController;
    private List<UnitBase> _playerUnitList;
    private int _activePlayerUnit;

    public PlayerPhaseState(PhaseStateController controller,
        UnitCommandStateController unitCommandStateController,
        Dictionary<UnitType,
        List<UnitBase>> spawnedUnitDict)
    {
        _controller = controller;
        _unitCommandStateController = unitCommandStateController;

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;

        _playerUnitList = playerUnitList;
    }

    public override void Enter()
    {
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            playerUnit.Stat.AddEnergy(5);
        }
        _activePlayerUnit = _playerUnitList.Count;
        BindCommandEvents();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            playerUnit.InitAction();
        }
        UnbindCommandEvents();
    }

    private void UnitCommandEnd()
    {
        _activePlayerUnit--;
        if (_activePlayerUnit == 0)
        {
            PlayerPhaseEnd();
        }
        else _unitCommandStateController.StartUnitCommand();
    }

    private void PlayerPhaseEnd()
    {
        _controller.StartEnemyPhase();
    }

    private void BindCommandEvents()
    {
        _unitCommandStateController.OnUnitCommandEnd += UnitCommandEnd;
    }

    private void UnbindCommandEvents()
    {
        _unitCommandStateController.OnUnitCommandEnd -= UnitCommandEnd;
    }
}
