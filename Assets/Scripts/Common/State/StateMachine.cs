using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private StateBase _currentState;
    private IStateEvent _stateEvent;

    private void Awake()
    {
        _stateEvent = GetComponent<IStateEvent>();
    }

    private void OnEnable()
    {
        _stateEvent.OnStateChanged += ChangeState;
    }

    public void Update()
    {
        _currentState?.Update();
    }

    private void OnDisable()
    {
        _stateEvent.OnStateChanged -= ChangeState;
    }

    public void ChangeState(StateBase state)
    {
        if (state == null || _currentState == state) return;

        _currentState?.Exit();
        _currentState = state;
        _currentState.Enter();
    }
}
