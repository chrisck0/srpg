using System;
using UnityEngine;

public class EnemyPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;

    public EnemyPhaseState(PhaseStateController phaseStateController)
    {
        _phaseStateController = phaseStateController;
    }

    public override void Enter()
    {
        _phaseStateController.TestPhaseTransition.OnEnemyPhaseEnd += EnemyPhaseEnd;
        Debug.Log("EnemyPhase Start");
    }

    public override void Update()
    {
        // Debug.Log("EnemyPhase");
    }

    public override void Exit()
    {
        _phaseStateController.TestPhaseTransition.OnEnemyPhaseEnd -= EnemyPhaseEnd;
        Debug.Log("EnemyPhase End");
    }

    private void EnemyPhaseEnd()
    {
        _phaseStateController.SetState(_phaseStateController.PlayerPhaseState);
    }
}
