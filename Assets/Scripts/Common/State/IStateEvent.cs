using UnityEngine;
using System;

public interface IStateEvent
{
    public Action<StateBase> OnStateChanged { get; set; }

    public void SetState(StateBase state);
}