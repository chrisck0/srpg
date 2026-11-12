using UnityEngine;

public sealed class CommandEndState : StateBase
{
    private UnitCommandStateController _controller;

    public CommandEndState(UnitCommandStateController controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.OnUnitCommandEnd?.Invoke();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
