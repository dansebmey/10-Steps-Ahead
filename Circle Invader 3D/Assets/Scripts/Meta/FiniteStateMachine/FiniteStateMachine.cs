using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FiniteStateMachine
{
    private Dictionary<Type, State> stateDictionary = new Dictionary<Type,State>();
    public State currentState;

    public FiniteStateMachine(Type startState, State[] states)
    {
        foreach (State state in states)
        {
            state.Init(this);
            stateDictionary.Add(state.GetType(), state);
        }
        SwitchState(startState);
    }

    public void SwitchState(Type newStateType)
    {
        currentState?.OnExit();
        currentState = stateDictionary[newStateType];
        currentState?.OnEnter();
    }
}