using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerPhaseState : StateBase
{
    private PhaseStateController _controller;
    private UnitCommandStateController _unitCommandStateController;
    private MapManager _mapManager;
    private List<UnitBase> _playerUnitList;
    private List<UnitBase> _enemyUnitList;
    private List<PlayerUnit> _activePlayerUnit = new();
    private PlayerUnit _currentPlayerUnit;

    public PlayerPhaseState(PhaseStateController controller,
        UnitCommandStateController unitCommandStateController,
        MapManager mapManager,
        Dictionary<UnitType, List<UnitBase>> spawnedUnitDict)
    {
        _controller = controller;
        _unitCommandStateController = unitCommandStateController;
        _mapManager = mapManager;

        if (!spawnedUnitDict.TryGetValue(UnitType.Player, out List<UnitBase> playerUnitList)) return;
        _playerUnitList = playerUnitList;

        if (!spawnedUnitDict.TryGetValue(UnitType.Enemy, out List<UnitBase> enemyUnitList)) return;
        _enemyUnitList = enemyUnitList;
    }

    public override void Enter()
    {
        _activePlayerUnit.Clear();
        foreach (PlayerUnit playerUnit in _playerUnitList)
        {
            _activePlayerUnit.Add(playerUnit);
            playerUnit.Stat.AddEnergy(5);
            AttackScoreCalculator.CalculateScore(
                playerUnit, _mapManager, playerUnit.transform.position.WorldToCoord());
        }

        foreach (EnemyUnit enemyUnit in _enemyUnitList)
        {
            enemyUnit.Stat.AddEnergy(5);
            AttackScoreCalculator.CalculateScore(
                enemyUnit, _mapManager, enemyUnit.transform.position.WorldToCoord());
        }

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

    private void UnitCommandEnd(PlayerUnit actionedPlayerUnit)
    {
        _activePlayerUnit.Remove(actionedPlayerUnit);
        if (_activePlayerUnit.Count == 0)
        {
            PlayerPhaseEnd();
        }
        else
        {
            foreach (PlayerUnit playerUnit in _activePlayerUnit)
            {
                AttackScoreCalculator.CalculateScore(
                    playerUnit, _mapManager, playerUnit.transform.position.WorldToCoord());
            }
            _unitCommandStateController.StartUnitCommand();
        }
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
