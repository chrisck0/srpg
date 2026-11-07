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
    }

    public override void Update()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}
