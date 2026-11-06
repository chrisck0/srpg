using UnityEngine;

public class PlayerPhaseState : StateBase
{
    private PhaseStateController _phaseStateController;

    public PlayerPhaseState(PhaseStateController phaseStateController)
    {
        _phaseStateController = phaseStateController;
    }

    public override void Enter()
    {
        _phaseStateController.TestPhaseTransition.OnPlayerPhaseEnd += PlayerPhaseEnd;
        Debug.Log("PlayerPhase Start");
    }

    public override void Update()
    {
        // Debug.Log("PlayerPhase");
    }

    public override void Exit()
    {
        _phaseStateController.TestPhaseTransition.OnPlayerPhaseEnd -= PlayerPhaseEnd;
        Debug.Log("PlayerPhase End");
    }

    private void PlayerPhaseEnd()
    {
        _phaseStateController.SetState(_phaseStateController.EnemyPhaseState);
    }
}
