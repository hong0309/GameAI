public class StateMachine
{
    public State CurrentState { get; private set; }

    public void ChangeState(State newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }

    public string GetCurrentStateName()
    {
        if (CurrentState == null)
            return "None";

        return CurrentState.GetType().Name;
    }

    public void RestartState(State state)
    {
        CurrentState?.Exit();

        CurrentState = state;

        CurrentState?.Enter();
    }
}