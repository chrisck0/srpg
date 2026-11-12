using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUnit : UnitBase
{
    private bool _canAction;
    public bool CanAction => _canAction;
    public override UnitType UnitType => UnitType.Player;

    public void InitAction()
    {
        _canAction = true;
    }

    public void Action()
    {
        _canAction = false;
    }
}
