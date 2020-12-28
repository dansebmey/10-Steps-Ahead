using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public abstract class State
{
    protected GameManager Gm;
    protected FiniteStateMachine Fsm;
    
    public virtual void Init(GameManager gm, FiniteStateMachine fsm)
    {
        Gm = gm;
        Fsm = fsm;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}