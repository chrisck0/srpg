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
        Debug.Log("Command End");
        _controller.SelectedPlayerUnit?.Action();
        _controller.OnUnitCommandEnd?.Invoke();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
