using System;
using System.Collections.Generic;

public class FiniteStateMachine
{
    private readonly Dictionary<Type, State> _stateDictionary = new Dictionary<Type,State>();
    public State CurrentState;

    public FiniteStateMachine(GameManager gm, Type startState)
    {
        State[] states = {
            new MenuState(),
            new WaitingForPlayerActionState(),
            new InvokeEnemyActionState(),
            new HighscoreEntryState()
        };
        
        foreach (State state in states)
        {
            state.Init(gm);
            _stateDictionary.Add(state.GetType(), state);
        }
        SwitchState(startState);
    }

    public void SwitchState(Type newStateType)
    {
        CurrentState?.OnExit();
        CurrentState = _stateDictionary[newStateType];
        CurrentState?.OnEnter();
    }
}