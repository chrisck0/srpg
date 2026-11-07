using UnityEngine;

public class UnitMoveState : StateBase
{
    private UnitCommandStateController _controller;

    public UnitMoveState(UnitCommandStateController controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        Debug.Log("Move Selected Unit");
    }
    public override void Update()
    {
    }

    public override void Exit()
    {
    }
}
