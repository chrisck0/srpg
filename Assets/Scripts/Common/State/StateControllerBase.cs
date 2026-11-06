using System;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class StateControllerBase : MonoBehaviour, IStateEvent
{
    public Action<StateBase> OnStateChanged { get; set; }
    public abstract void SetState(StateBase state);
}
