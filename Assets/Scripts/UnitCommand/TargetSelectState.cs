using UnityEngine;

public class TargetSelectState : StateBase
{
    private UnitCommandStateController _controller;

    public TargetSelectState(UnitCommandStateController controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        Debug.Log("Target Select");
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
